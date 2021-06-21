from nltk.tokenize import sent_tokenize
from nltk.tokenize import TreebankWordTokenizer
from nltk.corpus import alpino as alp
from nltk.tag import UnigramTagger, BigramTagger
from nltk import pos_tag, RegexpParser
import nltk
import re

test_corpus = [
    ("De prijzen zijn zonder btw", "wrong_btw"), 
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

def special_cases(doc):
    doc = doc.replace(' incl. ', ' inclusief ')
    doc = doc.replace(' excl. ' , ' exclusief ').replace(' ex ' , ' exclusief ').replace(' excl ' , ' exclusief ')
    return doc

def create_tree(input):
    grammar = ('''  KEYWORDS: { <prep> <det>? <None> | <adj> <det>? <None> | <None> }
                    Noun : { <det> <noun> } ''')
    chunkParser = RegexpParser(grammar)
    new_list = []
    for token in input:
        if token[1] is None:
            new_tuple = (token[0], "None")
            new_list.append(new_tuple)
        else:
            new_list.append(token)

    tree = chunkParser.parse(new_list)
    return tree

def check_btw(tree):
    is_valid_class = "not_set"
    btw_list = ('btw', 'belasting', 'toegevoegde', 'waarde', 'b.t.w', 'b.t.w.')
    negative_list = ('zonder', 'uitgezonderd', 'bevenens', 'behalve', 'exclusief')

    for subtree in tree.subtrees():
        if subtree._label == 'KEYWORDS':
            btw = False
            negative = False
            for x in range(len(subtree)) :
                if subtree[x][0] in btw_list : btw = True
                if subtree[x][0] in negative_list : negative = True  
            if (btw == True and negative == True): 
                is_valid_class = "wrong_btw"
    if is_valid_class == "not_set":
        is_valid_class = "good_btw"
    return is_valid_class

def create_NLP_Pipeline(doc, training_corpus, tree_tokenizer, pos_tag):
    sentence_tokens = sent_tokenize(special_cases(doc))
    check_btw_doc = "good_btw"
    for sentence in sentence_tokens:
        word_tokens = tree_tokenizer.tokenize(sentence)
        tagged_tokens = pos_tag(word_tokens)
        tree = create_tree( tagged_tokens )
        check_btw_doc = check_btw( tree )
        if check_btw_doc == "wrong_btw": break
    return check_btw_doc

def Main(test_corpus):     
    training_corpus = alp.tagged_sents()
    tree_tokenizer = TreebankWordTokenizer()
    unitagger = UnigramTagger(training_corpus)
    bitagger = BigramTagger(training_corpus, backoff=unitagger)
    for doc in test_corpus:
        prediction = create_NLP_Pipeline(doc[0], training_corpus, tree_tokenizer, bitagger.tag)

        good_or_wrong = False
        if doc[1] == prediction: good_or_wrong = True
        
        ftm = "{0} - {1} - {2}"
        print (ftm.format(good_or_wrong, prediction, doc[0]))
        print("________________________________")

Main(test_corpus)