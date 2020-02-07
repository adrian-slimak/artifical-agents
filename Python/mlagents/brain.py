import logging

from mlagents.communicator_objects.brain_parameters_pb2 import BrainParametersProto
from mlagents.memory import Memory

logger = logging.getLogger("mlagents.envs")


class Brain:
    def __init__(self, brain_prameters: BrainParametersProto, worker_id: int):
        self.brain_name = brain_prameters.brain_name
        self.agents_count = brain_prameters.agents_count
        self.observation_vector_size = brain_prameters.observations_vector_size
        self.action_vector_size = brain_prameters.actions_vector_size

        self._memory = Memory(self.brain_name, worker_id, self)
        self._memory.Init(self.agents_count, self.observation_vector_size, self.action_vector_size)

    def Init(self, brain_prameters: BrainParametersProto):
        self.agents_count = brain_prameters.agents_count
        self.observation_vector_size = brain_prameters.observations_vector_size
        self.action_vector_size = brain_prameters.actions_vector_size

        self._memory.Init(self.agents_count, self.observation_vector_size, self.action_vector_size)

    def getAgentsObservations(self):
        return self._memory.ReadAgentsObservations()

    def getAgentsFitness(self):
        return self._memory.ReadAgentsFitness()

    def setAgentsActions(self, agents_actions):
        self._memory.WriteAgentsActions(agents_actions)

