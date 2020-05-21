import numpy as np


class PreyAgents:
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
        pass