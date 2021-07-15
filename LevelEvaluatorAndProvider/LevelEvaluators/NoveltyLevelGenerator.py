import random
import os
import os.path
import numpy as np
import json
from .LevelSliceClient import GetLevelSlicesForVectors
from .Generators.Evolutionary.GeneticAlgorithm import GeneticAlgorithm
from .Generators.Evolutionary.Problems.RealProblem import MarioLevels

#TODO: Evolve offline, serve best generation online
class NoveltyLevelGenerator():
    def __init__(self):
        self.name = "Novelty-Search based Level Generator"
        self.generator_model_to_use = "mariovae_z_dim_2"
        self.primitive_analytics = dict()
        self.ga_generator =  GeneticAlgorithm(pc = 0.85, pm = 0.1, max_iter=50, elitism=0.1, selection="tournament")
        self.mario_problem = MarioLevels(60., (-10., 10.), rang_param=0.1, n_dim=10, experiment_name=self.name, generator_model_to_use=self.generator_model_to_use, analytics= self.primitive_analytics)

    def GenerateLevel(self, request_data):
        self.mario_problem.experiment_name = request_data["experimentName"]
        self.ga_generator =  GeneticAlgorithm(pc = 0.85, pm = 0.1, max_iter=50, elitism=0.1, selection="tournament")
        self.mario_problem = MarioLevels(60., (-5., 5.), rang_param=0.1, n_dim=10, experiment_name=self.name, generator_model_to_use=self.generator_model_to_use, analytics= self.primitive_analytics)
        solution = self.ga_generator.evolve(self.mario_problem, 11)
        with open('novelty_level_corpus.json', 'a') as fp:
            if os.path.isfile('novelty_level_corpus.json'):
                fp.write('\n')
            json.dump(self.primitive_analytics, fp)

        #Convert flattened latent vectors to API compliant format
        lvl_latent_vectors = []
        li = []
        list_rep = solution.latent_vector
        for i, x_i in enumerate(list_rep, start=1):
            #Convert from np.float to float
            li += [float(x_i)]
            if i % 2 == 0:
                lvl_latent_vectors.append(li)
                li = []

        #Convert matrix representing the entire level to API compliant format
        list_level_representation = []
        n = 14
        s = 5
        for slice_i in range(s):
            slice = solution.level_representation[:,slice_i*n:(slice_i + 1)*n]
            slice_list_rep = []
            for i in range(len(slice)):
                row = []
                for j in range(len(slice[i])):
                    row+=[int(slice[i][j])]
                slice_list_rep+=[row]
            list_level_representation+=[slice_list_rep]


        return {
            "latentVectors":lvl_latent_vectors,
            "levelRepresentation":list_level_representation
        }


