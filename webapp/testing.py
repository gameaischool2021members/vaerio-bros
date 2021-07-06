import requests
import json
from pprint import pprint

data = {"zs": [[0.0, 0.0], [1.0, 1.0]]}
a = requests.post("https://mariovae.herokuapp.com/level", json=data)
print(f"Requesting: {data}")
levels = a.json()
for level in levels:
    pprint(level)
    print()
