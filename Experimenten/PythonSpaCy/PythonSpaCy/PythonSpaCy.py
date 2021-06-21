import spacy
from spacy import displacy
from spacy.matcher import Matcher

class Sentense:
    def __init__(self, text):
        self.sentense = text
        self.nlp = spacy.load('nl_core_news_sm')

    def word_Tokenization(self):
        self.tokenized_list = []
        for token in self.sentense:
            self.tokenized_list.append(token)

    def pos_Tagging(self):
        self.tagged_list = []
        for token in self.tokenized_list:
            token_tuple = (token, token.pos_)
            self.tagged_list.append(token_tuple)

    def lemmatization(self):
        self.lemma_list = []
        for token in self.tagged_list:
            token_tuple = (token[0], token[0].lemma_, token[1])
            self.lemma_list.append(token_tuple)

    def remove_Stopwords(self):
        self.list_without_stopwords = []
        for token in self.lemma_list:
            if token[0].is_stop == False:
                self.list_without_stopwords.append(token)
   
    #the different types are 'dep' (for Dependency Parsing) and 'ent' (for Named Entity Recognition)
    def visualize(self, type = 'dep', jupyter_mode = True):
        sentense_to_visualize = self.nlp(self.sentense.string)
        #displacy.serve(sentense_to_visualize, style=type,)
        #displacy.render(sentense_to_visualize, style=type, page=False, jupyter=jupyter_mode)
        displacy.serve(sentense_to_visualize, style=type)

class Advertisement:
    def __init__(self, text):
        self.text = text
        self.list_of_sentences = []
        self.nlp = spacy.load('nl_core_news_sm')

    def sentence_Segmentation(self):
        # split the text in sentences
        self.splitted_text = self.nlp(self.text)
        self.splitted_text = list(self.splitted_text.sents)

        # make for every sentence a Sentence object
        for i in range(len(self.splitted_text)):
           self.list_of_sentences.append ( Sentense (self.splitted_text[i]))

def main():
    data = "Deze Volvo kan optrekken van nul tot honderd in zes seconden. Ook heeft hij een topsnelheid van 250 kilometer per uur."
    advertisement_to_check = Advertisement(data)
    advertisement_to_check.sentence_Segmentation()

    for i in advertisement_to_check.list_of_sentences:
        #The whole sentence
        i.word_Tokenization()
        print(i.tokenized_list)
        print("__________________________")

        #postagging
        i.pos_Tagging()
        print(i.tagged_list)
        print("__________________________")

        #lemma/stemming
        i.lemmatization()
        print(i.lemma_list)
        print("__________________________")

        #Remove stopwords and punctuation
        i.remove_Stopwords()
        print(i.list_without_stopwords)
        print("__________________________")

        #i.visualize()
        i.visualize('ent')
        #print(i.list_without_stopwords)
        print("__________________________")

        print("__________________________")
        print("next")
        print("next")
        print("next")
        print("__________________________")
        print("__________________________")
#main()

