import os
import json
import time

from flask import Flask, jsonify, request
from flask_cors import CORS
import psycopg2
from db_interface import PlayerFeedbackTable

from LevelEvaluators.RandomLevelGenerator import RandomLevelGenerator as LevelGenerator


app = Flask(__name__)
CORS(app)
level_generator = LevelGenerator()


try:
    from dotenv import load_dotenv

    load_dotenv()
except ModuleNotFoundError:
    print("Couldn't load the local .env file.")

db_url = os.environ["DATABASE_URL"]

#Create the table

db = psycopg2.connect(db_url)
feedback_table_db = PlayerFeedbackTable(db)
feedback_table_db.CreateTable()
db.close()


DEFAULT_LEVEL_DATA = []


REQUEST_FORMAT = """{
        "requestId":STRING,
        "playerId":STRING,
        "telemetry":{ // Empty for a new player
            "latentVectors": [S x [ N dimenstional vector ]],
            "levelRepresentation":[
                S x level slice representations of the form:
                [ H x W array of INTs represetning block types ],
                ...
            ],
            "modelName": STRING,
            "experimentName": STRING,
            "markedUnplayable": BOOL,
            "endedEarly": BOOL,
            "surveyResults":{
                "enjoyment": INT,
                "ratedNovelty": INT,
                "desiredNovelty": INT
            }
            ...
        }
    }"""

RESPONSE_FORMAT = """
    {
        "requestId":STRING,
        "latentVector":[ N dimenstional vector ],
        "modelName": STRING
        "experimentName":STRING,
        "levelRepresentation":[
            S x level slice representations of the form:
            [ H x W array of INTs represetning block types ],
            ...
        ]
    }
"""


def GetPlayerPreferenceFromPlayerTelemetry(player_request_data):
    """
    Simple approach to the 'Player Actions to Player Preferences' model.

    For now, ignores any telemtry data and just converts the survey data to floats between 0 and 1 if the survey was completed.
    """
    
    marked_unplayable = "markedUnplayable" in player_request_data["telemetry"] and player_request_data["telemetry"]["markedUnplayable"]
    eneded_early = "endedEarly" in player_request_data["telemetry"] and player_request_data["telemetry"]["endedEarly"]

    player_preferences = {}

    if(not (marked_unplayable or eneded_early) ):
        if("enjoyment" in player_request_data["telemetry"]):
            player_preferences["enjoyment"] = (player_request_data["telemetry"]["enjoyment"] - 2.5) / 2.5
        
        if("desiredNovelty" in player_request_data["telemetry"]):
            player_preferences["desiredNovelty"] = (player_request_data["telemetry"]["desiredNovelty"] - 2.5) / 2.5
        
    return player_preferences


def GetSimulatedPlayerTelemetry(request_data, proposed_level_data):
    MOCK_TIME_TAKEN_PER_SLICE = [15.0 / len(proposed_level_data["levelRepresentation"])] * len(proposed_level_data["levelRepresentation"])
    MOCK_RESPONSE = {
        "requestId":request_data["requestId"],
        "experimentName":request_data["experimentName"],
        "levelRepresentation":proposed_level_data["levelRepresentation"],
        "playthroughTelemetry":{
            "didComplete":True,
            "timeTaken":15.0,
            "timeSpentInEachSlice": MOCK_TIME_TAKEN_PER_SLICE
        }
    }


    simulated_player_request = {
        "requestId":request_data["requestId"],
        "experimentName":request_data["experimentName"],
        "levelRepresentation":proposed_level_data["levelRepresentation"]
    },

    simulated_player_telemetry = MOCK_RESPONSE

    return simulated_player_telemetry


def IsLevelValid(request_data, proposed_level_data):
    simulated_player_telemetry = GetSimulatedPlayerTelemetry(request_data, proposed_level_data)
    return simulated_player_telemetry["playthroughTelemetry"]["didComplete"]


def StoreTelemetryInDatabase(request_data):
    if("telemetry" in request_data and len(request_data["telemetry"].keys()) > 0):
        db = psycopg2.connect(db_url)
        feedback_table_db = PlayerFeedbackTable(db)
        feedback_table_db.SaveFeedback(
        time.time(),
        request_data["playerId"],
        request_data["telemetry"]["experimentName"],
        request_data["telemetry"]["modelName"],
        request_data["telemetry"]["latentVectors"],
        request_data["telemetry"]["levelRepresentation"],
        request_data["telemetry"]["markedUnplayable"],
        request_data["telemetry"]["endedEarly"],
        request_data["telemetry"]["surveyResults"]["enjoyment"],
        request_data["telemetry"]["surveyResults"]["ratedNovelty"],
        request_data["telemetry"]["surveyResults"]["desiredNovelty"])


def GetExperimentName():
    return level_generator.name


@app.route("/")
def landing():
    return f"""
    <h1>Level Evaluator and Providor</h1>
    <h2>Providor Name: {level_generator.name}</h2>

    <h3>/level</h3>
    <h4> Request Format</h4>
    <pre>{REQUEST_FORMAT}</pre>
    
    <h4> Response Format</h4>
    <pre>{RESPONSE_FORMAT}</pre>
    """


@app.route("/feedback")
def feedback():
    db = psycopg2.connect(db_url)
    feedback_table_db = PlayerFeedbackTable(db)
    feedbackItems = feedback_table_db.GetAllItems()
    return feedbackItems


@app.route("/level", methods=["GET", "POST"])
def level():
    if request.method == "GET":
        data = request.args
    else:
        data_request = request.get_json()
        if not isinstance(data_request, dict):
            data = request.form.to_dict()
            telemetry_string = '{ "telemetry":' +data["telemetry"].replace("'", "\"").replace("True", "true").replace("False", "false") + "}"
            data["telemetry"] = json.loads(telemetry_string)["telemetry"]
        else:
            data = data_request
    
    data["experimentName"] = GetExperimentName()


    # (2) Translate Player Telemetry Data to Player Preferences
    data["playerPreferences"] = GetPlayerPreferenceFromPlayerTelemetry(player_request_data=data)

    StoreTelemetryInDatabase(request_data=data)
        

    # (3) Generate a level candidate
    proposed_level_data = level_generator.GenerateLevel(request_data=data)


    # (4) Check level is valid
    retry_threshold = 50
    retry_count = 0
    level_valid = IsLevelValid(data, proposed_level_data)

    while not level_valid:
        retry_count += 1
        
        proposed_level_data = level_generator.GenerateLevel(request_data=data)
        level_valid = IsLevelValid(data, proposed_level_data)
        
        if(retry_count >= retry_threshold):
            break
    
    if(not level_valid):
        proposed_level_data = DEFAULT_LEVEL_DATA


    # Form Response and Send
    response = {
        "requestId":data["requestId"],
        "latentVectors": proposed_level_data["latentVectors"],
        "experimentName":data["experimentName"],
        "levelRepresentation":proposed_level_data["levelRepresentation"]
    }

    return jsonify(response)


if __name__ == "__main__":
    print("Starting Level Producer")
    app.run()
