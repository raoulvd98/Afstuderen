import spacy
from spacy.matcher import Matcher
from spacy.tokens import Span
from spacy.pipeline import merge_entities
from spacy import displacy

# url: https://www.youtube.com/watch?v=yHmfOWryK4M&list=PLc2rvfiptPSQgsORc7iuv7UxhbRJox-pW&index=8

nlp = spacy.load('en_core_web_sm')

doc = nlp('Dr. Alex Smith chaired first board meeting at Google')

print([(ent.text, ent.label_) for ent in doc.ents]) 

def add_title(doc):
    new_ents = []
    for ent in doc.ents:
        if ent.label_ == 'PERSON' and ent.start !=0:
            prev_token = doc[ent.start-1]
            if prev_token.text in ('Dr', 'Dr.', 'Mr', 'Mr.', 'Mrs', 'Mrs.'):
                new_ent = Span(doc, ent.start-1, ent.end, label=ent.label)
                new_ents.append(new_ent)
            else:
                new_ents.append(ent)
    doc.ents = new_ents
    return doc

#nlp.add_pipe(add_title, after='ner')
doc = nlp('Dr. Alex Smith chaired first board meeting at Google')
print([(ent.text, ent.label_) for ent in doc.ents]) 

print("__________________________")
print("__________________________")

def get_person_orgs(doc):
    print(doc.text)
    person_entities = [ent for ent in doc.ents if ent.label_ == "PERSON"]
    for ent in person_entities:
        head = ent.root.head
        if head.lemma_ == 'work':
            preps = [token for token in head.children if token.dep_ == 'prep']
            for prep in preps:
                orgs = [token for token in prep.children if token.ent_type_ == 'ORG']

                aux = [token for token in head.children if token.dep_ == 'aux']
                past_aux = any(t.tag_ == 'VBD' for t in aux)
                past = head.tag_ == "VBD" or head.tag_ == "VBG" and past_aux
            
                print({'person': ent, 'orgs': orgs, 'past': past})
    return doc

nlp.add_pipe(merge_entities)
nlp.add_pipe(get_person_orgs)
doc = nlp('Alex Smith is working at Google')