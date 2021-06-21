import pandas as pd 
import re
import json
import azure.cosmos.cosmos_client as cosmos_client
import azure.cosmos.errors as errors
import azure.cosmos.documents as documents
import azure.cosmos.http_constants as http_constants

# Initialize the Cosmos client
config = {
    "endpoint": "https://daisy-a-cosmosdb.documents.azure.com:443/",
    "primarykey": "Zo8pU3Re9NijguA0lZtNLkUOKY8e8Y8SDKugxExZxFwwtsDGdMAwiVdOJVDlj0W1MArZ77fU6UgRMJP498og0w=="
}

# Create the cosmos client
client = cosmos_client.CosmosClient(config["endpoint"], {"masterKey":config["primarykey"]})

database_id = 'jhkfAA=='
container_id = 'jhkfANsiHNc='
#container = client.ReadContainer("dbs/" + database_id + "/colls/" + container_id)

advertisements_to_check = []
for item in client.QueryItems("dbs/" + database_id + "/colls/" + container_id,
                              #'SELECT * FROM r WHERE r.id="bdfdf11e-333b-4008-bad2-c07b7ec5be8a"',
                              'SELECT * FROM r WHERE r.PartitionKey="ace9fddb-6648-4764-8054-0c181df4cf6c"',
                              #'SELECT top 10 * FROM r ',
                              {'enableCrossPartitionQuery': True}):
    advertisements_to_check.append(item)
    #print(json.dumps(item, indent=True))

print ("breakpoint")