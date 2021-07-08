import urllib.request
import json

LEVEL_SLICE_CLIENT_URL = "https://mariovae.herokuapp.com/level?zs={latent_vectors}&experimentName={experiment_name}&modelName={generator_model_name}"

def GetLevelSlicesForVectors(latent_vectors, experiment_name, generator_model_name="mariovae_z_dim_2"):
    response = urllib.request.urlopen(LEVEL_SLICE_CLIENT_URL.format(latent_vectors=latent_vectors, experiment_name=experiment_name, generator_model_name=generator_model_name).replace(" ","%20"))
    response_json = json.loads(response.read())
    if( isinstance(response_json, dict) ):
        return response_json["levelSliceRepresentation"]
    else:
        return response_json

if __name__ == "__main__":
    TEST_VECTORS = [[3.14, 3.14]]
    print( GetLevelSlicesForVectors(latent_vectors=TEST_VECTORS, experiment_name="test-python-client"))