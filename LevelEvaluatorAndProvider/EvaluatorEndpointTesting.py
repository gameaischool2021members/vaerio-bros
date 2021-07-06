import uuid
import urllib.request
import json


if __name__ == "__main__":
    TEST_LEVEL_REQUEST_NO_TELEMETRY = {
        "requestId":str(uuid.uuid4()),
            "playerId":"EndpointTest",
            "telemetry":{}
    }

    TEST_ENDPOINT = "http://localhost:5000/level"

    req = urllib.request.Request(TEST_ENDPOINT)
    req.add_header('Content-Type', 'application/json; charset=utf-8')
    json_data = json.dumps(TEST_LEVEL_REQUEST_NO_TELEMETRY)
    data_bytes = json_data.encode('utf-8')   # needs to be bytes
    req.add_header('Content-Length', len(data_bytes))

    with urllib.request.urlopen(req, data_bytes) as f:
        response_json = json.loads(f.read())

    print(response_json)