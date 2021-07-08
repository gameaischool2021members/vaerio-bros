This is a basic web app that serves an API. It allows people to query for levels using either GET or POST requests.

# Getting levels

The signature for POSTing:
```
{
    "requestId": "your_request_id",
    "experimentName": "a_string",
    "modelName": "mariovae_zdim_2",
    "zs": [[0.0, 0.0], [0.1, -0.6]]
}
```
we might add more possible `modelName`s later on.

You could also query the API using GET requests as follows:
- `/level` to get 5 random levels.
- `/level?zs=[[0.1, 0.2], [0.3, 0.5]]`, i.e. passing the `zs` you want to query.
- `/level?zs=[[0.1, 0.2]]&experimentName=my_cool_experiment`, i.e. passing the `zs` and passing the experiment name.

This query returns a JSON document with the following structure:

```
{
        "requestId":STRING,
        "experimentName":STRING,
        "modelName":STRING,
        "latentVector":[ N dimenstional vectors ],
        "levelSliceRepresentation":[ N H x W-arrays of INTs represetning block types ],
    }
```

And this is the encoding for the levels inside `levelSliceRepresentation`.

```
0 -> stone
1 -> breakable stone
2 -> empty
3 -> question mark
4 -> used question mark
5 -> goomba
6 -> left pipe head
7 -> right pipe head
8 -> left pipe
9 -> right pipe
10 -> coin
```

# Running it locally

If you want to run it locally, you will need to write a `.env` file with a `DATABASE_URL` that points towards a local postgresql. Something like

```
# this is the .env file
DATABASE_URL="postgres://localhost:5432/migd"
```

