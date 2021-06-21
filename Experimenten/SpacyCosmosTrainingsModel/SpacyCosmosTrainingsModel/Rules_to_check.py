import random
from pathlib import Path
import spacy
from spacy.util import minibatch, compounding

class Rules:
    def load_data(self, split=0.8):
        training_examples = [
            ("let op! prijs ex btw en ex bpm!", "wrong_btw"), 
            ("prijs is exclusief btw.", "wrong_btw"), 
            ("voor de meeneemprijs is de auto rijklaar, inclusief btw, bpm, apk en leges.", "good_btw"), 
            ("de genoemde verkoopprijs is een basisprijs exclusief btw, bpm en leges.", "wrong_btw"), 
            ("de genoemde verkoopprijs is een basisprijs inclusief btw, bpm en leges. ", "good_btw"),
            ("verkoopprijs is exclusief btw", "wrong_btw"),
            ("de prijs is inclusief btw", "good_btw"),
            ("De verkoopprijs is inclusief btw", "good_btw"),
            ("De verkoopprijs is exclusief btw", "wrong_btw"),
            ("Deze prijs is inclusief btw", "good_btw"),
            ("Deze prijs is exclusief btw", "wrong_btw"),
            ("De rijklaarprijs is incl. btw", "good_btw"),
            ("De rijklaarprijs is excl. btw", "wrong_btw"),
            ("De rijklaarprijs is getoond met de btw daarbij inbegrepen", "good_btw"),
            ("De rijklaarprijs is zonder de btw", "wrong_btw"),
            ("Zakelijk kost deze auto €23000 ex btw", "good_btw"),
            ("De zakelijk rijklaarprijs is €17000 exclusief btw", "good_btw"),
            ("De rijklaarprijs is €25000 exclusief btw", "wrong_btw"),
            ("De verkoopprijs is exclusief btw", "wrong_btw"),
        ]

        random.shuffle(training_examples)
        cats = []
        texts = []

        for example in training_examples:
            if (example[1] == "wrong_btw"):
                texts.append(example[0])
                cats.append({'good_btw': False, 'wrong_btw': True})
            elif (example[1] == "good_btw"):
                texts.append(example[0])
                cats.append({'good_btw': True, 'wrong_btw': False})

        split = int(len(texts) * split)
        return (texts[:split], cats[:split]), (texts[split:], cats[split:])

    def evaluate(self, tokenizer, textcat, texts, cats):
        docs = (tokenizer(text) for text in texts)
        tp = 0.0  # True good_btw
        fp = 1e-8  # False good_btw
        fn = 1e-8  # False wrong_btw
        tn = 0.0  # True wrong_btw
        for i, doc in enumerate(textcat.pipe(docs)):
            gold = cats[i]
            for label, score in doc.cats.items():
                if label not in gold:
                    continue
                if label == "wrong_btw":
                    continue
                if score >= 0.5 and gold[label] >= 0.5:
                    tp += 1.0
                elif score >= 0.5 and gold[label] < 0.5:
                    fp += 1.0
                elif score < 0.5 and gold[label] < 0.5:
                    tn += 1
                elif score < 0.5 and gold[label] >= 0.5:
                    fn += 1
        precision = tp / (tp + fp) # Precision is the Positive Predictive Value
        recall = tp / (tp + fn) # Recall is the True Positive Rate
        if (precision + recall) == 0:
            f_score = 0.0
        else:
            f_score = 2 * (precision * recall) / (precision + recall)
        return {"textcat_p": precision, "textcat_r": recall, "textcat_f": f_score}

class BTW (Rules):
    nlp = "not_defined"

    def main(self, model=None, output_dir=None, n_iter=20, n_texts=2000, init_tok2vec=None):
        if output_dir is not None:
            output_dir = Path(output_dir)
            if not output_dir.exists():
                output_dir.mkdir()

        if model is not None:
            self.nlp = spacy.load(model)
            print("Loaded model '%s'" % model)
        else:
            self.nlp = spacy.load('nl_core_news_sm')
            self.nlp.disable_pipes(["tagger", "parser", "ner"])
            print("Created blank 'nl' model")

        if "textcat" not in self.nlp.pipe_names:
            textcat = self.nlp.create_pipe(
                "textcat", config={"exclusive_classes": True, "architecture": "simple_cnn"}
            )
            self.nlp.add_pipe(textcat, last=True)
        else:
            textcat = self.nlp.get_pipe("textcat")

        textcat.add_label("wrong_btw")
        textcat.add_label("good_btw")

        print("Loading data...")
        (train_texts, train_cats), (dev_texts, dev_cats) = self.load_data()
        train_data = list(zip(train_texts, [{"cats": cats} for cats in train_cats]))
        print(
            "Using {} examples ({} training, {} evaluation)".format(
                len(train_texts)+len(dev_texts), len(train_texts), len(dev_texts)
            )
        )

        # get names of other pipes to disable them during training
        pipe_exceptions = ["textcat", "trf_wordpiecer", "trf_tok2vec"]
        other_pipes = [pipe for pipe in self.nlp.pipe_names if pipe not in pipe_exceptions]
        with self.nlp.disable_pipes(*other_pipes):  # only train textcat
            optimizer = self.nlp.begin_training()
            if init_tok2vec is not None:
                with init_tok2vec.open("rb") as file_:
                    textcat.model.tok2vec.from_bytes(file_.read())
            print("Training the model...")
            print("{:^5}\t{:^5}\t{:^5}\t{:^5}".format("LOSS", "P", "R", "F"))
            batch_sizes = compounding(4.0, 32.0, 1.001)
            for i in range(n_iter):
                losses = {}
                # batch up the examples using spaCy's minibatch
                random.shuffle(train_data)
                batches = minibatch(train_data, size=batch_sizes)
                for batch in batches:
                    texts, annotations = zip(*batch)
                    self.nlp.update(texts, annotations, sgd=optimizer, drop=0.2, losses=losses)
                with textcat.model.use_params(optimizer.averages):
                    # evaluate on the dev data split off in load_data()
                    scores = self.evaluate(self.nlp.tokenizer, textcat, dev_texts, dev_cats)
                print(
                    "{0:.3f}\t{1:.3f}\t{2:.3f}\t{3:.3f}".format(  # print a simple table
                        losses["textcat"],
                        scores["textcat_p"],
                        scores["textcat_r"],
                        scores["textcat_f"],
                    )
                )

    def predict(self, text_to_check):
        doc = self.nlp(text_to_check)
        print(text_to_check, doc.cats)