import numpy as np
import copy

class NoveltyEvaluator:
    def __init__(self, distance_metric, novelty_threshold = 30., novelty_floor = .25, min_novelty_archive_size = 1, k_neighbors = 20, max_novelty_archive_size = None, max_iter = 100):
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
        artifacts_copy = copy.deepcopy(artifacts)
        for i in range(len(artifacts)):
            # print(artifacts[i].level_representation)
            artifacts[i].fitness_metric = self._average_knn_distance(artifacts[i], artifacts_copy)
            if(artifacts[i].fitness_metric > self.novelty_threshold or len(self.novelty_archive) < self.min_novelty_archive_size):
                self.items_added_in_generation+=1
                self.novelty_archive += [artifacts[i]]
                if not self.max_novelty_archive_size is None and len(self.novelty_archive) > self.max_novelty_archive_size:
                    self.items_added_in_generation+=1
        self.adjust_archive_settings()
        return artifacts
    
    def adjust_archive_settings(self):
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
        # print(distances)
        if len(distances) < self.k_neighbors:
            average_knn_dist = np.average(distances)
        else:
            average_knn_dist = np.average(distances[0: self.k_neighbors])
        return average_knn_dist



