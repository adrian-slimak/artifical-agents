import numpy as np

class PredatorAgents:
    def __init__(self, pop_size):
        self.population_size: int = pop_size
        self.alive: list = None
        self.pos_x: np.ndarray = None
        self.pos_y: np.ndarray = None
        self.angle: np.ndarray = None
        self.fitness: np.array = None

        self.observations: np.ndarray = None
        self.actions: np.ndarray = None

    def reset(self):
        self.alive = [True] * self.population_size

        world_size = 128
        self.pos_x = np.random.uniform(low=0, high=world_size, size=self.population_size)
        self.pos_y = np.random.uniform(low=0, high=world_size, size=self.population_size)

        obs_array = 15
        self.observations = np.zeros(shape=(self.population_size, obs_array))
        act_array = 15
        self.actions = np.zeros(shape=(self.population_size, act_array))
        self.fitness = np.zeros(shape=self.population_size)
