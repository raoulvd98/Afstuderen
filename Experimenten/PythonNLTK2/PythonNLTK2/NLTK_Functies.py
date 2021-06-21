from nltk.tokenize import sent_tokenize, word_tokenize
from nltk.stem import PorterStemmer
from nltk.stem.wordnet import WordNetLemmatizer 
from nltk.tokenize import PunktSentenceTokenizer
import nltk.parse.api
import nltk
import re

#stopword_list = nltk.corpus.stopwords.words('dutch')
stopword_list = nltk.corpus.stopwords.words('English')
# input can only be text (change later)
def remove_stopwords(input_text):
    words = input_text.split()
    stopword_free_words = [word for word in words if word not in stopword_list]
    stopword_free_text = " ".join(stopword_free_words)
    return stopword_free_text

# input can only be text (change later)
def remove_pattern(input_text, pattern_to_remove):
    ## Example: if pattern = "#[\w]*"  then is removes al tokens which starts with a hashtag
    urls = re.finditer(pattern_to_remove, input_text)
    for i in urls:
        input_text = re.sub(i.group().strip(), '', input_text)
    return input_text

def get_list(input):
    if (isinstance(input, list)):
        return input
    elif (isinstance(input, str)):
        return tokenize(input)
    elif not (isinstance(input, list)):
        return "Error occured, the input type have to a list or a string"

def pos_tagger(input):
    if (isinstance(input, str)):
        input = nltk.word_tokenize(input)
    return nltk.pos_tag(input)

def stemming(input):
    input = pos_tagger(input)
 
    # bringing words to their root form
    stemmer = PorterStemmer()
    for i in range(len(input)):
        stemmed_word = stemmer.stem(input[i][0])
        pos_tag = input[i][1]
        input[i] = (stemmed_word, pos_tag)
    
    return input

def Lemmatization(input):
    input = pos_tagger(input)

    lem = WordNetLemmatizer()
    for i in range(len(input)):
        input[i] = lem.lemmatize(input[i])


print(Lemmatization("this is a test sample"))