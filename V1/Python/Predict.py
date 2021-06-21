import random
from pathlib import Path
import spacy
from spacy.pipeline import Sentencizer
from spacy.util import minibatch, compounding

class PredictAdvertisement():
    
    def __init__(self, advertisement, models):
        self.advertisement = advertisement
        self.models = models

    def Pipeline(self):
        predictionDict = []
        predictionDict.append( self.CheckBtw() )
        predictionDict.append( self.CheckPriceAllIn() )
        predictionDict.append( self.CheckPriceIsSalesPrice() )
        predictionDict.append( self.CheckVehicleType() )
        predictionDict.append( self.CheckBovagGuarantee() )
        predictionDict.append( self.CheckExchangeGuarantee() )
        predictionDict.append( self.CheckAdditionalAssurance() )
        return predictionDict

    def ReplaceCharacters(self, inputText):
        inputText = inputText.replace("<br />", ". ")
        inputText = inputText.replace("incl.", "inclusief")
        inputText = inputText.replace("excl.", "exclusief")
        inputText = inputText.replace("\n", ". ")
        inputText = inputText.replace("\r", ". ")
        inputText = inputText.replace(". .", ".")
        return inputText

    def sentencizer(self, nlp):
        try: 
            sentencizer = Sentencizer(punct_chars=[".", "?", "!", "。", "|"])
            nlp.add_pipe(sentencizer, first=True)
        except:
            pass
            
        return nlp

    def NlpCheck(self, cat, nlp, PartsToCheck, importantWords = None):
        ListOfSentences = []

        with nlp.disable_pipes("textcat"):
            for part in PartsToCheck:
                docs = nlp(part)
                for sent in docs.sents:
                    ListOfSentences.append(sent)

        result = {'model': cat, 'WrongSentence': 0.0, 'GoodSentence': 0.0}
        with nlp.disable_pipes("sentencizer"):
            for sentence in ListOfSentences:
                if (importantWords == None) or (any(word in sentence.string.lower() for word in importantWords)):
                    docs = nlp(sentence.string)
                    if docs.cats["WrongSentence"] > result["WrongSentence"]:
                        result["WrongSentence"] = docs.cats["WrongSentence"]
                        result["GoodSentence"] = docs.cats["GoodSentence"]
                    
        return result
    
    def CheckBtw(self):
        result = {'model': 'btw', 'WrongSentence': -1.0, 'GoodSentence': -1.0}
        
        if (self.advertisement["AdvertisementContent"]["Car"]["Price"]["ExclusiveBTW"] == True):
            return {'model': 'btw', 'WrongSentence': 0.0, 'GoodSentence': 1.0}
        
        PartsToCheck = [ 
            self.ReplaceCharacters(self.advertisement["AdvertisementContent"]["Car"]["General"]["Title"]), 
            self.ReplaceCharacters(self.advertisement["AdvertisementContent"]["Car"]["General"]["ConsumentComments"]),
            self.ReplaceCharacters(self.advertisement["AdvertisementContent"]["Car"]["General"]["TradeComments"])
        ]

        importantWords = ["btw", "toegevoegd", "belasting", "waarde", "inclusief", "exclusief", "incl", "excl", "ex" ]
        nlp = self.models['btw']


        if nlp == None:
            return result

        nlp = self.sentencizer(nlp)

        result = self.NlpCheck('btw', nlp, PartsToCheck, importantWords)

        print(nlp.pipe_names)
                    
        return result
        
    def CheckPriceAllIn(self):
        result = {'model': 'priceAllIn', 'WrongSentence': -1.0, 'GoodSentence': -1.0 }
        return result

    def CheckPriceIsSalesPrice(self): 
        result = {'model': 'PriceIsSale', 'WrongSentence': -1.0, 'GoodSentence': -1.0}
        return result

    def CheckVehicleType(self):
        result = {'model': 'vehicleType', 'WrongSentence': -1.0, 'GoodSentence': -1.0}
        return result

    def CheckBovagGuarantee(self):
        result = {'model': 'bovagGuarantee', 'WrongSentence': -1.0, 'GoodSentence': -1.0}

        if (self.advertisement["AdvertisementContent"]["Car"]["General"]["NewOccasion"] == "nieuw" ) or not (self.advertisement["AdvertisementContent"]["Car"]["Guarantee"]["GuaranteeBrand"] == None or self.advertisement["AdvertisementContent"]["Car"]["Guarantee"]["GuaranteeBrand"] == False):
            result = {'model': 'bovagGuarantee', 'WrongSentence': 0.0, 'GoodSentence': 1.0}
            return result

        PartsToCheck = [ 
            self.ReplaceCharacters(self.advertisement["AdvertisementContent"]["Car"]["General"]["ConsumentComments"]),
            self.ReplaceCharacters(self.advertisement["AdvertisementContent"]["Car"]["General"]["TradeComments"])
        ]

        importantWords = ["bovag garantie", "bovaggarantie", "bovag-garantie", "merk garantie", "merkgarantie"]
        nlp = self.models['bovagGuarantee']

        if nlp == None:
            return {'model': 'bovagGuarantee', 'WrongSentence': -1.0, 'GoodSentence': -1.0}
        
        nlp = self.sentencizer(nlp)

        result = self.NlpCheck('bovagGuarantee', nlp, PartsToCheck, importantWords)

        if result["WrongSentence"] < 0.25 and result["GoodSentence"] > 0.75:
            return result

        for package in self.advertisement["AdvertisementContent"]["Car"]["DeliveryPackages"]:
            PartsToCheck = [
                self.ReplaceCharacters(package["Name"]), self.ReplaceCharacters(package["Description"])
            ]
            packageResult = self.NlpCheck('bovagGuarantee', nlp, PartsToCheck, importantWords)
            if ( (package["Price"] == 0 or package["IsIncluded"] == True) and (packageResult["WrongSentence"] < 0.25 and packageResult["GoodSentence"] > 0.75)): 
                result = packageResult
                break
            elif (packageResult["WrongSentence"] < 0.25 and packageResult["GoodSentence"] > 0.75) and (1 - packageResult["WrongSentence"] > result["WrongSentence"]):
                result["WrongSentence"] = 1 - packageResult["WrongSentence"]
                result["GoodSentence"] = 1 - packageResult["GoodSentence"]

        return result

    def CheckExchangeGuarantee(self):
        result = {'model': 'exchangeGuarantee', 'WrongSentence': -1.0, 'GoodSentence': -1.0}  
        return result 

    def CheckAdditionalAssurance(self):
        result = {'model': 'additionalAssurance', 'WrongSentence': -1.0, 'GoodSentence': -1.0}  
        return result 