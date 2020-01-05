import logging

from mlagents.envs.communicator_objects.brain_parameters_pb2 import BrainParametersProto

logger = logging.getLogger("mlagents.envs")
class BrainParameters:
    def __init__(self, brain: BrainParametersProto):
        self.brain_name = brain.brain_name
        self.agents_count = brain.agents_count
        self.observation_vector_size = brain.observations_vector_size
        self.action_vector_size = brain.actions_vector_size
        self.mmf_offset_observations = brain.mmf_offset_observations
        self.mmf_size_observations = brain.mmf_size_observations
        self.mmf_offset_actions = brain.mmf_offset_actions
        self.mmf_size_actions = brain.mmf_size_actions
