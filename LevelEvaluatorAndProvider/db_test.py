import json
import time
from db_interface import PlayerFeedbackTable


with open("test_data.json", "r") as f:
    test_data = json.loads(f.read()) 

test_request_data = {
        'requestId': '12368719-b0a5-46b6-9f4d-fee9bf6f6471', 
        'playerId': 'EndpointTest', 
        'telemetry': {
            "latentVectors": test_data["latentVectors"],
            "levelRepresentation":test_data["levelRepresentation"],
            "modelName": "mariovae_z_dim_2",
            "experimentName": test_data["experimentName"],
            "markedUplayable": False,
            "endedEarly": False,
            "surveyResults":{
                    "enjoyment": 0.5,
                    "ratedNovelty": 0.4,
                    "desiredNovelty": 0.2
                }
        }
}

table = PlayerFeedbackTable(None)

print( table.ParseLatentVectorsArray(test_data["latentVectors"]) )
print()

print( table.ParseLevelRepresentationArray(test_data["levelRepresentation"]) )
print()

print( table.FormQueryString(
    time.time(),
    test_request_data["playerId"],
    test_request_data["telemetry"]["experimentName"],
    test_request_data["telemetry"]["modelName"],
    test_request_data["telemetry"]["latentVectors"],
    test_request_data["telemetry"]["levelRepresentation"],
    test_request_data["telemetry"]["markedUplayable"],
    test_request_data["telemetry"]["endedEarly"],
    test_request_data["telemetry"]["surveyResults"]["enjoyment"],
    test_request_data["telemetry"]["surveyResults"]["ratedNovelty"],
    test_request_data["telemetry"]["surveyResults"]["desiredNovelty"])
    
    )
print()
