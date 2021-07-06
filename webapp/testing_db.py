import os

import psycopg2

from db_interface import Query

try:
    from dotenv import load_dotenv

    load_dotenv()
except ModuleNotFoundError:
    print("Couldn't load the local .env file.")

db_url = os.environ["DATABASE_URL"]

db = psycopg2.connect(db_url)
query_db = Query(db)
print(query_db.get_all_of_experiment("random"))
