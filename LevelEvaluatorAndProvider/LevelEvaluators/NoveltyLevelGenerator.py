import random
import os
import os.path
import threading
import time
import math
import numpy as np
import json
from requests.api import request
from .LevelSliceClient import GetLevelSlicesForVectors
from .Generators.Evolutionary.GeneticAlgorithm import GeneticAlgorithm
from .Generators.Evolutionary.Problems.RealProblem import MarioLevels, MarioLevel

MAX_MODEL_QUEUE_SIZE = 5
WATCH_INTERVAL = 1

#TODO: Better way to use the telemetry data, 
class NoveltyLevelGenerator():
    def __init__(self):
        self.name = "Novelty-Search based Level Generator"
        self.generator_model_to_use = "mariovae_z_dim_2"
        self.ga_generator =  GeneticAlgorithm(pc = 0.85, pm = 0.1, max_iter=50, elitism=0.1, selection="tournament")
        self.model_queue = []
        self.reused_models_list = []
        self.player_to_model_mapping = {}
        self._recover_models()
        threading.Thread(target=self._queue_watcher, daemon=True).start()

    def _recover_models(self):
        data = []
        #Each line in the json is an evolutionary run
        with open('novelty_level_corpus.json', 'r') as fp:
            for levels_json in fp:
                if levels_json != "\n":
                    di = json.loads(levels_json)
                    data.append(di)
        data = data[len(data) - 5 : len(data)]
        for d in data:
            self._insert_to_queue(d)
    
    def _insert_to_queue(self, d):
        # last_generation_latents = d["latent_vectors"][-1]
        # last_generation_level_reps = d["level_representations"][-1]
        # last_generation_fitness = d["fitness"][-1]
        last_generation_novelty_archive_latents = d["novelty_archive"][-1]["latent_vectors"]
        last_generation_novelty_archive_level_reps = d["novelty_archive"][-1]["level_representations"]
        last_generation_novelty_archive_fitness = d["novelty_archive"][-1]["fitness"]
        latents = [last_generation_novelty_archive_latents[0]]
        indexes = [0]
        levels = []
        #Remove duplicates in the novelty archive
        for j in range(1, len(last_generation_novelty_archive_latents)):
            #if last_generation_novelty_archive_latents[j] not in last_generation_latents:
            if last_generation_novelty_archive_latents[j] not in latents:
                latents += [last_generation_novelty_archive_latents[j]]
                indexes += [j]

        for index in indexes:
            level = MarioLevel()
            level.latent_vector = last_generation_novelty_archive_latents[index]
            level.level_representation = last_generation_novelty_archive_level_reps[index]
            level.fitness_metric = last_generation_novelty_archive_fitness[index]
            levels += [level]
        #Add last generation levels
        # for j in range(len(last_generation_latents)):
        #     level = MarioLevel()
        #     level.latent_vector = last_generation_latents[j]
        #     level.level_representation = last_generation_level_reps[j]
        #     level.fitness_metric = last_generation_fitness[j]
        #     levels += [level]

        levels.sort(key = lambda x : x.fitness_metric)
        self.model_queue.append(levels)


    def _queue_watcher(self):
        while True:
            if len(self.model_queue) < MAX_MODEL_QUEUE_SIZE:
                self._evolve_levels()
            time.sleep(WATCH_INTERVAL)

    def _evolve_levels(self):      
        mario_problem = MarioLevels(60., (-5., 5.), rang_param=0.1, n_dim=10, experiment_name=self.name, generator_model_to_use=self.generator_model_to_use)
        self.ga_generator.evolve(mario_problem, 11)
        self._insert_to_queue(mario_problem.analytics)

        # Dump analytics in a json in order to extract insights from it offline
        if os.path.isfile('novelty_level_corpus.json'):
            with open('novelty_level_corpus.json', 'a') as fp:           
                fp.write('\n')
                json.dump(mario_problem.analytics, fp)
        else:
            with open('novelty_level_corpus.json', 'w') as fp: 
                json.dump(mario_problem.analytics, fp)


    def GenerateLevel(self, request_data):
        print(request_data)
        player_id = request_data["playerId"]
        index = 0
        #   If player doesn't have a model already assigned:
        if player_id not in self.player_to_model_mapping.keys():
            #   Try and get the first one from the reused models list
            #   If there is no first one from the reused models list:
            if len(self.reused_models_list) == 0:
                #   Dequeue one from the model queue, place it in the reused models list and assign it to the player
                new_model = self.model_queue.pop(0)
                self.reused_models_list.append(new_model.copy())
            next_model = self.reused_models_list[0]
            self.player_to_model_mapping[player_id] = [0, next_model.copy()]
        #   Otherwise
        else:
            #   If currently assigned model has no levels left:
            if len(self.player_to_model_mapping[player_id][1]) == 0:
                #   Try and get next model from the reused models list
                #   If there is no next model on the reused models list:
                if self.player_to_model_mapping[player_id][0] + 1 == len(self.reused_models_list):
                    #   Dequeue one from the model queue, place it in the reused models list and assign it to the player
                    new_model = self.model_queue.pop(0)
                    self.reused_models_list.append(new_model.copy())
                    self.player_to_model_mapping[player_id] = [self.player_to_model_mapping[player_id][0] + 1, new_model.copy()]
                #   Otherwise
                else:
                    #   Get next model from the reused models list
                    next_model = self.reused_models_list[self.player_to_model_mapping[player_id][0] + 1]
                    self.player_to_model_mapping[player_id] = [self.player_to_model_mapping[player_id][0] + 1, next_model.copy()]

            #   For now just use the desiredNovelty field to decide the next level index
            #   TODO: Find a way to incorporate the ratedNovelty and enjoyment, maybe a secondary evolutionary algorithm? MOEA?
            desired_novelty = (request_data["telemetry"]["surveyResults"]["desiredNovelty"] - 1) / 4
            index = math.floor(desired_novelty*(len(self.player_to_model_mapping[player_id][1]) - 1))
        #   Serve next level
        level_to_serve = self.player_to_model_mapping[player_id][1].pop(index)

        return {
            "latentVectors":level_to_serve.latent_vector,
            "levelRepresentation":level_to_serve.level_representation
        }


