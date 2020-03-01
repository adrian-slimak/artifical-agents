import logging

from mlagents.communicator_objects.brain_parameters_pb2 import BrainParametersProto
from mlagents.memory import Memory

logger = logging.getLogger("mlagents.envs")


class Brain:
    def __init__(self, brain_name: str, worker_id: int):
        self.brain_name = brain_name
        self._memory = Memory(self.brain_name, worker_id, self)

        self.agents_count = -1
        self.observations_vector_size = -1
        self.actions_vector_size = -1

    def init_from_brain(self, brain_parameters: BrainParametersProto):
        self.agents_count = brain_parameters.agents_count
        self.observations_vector_size = brain_parameters.observations_vector_size
        self.actions_vector_size = brain_parameters.actions_vector_size

        self._memory.Init(self.agents_count, self.observations_vector_size, self.actions_vector_size)

    def init_from_parameters(self, brain_parameters: dict):
        self.agents_count = int(brain_parameters[f'{self.brain_name}_count'])
        self.observations_vector_size = int(brain_parameters[f'{self.brain_name}_observations_vector_size'])
        self.actions_vector_size = int(brain_parameters[f'{self.brain_name}_actions_vector_size'])

        self._memory.Init(self.agents_count, self.observations_vector_size, self.actions_vector_size)

    def init_stacked(self, brain_parameters: dict, n_envs: int):
        self.agents_count = int(brain_parameters[f'{self.brain_name}_count'])
        self.observations_vector_size = int(brain_parameters[f'{self.brain_name}_observations_vector_size'])
        self.actions_vector_size = int(brain_parameters[f'{self.brain_name}_actions_vector_size'])

        self._memory.n_envs = n_envs
        self._memory.Init_stacked(self.agents_count, self.observations_vector_size, self.actions_vector_size)

    def get_agents_observations(self):
        return self._memory.read_agents_observations()

    def get_agents_fitness(self):
        return self._memory.read_agents_fitness()

    def set_agents_actions(self, agents_actions):
        self._memory.write_agents_actions(agents_actions)

    def get_stacked_observations(self):
        return self._memory.read_stacked_observations()

    def get_stacked_fitness(self):
        return self._memory.read_stacked_fitness()

