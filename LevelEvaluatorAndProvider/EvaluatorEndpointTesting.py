import uuid
import urllib.request
from urllib import parse, request
import json


if __name__ == "__main__":
    with open("test_data.json", "r") as f:
        test_level_data = json.loads(f.read())

    TEST_LEVEL_REQUEST_NO_TELEMETRY = {
        "requestId":str(uuid.uuid4()),
        "playerId":"EndpointTest",
        "telemetry":{
            "latentVectors": test_level_data["latentVectors"],
            "levelRepresentation":test_level_data["levelRepresentation"],
            "modelName": "mariovae_z_dim_2",
            "experimentName": test_level_data["experimentName"],
            "markedUnplayable": False,
            "endedEarly": False,
            "surveyResults":{
                    "enjoyment": 0.5,
                    "ratedNovelty": 0.4,
                    "desiredNovelty": 0.2
                }
        }
    }

    TEST_ENDPOINT = "http://localhost:5000/level"
    TEST_ENDPOINT = "https://vaerio-level-providor.herokuapp.com/level"

    req = urllib.request.Request(TEST_ENDPOINT)
    req.add_header('Content-Type', 'application/json; charset=utf-8')
    json_data = json.dumps(TEST_LEVEL_REQUEST_NO_TELEMETRY)
    data_bytes = json_data.encode('utf-8')   # needs to be bytes
    req.add_header('Content-Length', len(data_bytes))

    with urllib.request.urlopen(req, data_bytes) as f:
        response_json = json.loads(f.read())

    ### AS FORM DATA
    # data = parse.urlencode(TEST_LEVEL_REQUEST_NO_TELEMETRY).encode()
    # req =  request.Request(TEST_ENDPOINT, data=data) # this will make the method "POST"
    # with request.urlopen(req) as f:
    #     response_json = json.loads(f.read())


    print(response_json)