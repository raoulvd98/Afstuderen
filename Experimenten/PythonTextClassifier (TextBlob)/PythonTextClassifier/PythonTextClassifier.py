import sys
from textblob.classifiers import NaiveBayesClassifier as NBC
from textblob import TextBlob

training_corpus = [
    ("actieprijs rijklaar €12.690,00 ex btw - bpm.", "wrong_btw"),
    ("let op: de prijs is ex btw", "wrong_btw"),
    ("getoonde prijs is ex btw", "wrong_btw"),
    ("prijs is exclusief btw", "wrong_btw"),
    ("verkooppprijs is exclusief b.t.w", "wrong_btw"),
    ("de verkoopprijs is exclusief btw", "wrong_btw"),
    ("deze prijs is excl. btw", "wrong_btw"),
    ("de prijzen van de auto's zijn exclusief bpm, btw en alle kosten met nederlands kenteken, tenzij anders vermeld", "wrong_btw"),
    ("exclusief btw, bpm en rijklaar maken.", "wrong_btw"),
    ("verkooppprijs is exclusief b.t.w", "wrong_btw"),
    ("prijs is exclusief btw en de auto is geschikt voor de mia, kia en vamil regeling.", "wrong_btw"),
    ("de vermelde verkoopprijs is de rijklaar basisprijs exclusief: 6 maanden bovag garantie, vloeistofcontrole, geldige apk, reinigingsbeurt, btw en bpm.", "wrong_btw"),
    ("deze prijs is excl. btw", "wrong_btw"),
    ("de verkoopprijs is exclusief btw", "wrong_btw"),

    ("actieprijs rijklaar €12.690,00 incl. btw - bpm.", "good_btw"),
    ("let op: de prijs is incl. btw", "good_btw"),
    ("getoonde prijs is incl. btw", "good_btw"),
    ("prijs is inclusief btw", "good_btw"),
    ("verkooppprijs is inclusief b.t.w", "good_btw"),
    ("de verkoopprijs is inclusief btw", "good_btw"),
    ("deze prijs is incl. btw", "good_btw"),
    ("de prijzen van de auto's zijn inclusief bpm, btw en alle kosten met nederlands kenteken, tenzij anders vermeld", "good_btw"),
    ("inclusief btw, bpm en rijklaar maken.", "good_btw"),
    ("verkooppprijs is inclusief b.t.w", "good_btw"),
    ("prijs is inclusief btw en de auto is geschikt voor de mia, kia en vamil regeling.", "good_btw"),
    ("de vermelde verkoopprijs is de rijklaar basisprijs inclusief: 6 maanden bovag garantie, vloeistofcontrole, geldige apk, reinigingsbeurt, btw en bpm.", "good_btw"),
    ("deze prijs is incl. btw", "good_btw"),
    ("de verkoopprijs is inclusief btw", "good_btw"),
]

test_corpus = [
    #("let op!  prijs ex btw en ex bpm!", "wrong_btw"), 
    #("prijs is exclusief btw.", "wrong_btw"), 
    #("voor de meeneemprijs is de auto rijklaar, inclusief btw, bpm, apk en leges.", "good_btw"), 
    #("de genoemde verkoopprijs is een basisprijs exclusief btw, bpm en leges.", "wrong_btw"), 
    #("de genoemde verkoopprijs is een basisprijs inclusief btw, bpm en leges. ", "good_btw"),
    #("verkoopprijs is exclusief btw", "wrong_btw"),
    #("de prijs is inclusief btw", "good_btw"),
    #("De verkoopprijs is inclusief btw", "good_btw"),
    #("De verkoopprijs is exclusief btw", "wrong_btw"),
    #("Deze prijs is inclusief btw", "good_btw"),
    #("Deze prijs is exclusief btw", "wrong_btw"),
    #("De rijklaarprijs is incl. btw", "good_btw"),
    #("De rijklaarprijs is excl. btw", "wrong_btw"),
    ("De rijklaarprijs is getoond met de btw daarbij inbegrepen", "good_btw"),
    ("De rijklaarprijs is zonder de btw", "wrong_btw"),
]

model = NBC(training_corpus) 

for sentence in test_corpus:
    good_or_wrong = False
    if sentence[1] == model.classify(sentence[0]):
        good_or_wrong = True

    ftm = "{0} - {1} - {2}"
    print (ftm.format(good_or_wrong, model.classify( sentence[0] ), sentence[0]))

    print("________________________________")