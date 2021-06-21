from nltk.tokenize import sent_tokenize, word_tokenize
from nltk.stem import PorterStemmer
from nltk.stem.wordnet import WordNetLemmatizer 
from nltk.tokenize import PunktSentenceTokenizer
from nltk.grammar import DependencyGrammar
from nltk.parse import (
    DependencyGraph,
    ProjectiveDependencyParser,
    NonprojectiveDependencyParser,
)
from nltk.corpus import treebank
import nltk
import re

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

def pos_tagger(input):
    if (isinstance(input, str)):
        input = nltk.word_tokenize(input)
    return nltk.pos_tag(input)


def ceate_nlp_pipeline(sentence):
    #tagger
    tagged = pos_tagger(sentence)
    #parser
    entities = nltk.chunk.ne_chunk(tagged)
    print(entities)
    #ner

    #check btw
    

    return

def main(doc):
    nlp = ceate_nlp_pipeline(doc[0][0])

main(test_corpus)