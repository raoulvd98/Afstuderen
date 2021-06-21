from flask import Flask, jsonify, request
from flask_restful import Resource, Api, reqparse
from MLModel import *
from Predict import *
import spacy
from spacy.pipeline import Sentencizer
from spacy.util import minibatch, compounding

models = { }
model_names = ["btw", "priceAllIn", "PriceIsSale", "vehicleType", "bovagGuarantee", "exchangeGuarantee", "additionalAssurance" ]
for model in model_names:
    try: 
        models[model] = spacy.load("C:/Users/raoul/git/repository/V1/Python/models/{}".format(model))
    except:
        models[model] = None


app = Flask(__name__)
api = Api(app)

class TrainModelController(Resource):

    def get(self, model_name):
        if model_name == "all":
            return list(models.keys())
        else:
            return 'The given model is not valid', 404
    
    def post(self, model_name):
        print (model_name)
        if model_name not in list(models.keys()):
            return 'The given model is not valid', 404
            
        ModelToTrain = MLModel( request.get_json(force=True),  model_name) 

        ModelDirection = "C:/Users/raoul/git/repository/V1/Python/models/{}".format(model_name)
        Training, nlp = ModelToTrain.TrainModel( models[model_name] , ModelDirection)
        models[model_name] = nlp

        return Training

class PredictController(Resource):

    def post(self):
        Advertisement = request.get_json(force=True)
        NlpPipeline = PredictAdvertisement(Advertisement, models)

        return NlpPipeline.Pipeline()

api.add_resource(TrainModelController, '/trainmodel/<model_name>')
api.add_resource(PredictController, '/predict')

if __name__ == "__main__":
    app.run(host='127.0.0.1', port=80, debug=True)