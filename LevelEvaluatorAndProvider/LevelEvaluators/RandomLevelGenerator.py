import random
from .LevelSliceClient import GetLevelSlicesForVectors


class RandomLevelGenerator():
    def __init__(self):
        self.name = "Random Level Generator"
        self.generator_model_to_use = "mariovae_z_dim_2"
    
    def GenerateRandomVectors(self, num_vectors, vector_length):
        latent_vectors = []

        for vector_i in range(num_vectors):
        
            new_vector = []
            for element_i in range(vector_length):
                new_vector.append(random.uniform(0, 10))
        
            latent_vectors.append(new_vector)
        
        return latent_vectors


    def GenerateLevel(self, request_data):
        latent_vectors = self.GenerateRandomVectors(5, 2)
        level_representation = GetLevelSlicesForVectors(latent_vectors=latent_vectors, experiment_name=request_data["experimentName"], generator_model_name=self.generator_model_to_use)
        return {
            "latentVectors":latent_vectors,
            "levelRepresentation":level_representation
        }