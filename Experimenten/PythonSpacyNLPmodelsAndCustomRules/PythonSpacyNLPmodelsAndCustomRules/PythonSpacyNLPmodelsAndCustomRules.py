import sys
import spacy
from spacy.tokens import Span
from spacy.pipeline import merge_entities
from spacy.pipeline import merge_noun_chunks
from spacy.attrs import ORTH, NORM

test_corpus = [
    ("let op!  prijs ex btw en ex bpm!", "wrong_btw"), 
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
    ("De zakelijke prijs is €15000 exclusief btw", "good_btw")
]

def special_cases(nlp):
    case = [{ORTH: "exclusief"}]
    nlp.tokenizer.add_special_case("excl.", case)

def create_NLP_Pipeline():
    nlp = spacy.load('nl_core_news_sm')
   
    nlp.add_pipe(merge_entities)
    nlp.add_pipe(merge_noun_chunks)
    nlp.add_pipe(check_btw, after="ner")

    special_cases(nlp)
    return nlp 

def check_btw(doc):
    doc.user_data['is_valid_class'] = "not_set"
    btw_list = ('btw', 'belasting', 'toegevoegde', 'waarde', 'b.t.w', 'b.t.w.')
    negative_list = ('zonder', 'uitgezonderd', 'bevenens', 'behalve', 'exclusief', 'ex', 'excl.', 'excl', )
    for token in doc:
        head = token.head
        if (token.lemma_ in btw_list):
            if (head.lemma_ in negative_list) or (token.left_edge.lemma_ in negative_list) or (token.right_edge.lemma_ in negative_list) or (head.left_edge.lemma_ in negative_list) or (head.right_edge.lemma_ in negative_list):
                doc.user_data['is_valid_class'] = "wrong_btw"
            else:
                for con in head.conjuncts:
                    if con.lemma_ in negative_list:
                        doc.user_data['is_valid_class'] = "wrong_btw"
        elif (token.lemma_ in negative_list):
            if (head.lemma_ in btw_list):
                doc.user_data['is_valid_class'] = "wrong_btw"

    if doc.user_data['is_valid_class'] == "not_set":
        doc.user_data['is_valid_class'] = "good_btw"
    return (doc)

def Main(test_corpus):     
    nlp = create_NLP_Pipeline()
    print(nlp.pipe_names)

    for sentence in test_corpus:
        prediction = nlp(sentence[0])

        good_or_wrong = False
        if sentence[1] == prediction.user_data['is_valid_class']: 
            good_or_wrong = True

        ftm = "{0} - {1} - {2}"
        print (ftm.format(good_or_wrong, prediction.user_data['is_valid_class'], sentence[0]))
        print("________________________________")

Main(test_corpus)