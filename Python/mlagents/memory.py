import mmap
from numpy import fromstring
import numpy as np
from tensorflow import io, reshape, float32, transpose

MMF_SIZE = 250000  # number of bytes
OBSERVATIONS_SIZE = 100 * 1024
ACTIONS_SIZE = 50 * 1024
FITNES_SIZE = 50 * 1024
STATS_SIZE = 50 * 1024

class Memory:
    def __init__(self, brain_name, worker_id, brain):
        self._mmf = mmap.mmap(fileno=-1, length=MMF_SIZE, tagname=f'{brain_name}_brain')
        self.worker_id = worker_id
        self._brain = brain
        self.n_envs = -1

        self.mmf_offset_observations = -1
        self.mmf_size_observations = -1

        self.mmf_offset_actions = -1
        self.mmf_size_actions = -1

        self.mmf_offset_fitness = -1
        self.mmf_size_fitness = -1

        self.mmf_offset_stats = -1
        self.mmf_size_stats = -1

    def Init(self, agents_count, observations_vector_size, actions_vector_size, stats_vector_size):
        self.mmf_size_observations = agents_count * observations_vector_size * 4
        self.mmf_size_actions = agents_count * actions_vector_size * 4
        self.mmf_size_fitness = agents_count * 4
        self.mmf_size_stats = stats_vector_size * 4

        self.mmf_offset_observations = self.mmf_size_observations * self.worker_id
        self.mmf_offset_actions = 100000 + self.mmf_size_actions * self.worker_id
        self.mmf_offset_fitness = 150000 + self.mmf_size_fitness * self.worker_id
        self.mmf_offset_stats = 175000 + self.mmf_size_stats * self.worker_id

        # size_needed = self.mmf_size_observations+self.mmf_offset_actions+self.mmf_size_fitness
        # if MMF_SIZE < size_needed:
        #     raise Exception(f"Memory Mapped File not large enough! {MMF_SIZE}<{size_needed}")

    def Init_stacked(self, agents_count, observations_vector_size, actions_vector_size):
        self.mmf_size_observations = agents_count * observations_vector_size * 4 * self.n_envs
        self.mmf_size_actions = agents_count * actions_vector_size * 4 * self.n_envs
        self.mmf_size_fitness = agents_count * 4 * self.n_envs

        self.mmf_offset_observations = 0
        self.mmf_offset_actions = 100000
        self.mmf_offset_fitness = 150000

    def read_agents_observations(self):
        self._mmf.seek(self.mmf_offset_observations)

        observations = io.decode_raw(self._mmf.read(self.mmf_size_observations), out_type=float32)

        observations = reshape(observations, (self._brain.agents_count, 1, self._brain.observations_vector_size))

        return observations

    def read_agents_fitness(self):
        self._mmf.seek(self.mmf_offset_fitness)

        fitness = fromstring(self._mmf.read(self.mmf_size_fitness), dtype='f')

        return fitness

    def write_agents_actions(self, agents_actions):
        self._mmf.seek(self.mmf_offset_actions)
        byteArray = agents_actions.tobytes()
        self._mmf.write(byteArray)

    def read_stacked_observations(self):
        self._mmf.seek(self.mmf_offset_observations)
        observations = io.decode_raw(self._mmf.read(self.mmf_size_observations), out_type=float32)
        observations = reshape(observations, (self.n_envs, self._brain.agents_count, self._brain.observations_vector_size))
        observations = transpose(observations, [1, 0, 2])

        return observations

    def read_stacked_fitness(self):
        self._mmf.seek(self.mmf_offset_fitness)
        fitness = io.decode_raw(self._mmf.read(self.mmf_size_fitness), out_type=float32)
        fitness = reshape(fitness, (self.n_envs, self._brain.agents_count))
        # fitness = transpose(fitness, [1, 0, 2])

        return fitness
