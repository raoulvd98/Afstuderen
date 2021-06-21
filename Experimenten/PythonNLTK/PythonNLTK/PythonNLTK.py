from nltk.tokenize import sent_tokenize, word_tokenize
from nltk.stem import PorterStemmer
from nltk.tokenize import PunktSentenceTokenizer
import nltk.parse.api
import nltk

class Sentense:
    #stopword_list = nltk.corpus.stopwords.words('dutch')
    stopword_list = nltk.corpus.stopwords.words('English')
    def __init__(self, number, untokenized_sentense):
        self.number = number
        self.untokenized_sentense = untokenized_sentense
        self.result = []

    def PoS_tagging(self):
        self.result = nltk.pos_tag(nltk.word_tokenize(self.untokenized_sentense))

    def PorterStemmer(self):
        # bringing words to their root form
        stemmer = PorterStemmer()
        for i in range(len(self.result)):
            stemmed_word = stemmer.stem(self.result[i][0])
            pos_tag = self.result[i][1]
            self.result[i] = (stemmed_word, pos_tag)

    def Filter_StopWords(self):
        # Check all words if they are in the stopword list or not
        filter_list = []
        for tuple in self.result:
            if tuple[0] not in self.stopword_list:
                filter_list.append(tuple)

        self.result = filter_list

class Advertisement:
    def __init__(self, title, text):
        self.title = title
        self.text = text
        self.splitted_text = "not set"
        self.list_of_sentences = []

    def Sentence_Segmentation(self):
        # split the text in sentences
        self.splitted_text = sent_tokenize(self.text)

        # make for every sentence a Sentence object
        for i in range(len(self.splitted_text)):
           self.list_of_sentences.append( Sentense(i, self.splitted_text[i]) )

def main():
    #data = "Deze Volvo kan optrekken van nul tot honderd in zes seconden. Ook heeft hij een topsnelheid van 250 kilometer per uur."
    data = "He likes playing football, but not too much"
    
    advertisement_to_check = Advertisement("Volvo V60", data)
    advertisement_to_check.Sentence_Segmentation()

    for i in advertisement_to_check.list_of_sentences:

        #de gehele zin
        print(i.untokenized_sentense)
        print("__________________________")

        # postagging
        i.PoS_tagging()
        print(i.result)
        print("__________________________")

        # stemming
        i.PorterStemmer()
        print(i.result)
        print("__________________________")

        # identify and remove stopwords
        i.Filter_StopWords()
        print(i.result)
        print("__________________________")
        print("__________________________")

        print(i.stopword_list)

main()