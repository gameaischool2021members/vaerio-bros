"""
Real number problems structure and the MarioLevels problem to be used in evolutionary algorithm approaches
Defines a problem which genotype is codified in the domain of real numbers.
Based on the BGA algorithm for continous parameter optimization (https://ieeexplore.ieee.org/document/6792992).
Author: Luis Andrés Eguiarte-Morett (Github: @leguiart)
License: MIT.
"""

import abc
import numpy as np
import math
import copy
from ....LevelSliceClient import GetLevelSlicesForVectors
from ....Evaluators.NoveltyEvaluator import NoveltyEvaluator


class _BaseRealProblem(metaclass=abc.ABCMeta):
    """
    Base class for all problems codified with real number vectors.
    ...

    Attributes
    ----------
    evaluator : Evaluator
        Object defining the way to evaluate the phenotypes
    n_dim : float
        Dimensions of the genotype vectors
    thresh : float
        Fitness function value threshold to decide a finishing criteria
    bounds : tuple (float, float)
        Restriction on the domain of the problem, equal for each dimension
    rang_param : float
        Fixed rate of the mutation range
        
    Methods
    -------
    populate(n_individuals)
        Fill out a real number matrix of dimensions n_individuals X n_dim with random values in (bounds[0], bounds[1])
        representing the population genotype pool
    stop_criteria(self, X_eval)
        Evaluates if the stop criteria for a population has been met  
    crossover(X, pc, elitism_num)
        Applies crossover operator over a real number matrix of dimensions n_individuals X n_dim
    mutate(X, pm, elitism_num)
        Applies mutation operator over a real number matrix of dimensions n_individuals X n_dim
    evaluate(X)
        Applies an evaluation function over an encoded representation of the population
        by first decoding the genotype, filling the phenotype representation and then evaluating
        that phenotype
    """
    def __init__(self, evaluator, thresh, bounds, rang_param = 0.1, n_dim = 2):
        """
        Parameters
        ----------
        evaluator : Evaluator
            Object defining the way to evaluate the phenotypes
        n_dim : int, optional
            Dimensions of the genotype vectors (default is 2)
        thresh : float
            Fitness function value threshold to decide a finishing criteria
        bounds : tuple (float, float)
            Restriction on the domain of the problem, equal for each dimension
        rang_param : float, optional
            Fixed rate of the mutation range (default is 0.1)
        """
        self.evaluator  = evaluator
        self.n_dim = n_dim
        self.thresh = thresh
        self.bounds = bounds
        self.rang_param = rang_param

    def stop_criteria(self, X_eval):
        """Evaluates if the stop criteria for a population has been met  
        Parameters
        ----------
        X_eval : list
            List of evaluated individuals representing the population of proposed solutions
        """
        return list(np.where(X_eval >= self.thresh)[0])

    def populate(self, n_individuals):
        """Fill out a real number matrix of dimensions n_individuals X n_dim with random values in (bounds[0], bounds[1])
        representing the population genotype pool
        n_individuals : int
            Size of the population
        """
        return np.random.uniform(self.bounds[0], self.bounds[1], size = (n_individuals, self.n_dim))

    @abc.abstractmethod
    def _decode(self, X_encoded):
        pass
    
    @abc.abstractmethod
    def _gather_analytics(self, X_eval):
        pass

    def _get_crossover_probs(self, n_cross):
        return np.random.rand(1 , n_cross)[0,:]

    def _get_crossover_points(self, length):
        return np.random.uniform(low = -.25 , high = 1.25, size = length)

    def crossover(self, X, pc, elitism_num):
        """Applies crossover operator over a real number matrix of dimensions n_individuals X n_dim
        as described here: https://ieeexplore.ieee.org/document/6792992
        Parameters
        ----------
        X : np.array
            Genotype matrix of dimensions n_individuals X n_dim
        pc : float
            Crossover probability
        elitism_num : int
            Number of individuals from the last rows to be kept without modification
        """
        n_cross = (X.shape[0] - elitism_num) // 2
        prob_cross = self._get_crossover_probs(n_cross)
        #Extended intermediate recombination
        for i, p in enumerate(prob_cross):
            if p <= pc:
                alphas = self._get_crossover_points(X.shape[1])
                X[2*i,:] += alphas * (X[2*i + 1, :] - X[2*i,:])
                X[2*i + 1,:] += alphas * (X[2*i,:] - X[2*i + 1, :])
                X[2*i,:] = np.clip(X[2*i,:], self.bounds[0], self.bounds[1])
                X[2*i + 1,:] = np.clip(X[2*i + 1,:], self.bounds[0], self.bounds[1])
        return X

    def _get_mutation(self, shape):
        return np.random.uniform(size = shape)
    
    def mutate(self, X, pm, elitism_num):
        """Applies mutation operator over a real number matrix of dimensions n_individuals X n_dim
        as described here: https://ieeexplore.ieee.org/document/6792992
        Parameters
        ----------
        X : np.array
            Genotype matrix of dimensions n_individuals X n_dim
        pm : float
            Mutation probability
        elitism_num : int
            Number of individuals from the last rows to be kept without modification
        """
        rang = (self.bounds[1] - self.bounds[0])*self.rang_param
        mutate_m = self._get_mutation((X.shape[0], X.shape[1]))
        
        mutate_plus_minus = self._get_mutation((X.shape[0], X.shape[1]))

        mutate_m[mutate_m <= pm] = 1.
        mutate_m[mutate_m < 1.] = 0.
        mutate_plus_minus[mutate_plus_minus <= .5] = 1.0
        mutate_plus_minus[mutate_plus_minus > .5] = -1.0

        for i in range(X.shape[0] - elitism_num):
            mutate_delta = self._get_mutation((X.shape[1], X.shape[1]))
            mutate_delta[mutate_delta <= 1./self.n_dim] = 1.
            mutate_delta[mutate_delta < 1.] = 0.
            deltas = (mutate_delta @ (2**-np.arange(self.n_dim, dtype = np.float64)[:, np.newaxis])).T
            X[i, :] = X[i, :] + mutate_m[i, :] * mutate_plus_minus[i, :] * rang * deltas
            X[i, :] = np.clip(X[i, :], self.bounds[0], self.bounds[1])
        return X
    
    def evaluate(self, X):
        """Applies an evaluation function over an encoded representation of the population
        by first decoding the genotype, filling the phenotype representation and then evaluating
        that phenotype
        Parameters
        ----------
        X : list
            List of individuals containing their respective genotype
        """
        X_decoded = self._decode(X)
        X_eval = self.evaluator.evaluate(X_decoded)
        self._gather_analytics(X_eval)
        return X_eval

class MarioLevel:
    def __init__(self):
        self.latent_vector = []
        self.level_representation = []
        self.fitness_metric = 0

    def copy(self):
        cpy = MarioLevel()
        cpy.latent_vector = self.latent_vector.copy()
        cpy.level_representation = self.level_representation.copy()
        cpy.fitness_metric = self.fitness_metric


class MarioLevels(_BaseRealProblem):
    def __init__(self, thresh, bounds, rang_param, n_dim, experiment_name, generator_model_to_use):
        self.evaluator = NoveltyEvaluator(self._simple_edit_distance, novelty_threshold=.85)
        super().__init__(self.evaluator, thresh, bounds, rang_param=rang_param, n_dim=n_dim)  
        self.experiment_name = experiment_name
        self.generator_model_to_use = generator_model_to_use
        self.analytics = dict()
        self.analytics["latent_vectors"] = []
        self.analytics["level_representations"] = []
        self.analytics["fitness"] = []
        self.analytics["novelty_archive"] = []
        

    def _simple_edit_distance(self, X, Y):
        lvlX = X.level_representation
        lvlY = Y.level_representation
        total_phenotypic_features = lvlX.shape[0]*lvlX.shape[1]
        bools = lvlX == lvlY
        return np.sum(bools)/float(total_phenotypic_features)

    def populate(self, n_individuals):
        latent_vectors = super().populate(n_individuals)
        levels = []
        for i in range(len(latent_vectors)):
            level = MarioLevel()
            level.latent_vector = latent_vectors[i].copy()
            level.level_representation = np.empty((14, 14))
            levels+=[level]
        return levels
        
    def crossover(self, X, pc, elitism_num):
        new_X = self._to_matrix(X)
        new_X = super().crossover(new_X, pc, elitism_num)
        return self._to_levels(X, new_X)

    def mutate(self, X, pm, elitism_num):
        new_X = self._to_matrix(X)
        super().mutate(new_X, pm, elitism_num)
        return self._to_levels(X, new_X)

    def _to_matrix(self, X):
        new_X = np.array(X[0].latent_vector)
        for i in range(1, len(X)):
            new_X = np.concatenate((new_X, np.array(X[i].latent_vector)))
        new_X = new_X.reshape((len(X), len(X[0].latent_vector)))
        return new_X

    def _to_levels(self, X, X_mat):
        for i in range(len(X)):
            X[i].latent_vector = list(X_mat[i,:].copy())
        return X

    def _decode(self, X_encoded):
        X_decoded = []
        list_of_latent_vectors = []
        for x in X_encoded:
            lvl_latent_vectors = []
            li = []
            for i, x_i in enumerate(x.latent_vector, start=1):
                li += [x_i.copy()]
                if i % 2 == 0:
                    lvl_latent_vectors.append(li)
                    li = []
            list_of_latent_vectors.append(lvl_latent_vectors)

        for i, latent_vectors in enumerate(list_of_latent_vectors):    
            level_slices = GetLevelSlicesForVectors(latent_vectors=latent_vectors, experiment_name=self.experiment_name, generator_model_name=self.generator_model_to_use)
            level_matrix = np.array(level_slices[0])
            for j in range(1, len(level_slices)):
                level_matrix = np.concatenate((level_matrix, np.array(level_slices[j])), axis = 1)
            newMarioLevel = MarioLevel()
            newMarioLevel.fitness_metric = X_encoded[i].fitness_metric            
            newMarioLevel.latent_vector = X_encoded[i].latent_vector.copy()
            newMarioLevel.level_representation = level_matrix.copy()
            X_decoded += [newMarioLevel]

        return X_decoded

    def _gather_analytics(self, X_eval):
        list_of_latent_vectors = []
        list_of_levels = []
        X_eval.sort(key = lambda x : x.fitness_metric)
        fitness_metrics = [individual.fitness_metric for individual in X_eval]
        for x in X_eval:
            lvl_latent_vectors = []
            li = []
            for i, x_i in enumerate(x.latent_vector, start=1):
                li += [x_i]
                if i % 2 == 0:
                    lvl_latent_vectors.append(li)
                    li = []
            list_of_latent_vectors.append(lvl_latent_vectors)

            list_level_representation = []
            n = 14
            s = 5
            for slice_i in range(s):
                slice = x.level_representation[:,slice_i*n:(slice_i + 1)*n]
                slice_list_rep = []
                for i in range(len(slice)):
                    row = []
                    for j in range(len(slice[i])):
                        row+=[int(slice[i][j])]
                    slice_list_rep+=[row]
                list_level_representation+=[slice_list_rep]
            list_of_levels.append(list_level_representation)
        
        self.analytics["latent_vectors"] += [list_of_latent_vectors]
        self.analytics["level_representations"] += [list_of_levels]
        self.analytics["fitness"]+=[fitness_metrics]
        list_of_latent_vectors_archive = []
        list_of_levels_archive = []
        fitness_metrics_archive = [individual.fitness_metric for individual in self.evaluator.novelty_archive]
        for x in self.evaluator.novelty_archive:
            lvl_latent_vectors = []
            li = []
            for i, x_i in enumerate(x.latent_vector, start=1):
                li += [x_i]
                if i % 2 == 0:
                    lvl_latent_vectors.append(li)
                    li = []
            list_of_latent_vectors_archive.append(lvl_latent_vectors)

            list_level_representation = []
            n = 14
            s = 5
            for slice_i in range(s):
                slice = x.level_representation[:,slice_i*n:(slice_i + 1)*n]
                slice_list_rep = []
                for i in range(len(slice)):
                    row = []
                    for j in range(len(slice[i])):
                        row+=[int(slice[i][j])]
                    slice_list_rep+=[row]
                list_level_representation+=[slice_list_rep]
            list_of_levels_archive.append(list_level_representation)
        novelty_archive_dict = {"latent_vectors":list_of_latent_vectors_archive, 
                                "level_representations":list_of_levels_archive,
                                "fitness":fitness_metrics_archive}
        self.analytics["novelty_archive"]+=[novelty_archive_dict]



        


