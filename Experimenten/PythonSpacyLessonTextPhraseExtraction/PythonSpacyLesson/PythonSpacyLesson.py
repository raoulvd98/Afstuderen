import spacy
from spacy.matcher import Matcher
from spacy.tokens import Span
from spacy import displacy

nlp = spacy.load('en_core_web_sm')

#setup document
doc = nlp('Hello, world!')

# print all tokens in your document
for token in doc:
    print(token)

# print seperation line
print("__________________________")

#create pattern
pattern_helloworld = [{"LOWER": "hello"}, {"IS_PUNCT": True, 'OP': '?'}, {"LOWER": "world"}]
pattern_world = [{"IS_PUNCT": True, 'OP': '?'}, {"LOWER": "world"}]

#create matcher and add your patters
matcher = Matcher(nlp.vocab)
matcher.add('HelloWorld', None, pattern_helloworld)
matcher.add('World', None, pattern_world)

#chech if patterns matches document
matches = matcher(doc)

#print all matches
for match_id, start, end in matches:
    string_id = nlp.vocab.strings[match_id]
    span = doc[start:end]
    print(match_id, string_id, start, end, span.text)

# print seperation lines
print("__________________________")
print("__________________________")

pattern_Google_io = [{'TEXT':'Google'}, {'TEXT': "I"}, {'TEXT': "/"}, {'TEXT': "O"}]

def callback_method(matcher, doc, i, matches):
    match_id, start, end = matches[i]
    string_id = nlp.vocab.strings[match_id]
    entity = doc[start:end]
    print(match_id, string_id, start, end, entity.text)


matcher = Matcher(nlp.vocab)
matcher.add('Google', callback_method, pattern_Google_io)
doc = nlp('Google announced a new Pixel at Google I/O. Google I/O is a great place te get all updates from Google.')
matcher(doc)