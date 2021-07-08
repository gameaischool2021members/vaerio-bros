import os
import json
import time

import psycopg2
import torch
from flask import Flask, render_template, jsonify, request, session

from db_interface import Query
from vae_mario import VAEMario

app = Flask(__name__)

try:
    from dotenv import load_dotenv

    load_dotenv()
except ModuleNotFoundError:
    print("Couldn't load the local .env file.")

db_url = os.environ["DATABASE_URL"]

# Create the table
db = psycopg2.connect(db_url)
query_db = Query(db)
query_db.create_query_table()
db.close()


@app.route("/")
def landing():
    return "Query a post to /level with your zs. :)"


@app.route("/level", methods=["GET", "POST"])
def level():
    if request.method == "GET":
        data = request.args
    else:
        data_request = request.get_json()
        if not isinstance(data_request, dict):
            data = json.loads(request.get_json())
        else:
            data = data_request

    request_id = data.get("requestId") or "None"
    zs = data.get("zs")
    experiment_name = data.get("experimentName") or "None"
    model_name = data.get("modelName") or "mariovae_z_dim_2"
    # model_name = "mariovae_z_dim_2"

    if isinstance(zs, str):
        zs = json.loads(zs)

    if zs is None:
        experiment_name = "random"
        zs = 3 * torch.randn((5, 2))
        og_zs = [z.tolist().copy() for z in zs]
    else:
        og_zs = [z.copy() for z in zs]

    db = psycopg2.connect(db_url)
    query_db = Query(db)
    timestamp = time.time()
    query_db.save_query(timestamp, experiment_name, model_name, zs)
    db.close()

    model = VAEMario(2)
    model.load_state_dict(torch.load(f"./models/{model_name}.pt"))
    model.eval()

    zs = torch.Tensor(zs)
    zs = zs.type(dtype=torch.float)
    levels = model.decode(zs)
    levels = torch.argmax(levels, dim=1)
    levels = levels.tolist()

    res = {
        "requestId": request_id,
        "experimentName": experiment_name,
        "modelName": model_name,
        "latentVector": og_zs,
        "levelSliceRepresentation": levels,
    }
    # res = levels

    return jsonify(res)


if __name__ == "__main__":
    print("Serving the web app")
    app.run()
