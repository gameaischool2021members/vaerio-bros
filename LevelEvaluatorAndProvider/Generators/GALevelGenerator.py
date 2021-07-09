import numpy as np
import random
import math
import copy

class GeneticAlgorithm:
    def __init__(self, pc = 0.6, pm = 0.1, max_iter = 5000, selection = "proportional", elitism = 0, tournament_type = 0, analytics = None):
        self.max_iter = max_iter
        self.pc = pc
        self.pm = pm
        #print(pm)
        self.elitism = elitism
        self.k_tournament = 2
        self.tournament_type = tournament_type
        self.selection = selection
        self.analytics = analytics
        if self.analytics is not None:
            self.analytics["latent_vectors"] = []
            self.analytics["level_representations"] = []
            self.analytics["fitness"]=[]
        
    
    def evolve(self, problem, n_individuals):
        its = 1
        self.elitismNum = math.floor(self.elitism*n_individuals)
        self.pop = problem.populate(n_individuals)
        while its <= self.max_iter:
            self.pop = self.select(problem, self.pop)
            self.pop = problem.crossover(self.pop, self.pc, self.elitism)
            self.pop = problem.mutate(self.pop, self.pm, self.elitism)
            its+=1
        self.pop = self.select(problem, self.pop)
        return self.pop[-1]


    def get_roulette_probs(self, n_individuals):
        return np.random.uniform(size = (1, n_individuals))

    def select(self, problem, population):
        population = problem.evaluate(population)
        population.sort(key = lambda x : x.fitness_metric)
        fitness_metrics = [individual.fitness_metric for individual in population]
        if self.analytics:
            list_of_latent_vectors = []
            list_of_levels = []
            for x in population:
                list_of_levels+=[list(x.level_representation)]
                lvl_latent_vectors = []
                li = []
                for i, x_i in enumerate(x.latent_vector, start=1):
                    li += [x_i]
                    if i % 2 == 0:
                        lvl_latent_vectors.append(li)
                        li = []
                list_of_latent_vectors.append(lvl_latent_vectors)
            
            
            self.analytics["latent_vectors"] += [list_of_latent_vectors]
            # self.analytics["level_representations"] += [list_of_levels]
            self.analytics["fitness"]+=[fitness_metrics]

        if self.selection == "proportional":
            prob_sel = np.array(fitness_metrics)/sum(fitness_metrics)
            c_prob_sel = np.cumsum(prob_sel)
            probs = self.get_roulette_probs(len(population) - self.elitismNum)

            for j, prob in enumerate(probs[0, :]):
                i = np.searchsorted(c_prob_sel, prob)
                population[j] = copy.deepcopy(population[i])

        elif self.selection == "tournament":
            t = 0
            #with replacement
            if self.tournament_type == 1:
                
                while t < len(population) - self.elitismNum:
                    tournament_contestants = np.random.permutation(len(population))[0:self.k_tournament]
                    greatest_score_so_far = float('-inf')
                    for contestant in tournament_contestants:
                        if population[contestant].fitness_metric > greatest_score_so_far:
                            greatest_score_so_far = population[contestant].fitness_metric
                            population[t] = copy.deepcopy(population[contestant])
                    t+=1
            #without replacement
            elif self.tournament_type == 0:
                while t < len(population) - self.elitismNum:
                    permutation = np.random.permutation(len(population))
                    i = 0
                    while i < len(permutation) and t < len(population) - self.elitismNum:
                        greatest_score_so_far = float('-inf')
                        for j in range(i,min(i + self.k_tournament, len(population))):
                            if population[permutation[j]].fitness_metric > greatest_score_so_far:
                                greatest_score_so_far = population[j].fitness_metric
                                population[t] = copy.deepcopy(population[permutation[j]])
                        t+=1
                        i+=self.k_tournament


        return population
