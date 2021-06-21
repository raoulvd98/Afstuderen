import pandas as pd 
import re
import json
import azure.cosmos.cosmos_client as cosmos_client
import azure.cosmos.errors as errors
import azure.cosmos.documents as documents
import azure.cosmos.http_constants as http_constants

from Pipeline import *

class _CosmosDB_advertisements:
    __endpoint = "https://daisy-a-cosmosdb.documents.azure.com:443/"
    __primaryKey = "Zo8pU3Re9NijguA0lZtNLkUOKY8e8Y8SDKugxExZxFwwtsDGdMAwiVdOJVDlj0W1MArZ77fU6UgRMJP498og0w=="
    __databaseId = 'jhkfAA=='
    __containerId = 'jhkfANsiHNc='
    __client = ''

    def __setup_client(self):
        self.__client = cosmos_client.CosmosClient( self._CosmosDB_advertisements__endpoint , {"masterKey": self._CosmosDB_advertisements__primaryKey})

    # Query Examples
    #'SELECT * FROM r WHERE r.id="bdfdf11e-333b-4008-bad2-c07b7ec5be8a"',
    #'SELECT * FROM r WHERE r.PartitionKey="ace9fddb-6648-4764-8054-0c181df4cf6c"',
    #'SELECT top 10 * FROM r ',
    def __query_items(self, query):
        query_results = []
        locationDatabase = "dbs/" + self._CosmosDB_advertisements__databaseId + "/colls/" + self._CosmosDB_advertisements__containerId

        for result in self._CosmosDB_advertisements__client.QueryItems(
                locationDatabase,
                query,
                {'enableCrossPartitionQuery': True}):
            query_results.append(result)
        
        return query_results

class Advertisements:
    loaded_advertisements = ""

    def __init__(self):
        self.Pipeline = Pipeline()

    def load_advertisements_to_check(self):
        DB_connection = _CosmosDB_advertisements()
        DB_connection._CosmosDB_advertisements__setup_client()
        self.loaded_advertisements = DB_connection._CosmosDB_advertisements__query_items('SELECT top 10 * FROM r ')

    def check_all_advertisement(self):
        if self.loaded_advertisements == "":
            self.load_advertisements_to_check()

        for ad in self.loaded_advertisements:
            if ad["DocumentType"] != "AdvertismentVersion":
                continue

        x = 5
        print(x)

