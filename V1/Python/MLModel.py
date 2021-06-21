import random
from pathlib import Path
import spacy
from spacy.util import minibatch, compounding

class MLModel:

    def __init__(self, data, category):
        self.data = data
        self.category = category
    
    def TrainModel (self, nlp=None, output_dir=None, n_iter=20, n_texts=2000, init_tok2vec=None):
        if output_dir is not None:
            output_dir = Path(output_dir)
            if not output_dir.exists():
                output_dir.mkdir()

        if nlp == None:
            nlp = spacy.load('nl_core_news_sm')
            print("Created blank 'nl' model")

        if "textcat" not in nlp.pipe_names:
            textcat = nlp.create_pipe(
                "textcat", config={"exclusive_classes": True, "architecture": "simple_cnn"}
            )
            nlp.add_pipe(textcat, last=True)
        else:
            textcat = nlp.get_pipe("textcat")

        textcat.add_label("WrongSentence")
        textcat.add_label("GoodSentence")

        print(nlp.pipe_names)

        print("Loading data...")
        (train_texts, train_cats), (dev_texts, dev_cats) = self.LoadData()
        train_data = list(zip(train_texts, [{"cats": cats} for cats in train_cats]))
        print(
            "Using {} examples ({} training, {} evaluation)".format(
                len(train_texts)+len(dev_texts), len(train_texts), len(dev_texts)
            )
        )

        # get names of other pipes to disable them during training
        pipe_exceptions = ["textcat", "trf_wordpiecer", "trf_tok2vec"]
        other_pipes = [pipe for pipe in nlp.pipe_names if pipe not in pipe_exceptions]
        score_list = []
        with nlp.disable_pipes(*other_pipes):  # only train textcat
            optimizer = nlp.begin_training()
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
                    nlp.update(texts, annotations, sgd=optimizer, drop=0.2, losses=losses)
                with textcat.model.use_params(optimizer.averages):
                    # evaluate on the dev data split off in load_data()
                    scores = self.Evaluate(nlp.tokenizer, textcat, dev_texts, dev_cats)
                    score_list.append ( 
                        { "Loss": losses["textcat"], 
                        "Precision": scores["textcat_p"], 
                        "Recall": scores["textcat_r"], 
                        "F-score": scores["textcat_f"] } 
                    )
                print(
                    "{0:.3f}\t{1:.3f}\t{2:.3f}\t{3:.3f}".format(  # print a simple table
                        losses["textcat"],
                        scores["textcat_p"],
                        scores["textcat_r"],
                        scores["textcat_f"],
                    )
                )

        if output_dir is not None:
            with nlp.use_params(optimizer.averages):
                nlp.to_disk(output_dir)
            print("Saved model to", output_dir)

        return score_list, nlp

    def LoadData (self, split = 0.8):
        test_corpus = []

        for example in self.data:
            if (example['category'] == self.category):
                test_corpus.append( (example['example'], example['isPositive']) )
        random.shuffle(test_corpus)
        
        cats = []
        texts = []
        for sentence in test_corpus:
            if (sentence[1] == False):
                texts.append(sentence[0])
                cats.append({'GoodSentence': False, 'WrongSentence': True})
            elif (sentence[1] == True):
                texts.append(sentence[0])
                cats.append({'GoodSentence': True, 'WrongSentence': False})

        split = int(len(texts) * split)
        return (texts[:split], cats[:split]), (texts[split:], cats[split:])

    def Evaluate(self, tokenizer, textcat, texts, cats):
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
                if label == "WrongSentence":
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