import numpy as np
import math
import copy
from .LevelSliceClient import GetLevelSlicesForVectors
from .Evaluators.NoveltyEvaluator import NoveltyEvaluator


class _BaseRealProblem:
    def __init__(self, thresh, bounds, rang_param = 0.1, n_dim = 2):
        self.n_dim = n_dim
        self.thresh = thresh
        self.bounds = bounds
        self.rang_param = rang_param

    def get_crossover_probs(self, n_cross):
        return np.random.rand(1 , n_cross)[0,:]
    def stop_criteria(self, X_eval):
        return list(np.where(X_eval >= self.thresh)[0])

    def populate(self, n_individuals):
        return np.random.uniform(self.bounds[0], self.bounds[1], size = (n_individuals, self.n_dim))

    def decode(self, X_encoded):
        return X_encoded

    def get_crossover_points(self, length):
        return np.random.uniform(low = -.25 , high = 1.25, size = length)

    def crossover(self, X, pc, elitism):
        if not elitism:
            n_cross = X.shape[0] // 2
            elitism_num = 0
        else:
            elitism_num = math.floor(elitism * X.shape[0])
            n_cross = (X.shape[0] - elitism_num) // 2
        prob_cross = self.get_crossover_probs(n_cross)
        for i, p in enumerate(prob_cross):
            if p <= pc:
                alphas = self.get_crossover_points(X.shape[1])
                X[2*i + elitism_num,:] += alphas * (X[2*i + 1 + elitism_num, :] - X[2*i + elitism_num,:])
                X[2*i + 1 + elitism_num,:] += alphas * (X[2*i + elitism_num,:] - X[2*i + 1 + elitism_num, :])
                X[2*i + elitism_num,:] = np.clip(X[2*i + elitism_num,:], self.bounds[0], self.bounds[1])
                X[2*i + 1 + elitism_num,:] = np.clip(X[2*i + 1 + elitism_num,:], self.bounds[0], self.bounds[1])
        return X

    def get_mutation(self, shape):
        return np.random.uniform(size = shape)
    
    def mutate(self, X, pm, elitism):
        if not elitism:
            elitism = 0

        rang = (self.bounds[1] - self.bounds[0])*self.rang_param
        mutate_m = self.get_mutation((X.shape[0], X.shape[1]))
        
        mutate_plus_minus = self.get_mutation((X.shape[0], X.shape[1]))

        mutate_m[mutate_m <= pm] = 1.
        mutate_m[mutate_m < 1.] = 0.
        mutate_plus_minus[mutate_plus_minus <= .5] = 1.0
        mutate_plus_minus[mutate_plus_minus > .5] = -1.0
        
        elitism_num = math.floor(elitism * X.shape[0])
        for i in range(elitism_num, X.shape[0]):
            mutate_delta = self.get_mutation((X.shape[1], X.shape[1]))
            mutate_delta[mutate_delta <= 1./self.n_dim] = 1.
            mutate_delta[mutate_delta < 1.] = 0.
            deltas = (mutate_delta @ (2**-np.arange(self.n_dim, dtype = np.float64)[:, np.newaxis])).T
            X[i, :] = X[i, :] + mutate_m[i, :] * mutate_plus_minus[i, :] * rang * deltas
            X[i, :] = np.clip(X[i, :], self.bounds[0], self.bounds[1])
        return X


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
        super().__init__(thresh, bounds, rang_param=rang_param, n_dim=n_dim)
        self.evaluator = NoveltyEvaluator(self.simple_edit_distance)
        self.experiment_name = experiment_name
        self.generator_model_to_use = generator_model_to_use

    def simple_edit_distance(self, X, Y):
        lvlX = X.level_representation
        lvlY = Y.level_representation
        total_phenotypic_features = lvlX.shape[0]*lvlX.shape[1]
        bools = lvlX == lvlY
        return np.sum(bools)

    def populate(self, n_individuals):
        latent_vectors = super().populate(n_individuals)
        levels = []
        for i in range(len(latent_vectors)):
            level = MarioLevel()
            level.latent_vector = latent_vectors[i]
            level.level_representation = np.empty((14, 14))
            levels+=[level]
        return levels
        
    def crossover(self, X, pc, elitism):
        new_X = self.to_matrix(X)
        new_X = super().crossover(new_X, pc, elitism)
        return self.to_levels(X, new_X)

    def mutate(self, X, pm, elitism):
        new_X = self.to_matrix(X)
        super().mutate(new_X, pm, elitism)
        return self.to_levels(X, new_X)

    def to_matrix(self, X):
        new_X = np.array(X[0].latent_vector)
        for i in range(1, len(X)):
            new_X = np.concatenate((new_X, np.array(X[i].latent_vector)))
        new_X = new_X.reshape((len(X), len(X[0].latent_vector)))
        return new_X

    def to_levels(self, X, X_mat):
        for i in range(len(X)):
            X[i].latent_vector = list(X_mat[i,:].copy())
        return X

    def decode(self, X_encoded):
        X_decoded = copy.deepcopy(X_encoded)
        list_of_latent_vectors = []
        for x in X_decoded:
            lvl_latent_vectors = []
            li = []
            for i, x_i in enumerate(x.latent_vector, start=1):
                li += [x_i]
                if i % 2 == 0:
                    lvl_latent_vectors.append(li)
                    li = []
            list_of_latent_vectors.append(lvl_latent_vectors)
        
        for i, latent_vectors in enumerate(list_of_latent_vectors):    
            level_slices = GetLevelSlicesForVectors(latent_vectors=latent_vectors, experiment_name=self.experiment_name, generator_model_name=self.generator_model_to_use)
            level_matrix = np.array(level_slices[0])
            for i in range(1, len(level_slices)):
                level_matrix = np.concatenate((level_matrix, np.array(level_slices[i])), axis = 1)
            X_decoded[i].level_representation = level_matrix
        return X_decoded


    def evaluate(self, X):
        X_decoded = self.decode(X)
        return self.evaluator.evaluate(X_decoded)
