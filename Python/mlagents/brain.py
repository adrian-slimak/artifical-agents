from mlagents.communicator_objects.brain_parameters_pb2 import BrainParametersProto
from mlagents.memory import Memory
from tensorflow import reshape, transpose


class Brain:
    def __init__(self, brain_name: str, worker_id: int = 0, n_envs: int = 1):
        self.brain_name = brain_name

        self.agents_count = -1
        self.observations_vector_size = -1
        self.actions_vector_size = -1
        self.fitness_vector_size = -1
        self.stats_vector_size = -1

        self.n_envs = n_envs
        self._memory = Memory(self.brain_name, worker_id, n_envs)

    def init_from_brain(self, brain_parameters: BrainParametersProto):
        self.agents_count = brain_parameters.agents_count
        self.observations_vector_size = brain_parameters.observations_vector_size
        self.actions_vector_size = brain_parameters.actions_vector_size

        self._memory.init(self.agents_count, self.observations_vector_size, self.fitness_vector_size, self.actions_vector_size, self.stats_vector_size)

    def init_from_parameters(self, brain_parameters: dict):
        self.agents_count = int(brain_parameters[f'{self.brain_name}_count'])
        self.observations_vector_size = int(brain_parameters[f'{self.brain_name}_observations_vector_size'])
        self.actions_vector_size = int(brain_parameters[f'{self.brain_name}_actions_vector_size'])
        self.fitness_vector_size = int(brain_parameters[f'{self.brain_name}_fitness_vector_size'])
        self.stats_vector_size = int(brain_parameters[f'{self.brain_name}_stats_vector_size'])

        self._memory.init(self.agents_count, self.observations_vector_size, self.fitness_vector_size, self.actions_vector_size, self.stats_vector_size)

    def get_observations(self):
        observations = self._memory.read_observations()
        observations = reshape(observations, (self.agents_count, 1, self.observations_vector_size))
        return observations


    def get_fitness(self):
        return self._memory.read_fitness()

    def set_actions(self, agents_actions):
        self._memory.write_actions(agents_actions)

    def get_stacked_observations(self):
        observations = self._memory.read_observations()
        observations = reshape(observations, (self.n_envs, self.agents_count, self.observations_vector_size))
        observations = transpose(observations, [1, 0, 2])
        return observations

    def get_stacked_fitness(self):
        fitness = self._memory.read_fitness()
        fitness = reshape(fitness, (self.n_envs, self.agents_count))
        # fitness = transpose(fitness, [1, 0, 2])
        return fitness
