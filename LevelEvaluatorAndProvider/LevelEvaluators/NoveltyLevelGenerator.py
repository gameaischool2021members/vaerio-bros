import random
import numpy as np
import json
from .LevelSliceClient import GetLevelSlicesForVectors
from Generators.GALevelGenerator import GeneticAlgorithm
from .RealProblem import MarioLevels, MarioLevel

#TODO: Evolve offline, serve best generation online
class NoveltyLevelGenerator():
    def __init__(self):
        self.name = "Novelty-Search based Level Generator"
        self.generator_model_to_use = "mariovae_z_dim_2"
        self.primitive_analytics = dict()
        self.ga_generator =  GeneticAlgorithm(pc = 0.85, pm = 0.3, max_iter=50, elitism=0.1, selection="tournament", analytics = self.primitive_analytics)
        self.mario_problem = MarioLevels(60., (-5., 5.), rang_param=0.1, n_dim=10, experiment_name=self.name, generator_model_to_use=self.generator_model_to_use)
        


    def GenerateLevel(self, request_data):
        self.mario_problem.experiment_name = request_data["experimentName"]
        self.ga_generator =  GeneticAlgorithm(pc = 0.85, pm = 0.3, max_iter=50, elitism=0.1, selection="tournament", analytics = self.primitive_analytics)
        self.mario_problem = MarioLevels(60., (-5., 5.), rang_param=0.1, n_dim=10, experiment_name=self.name, generator_model_to_use=self.generator_model_to_use)
        solution = self.ga_generator.evolve(self.mario_problem, 11)
        with open('data.json', 'w') as fp:
            json.dump(self.primitive_analytics, fp)
        print(solution.latent_vector)
        print(solution.level_representation)
        return {
            "latentVectors":solution.latent_vector,
            "levelRepresentation":solution.level_representation
        }


