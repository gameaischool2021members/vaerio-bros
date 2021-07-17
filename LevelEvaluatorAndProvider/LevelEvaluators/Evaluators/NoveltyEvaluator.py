"""
Novelty search.
General implementation of the Novelty-Search algorithm used to give a novelty score 
as a fitness score in order to drive the selection pressure on an evolutionary algorithm.
Based on the Novelty-Search algorithm found in here: https://github.com/PacktPublishing/Hands-on-Neuroevolution-with-Python/tree/master/Chapter6
Author: Luis Andrés Eguiarte-Morett (Github: @leguiart)
License: MIT.
"""
import numpy as np
import copy

class NoveltyEvaluator:
    """
    Base class for all problems codified with real number vectors.
    ...

    Attributes
    ----------
    distance_metric : function
        Function which defines a way to measure a distance
    novelty_threshold : float
        Novelty score dynamic threshold for entrance to novelty archive
    novelty_floor : float
        Lower bound of the novelty threshold
    min_novelty_archive_size : int
        Novelty archive must have at least this number of individuals
    k_neighbors : tuple (float, float)
        K nearest neighbors to compute average distance to in order to get a novelty score
    max_novelty_archive_size : int
        Novelty archive can have at most this number of individuals

    Methods
    -------
    evaluate(artifacts)
        Evaluates the novelty of each artifact in a list of artifacts
    """
    def __init__(self, distance_metric, novelty_threshold = 30., novelty_floor = .25, min_novelty_archive_size = 1, k_neighbors = 5, max_novelty_archive_size = None, max_iter = 100):
        """
        Parameters
        ----------
        distance_metric : function
            Function which defines a way to measure a distance
        novelty_threshold : float, optional
            Novelty score dynamic threshold for entrance to novelty archive (default is 30)
        novelty_floor : float, optional
            Lower bound of the novelty threshold (default is 0.25)
        min_novelty_archive_size : int, optional (default is 1)
            Novelty archive must have at least this number of individuals
        k_neighbors : int, optional (default is 20)
            K nearest neighbors to compute average distance to in order to get a novelty score
        max_novelty_archive_size : int, optional (default is None)
            Novelty archive can have at most this number of individuals
        """        
        self.novelty_threshold = novelty_threshold
        self.novelty_floor = novelty_floor
        self.min_novelty_archive_size = min_novelty_archive_size
        self.k_neighbors = k_neighbors
        self.max_novelty_archive_size = max_novelty_archive_size
        self.max_iter = max_iter
        self.items_added_in_generation = 0
        self.time_out = 0
        self.its = 0
        self.novelty_archive = []
        self.distance_metric = distance_metric

    def evaluate(self, artifacts):
        """Evaluates the novelty of each artifact in a list of artifacts according to the Novelty-Search algorithm
        artifacts : list
            List of artifact objects which contain a fitness metric and a genotype
        """
        artifacts_copy = artifacts.copy()
        for i in range(len(artifacts)):
            artifacts[i].fitness_metric = self._average_knn_distance(artifacts[i], artifacts_copy)
            if(artifacts[i].fitness_metric > self.novelty_threshold or len(self.novelty_archive) < self.min_novelty_archive_size):
                self.items_added_in_generation+=1
                self.novelty_archive += [artifacts[i]]
                if not self.max_novelty_archive_size is None and len(self.novelty_archive) > self.max_novelty_archive_size:
                    self.novelty_archive.sort(key = lambda x : x.fitness_metric)
                    self.novelty_archive.pop(0)
        self._adjust_archive_settings()
        return artifacts
    
    def _adjust_archive_settings(self):
        if self.items_added_in_generation == 0:
            self.time_out+=1
        else:
            self.time_out = 0
        if self.time_out >= 10:
            self.novelty_threshold *= 0.95
            self.novelty_threshold = max(self.novelty_threshold, self.novelty_floor)
            self.time_out = 0
        if self.items_added_in_generation >= 4:
            self.novelty_threshold *= 1.2
        self.items_added_in_generation = 0
    
    def _average_knn_distance(self, artifact, artifacts):
        distances = []
        for a in artifacts:
            distances += [self.distance_metric(artifact, a)]
        for novel in self.novelty_archive:
            distances += [self.distance_metric(artifact,novel)]
        distances.sort()
        if len(distances) < self.k_neighbors:
            average_knn_dist = np.average(distances)
        else:
            average_knn_dist = np.average(distances[0: self.k_neighbors])
        return average_knn_dist



