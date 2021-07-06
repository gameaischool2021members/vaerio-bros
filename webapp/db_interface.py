from typing import List
import psycopg2


class Query:
    """
    The trials table
    """

    def __init__(self, db):
        """
        Takes a db connection.
        """
        self.table_name = "query_latent_space"
        self.db = db

    def execute_query(self, query: str):
        c = self.db.cursor()
        c.execute(query)
        self.db.commit()

    def create_query_table(self):
        query = f"CREATE TABLE IF NOT EXISTS {self.table_name} ("
        query += "id SERIAL PRIMARY KEY,"
        query += "timestamp FLOAT,"
        query += "experiment_name VARCHAR(100),"
        query += "model_name VARCHAR(100),"
        query += "z FLOAT[]"
        query += ")"
        try:
            self.execute_query(query)
        except psycopg2.errors.UniqueViolation:
            print("Table already exists?")
            pass

    def parse_latent_code(self, z: List[float]):
        """
        Returns a postgresql-friendly version
        of the latent code.
        """
        string = "{"
        for i, zi in enumerate(z):
            if i < len(z) - 1:
                string += f"{zi},"
            else:
                string += f"{zi}"
        string += "}"

        return string

    def save_query(
        self,
        timestamp: float,
        experiment_name: str,
        model_name: str,
        zs: List[List[float]],
    ):
        query = f"INSERT INTO {self.table_name} (timestamp, experiment_name,model_name,z) VALUES "
        for i, z in enumerate(zs):
            query += f" ({timestamp}, '{experiment_name}', '{model_name}', '{self.parse_latent_code(z)}')"
            if i < len(zs) - 1:
                query += ","
            else:
                query += ";"
        print(query)
        self.execute_query(query)

    def get_all_of_experiment(self, experiment_name: str):
        """
        Returns all (timestamp, experiment_name, model_name, z)
        values for a given experiment name.
        """
        query = (
            f"SELECT * FROM {self.table_name} WHERE experiment_name='{experiment_name}'"
        )
        c = self.db.cursor()
        c.execute(query)
        queries = c.fetchall()
        return queries
