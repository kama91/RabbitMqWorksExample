import requests
import json
import time

with open('testData.json') as file:
    body = json.load(file)
    file.close()
    
body = json.dumps(body)    
api_endpoint = 'http://localhost:5000/api/notification/events'
headers = {'content-type': 'application/json'}
start = time.perf_counter()
response = requests.post(api_endpoint, headers=headers, data=body, verify=False)
request_time = time.perf_counter() - start
if (response.status_code == 200):
    print("Request to", api_endpoint, "successfully completed in {:05.3f}s".format(request_time))
else:
    print("Request completed with status code", response.status_code)        
