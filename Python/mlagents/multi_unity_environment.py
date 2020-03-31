from mlagents.communicator_objects.unity_initialization_input_pb2 import UnityInitializationInputProto
from mlagents.communicator_objects.unity_output_pb2 import UnityOutputProto

from mlagents.exception import (UnityEnvironmentException, UnityCommunicationException, UnityTimeOutException)
from mlagents.base_environment import Environment
from mlagents.brain import Brain

from utils import merge_environment_parameters
from typing import Dict, Optional
import numpy as np
import logging
import atexit

logging.basicConfig(level=logging.INFO)
_logger = logging.getLogger("Environment")


class MultiUnityEnvironment:
    def __init__(self, file_path: Optional[str] = None, number_of_environments: int = 0,
                 engine_configuration=None, environment_parameters=None,
                 timeout_wait: int = 60):

        self.number_of_environments = number_of_environments
        self.external_brains: Dict[str, Brain] = None
        atexit.register(self._close)

        self.environments = []

        # Initialize environment
        if environment_parameters is not None:
            environment_parameters = merge_environment_parameters(environment_parameters)
            self.set_brains(environment_parameters)

        initialization_input = UnityInitializationInputProto(seed=1, engine_configuration=engine_configuration, custom_reset_parameters=environment_parameters)
        unity_output: UnityOutputProto = None

        for worker_id in range(number_of_environments):
            environment = Environment(file_path, worker_id, timeout_wait)
            unity_output = environment.initialize(initialization_input)
            self.environments.append(environment)

    def run_single_episode(self, brain_models, number_of_steps, environment_parameters=None):
        self.reset(environment_parameters)

        for current_step in range(number_of_steps):
            agent_observations = self.step_receive_observations()

            agent_actions = {}
            for brain_name, brain_model in brain_models.items():
                actions = brain_model(agent_observations[brain_name])
                agent_actions[brain_name] = actions

            self.step_send_actions(agent_actions)

        fitness = self.episode_completed()
        return fitness

    def reset(self, environment_parameters: Dict = None):
        if environment_parameters is not None:
            environment_parameters = merge_environment_parameters(environment_parameters)
            self.set_brains(environment_parameters)

        _logger.info(f"Reset...")

        unity_output: UnityOutputProto = None

        for env in self.environments:
            unity_output = env.reset(environment_parameters)

        if unity_output is None:
            raise UnityCommunicationException("Communicator has stopped.")

    def step_receive_observations(self):
        for env in self.environments:
            env.status_receive_observations()

        # Read observations from memory
        observations = {}
        for brain_name, brain in self.external_brains.items():
            observations[brain_name] = brain.get_stacked_observations()

        return observations

    def step_send_actions(self, agents_actions: Dict[str, np.ndarray] = None):

        # Write actions to memory
        for brain_name, actions in agents_actions.items():
            self.external_brains[brain_name].set_actions(actions)

        for env in self.environments:
            env.status_send_actions()

    def episode_completed(self):
        for env in self.environments:
            env.status_episode_completed()

        # Read fitness from memory
        fitness = {}
        for brain_name, brain in self.external_brains.items():
            fitness[brain_name] = brain.get_stacked_fitness()

        return fitness

    def set_brains(self, environment_parameters: Dict):
        self.external_brains = {}

        for brain_name in ['prey', 'predator']:
            self.external_brains[brain_name] = Brain(brain_name, worker_id=0, n_envs=self.number_of_environments)
            self.external_brains[brain_name].init_from_parameters(environment_parameters)

    def __str__(self):
        # reset_params_str = (
        #     "\n\t\t".join(
        #         [
        #             str(k) + " -> " + str(self.default_reset_parameters[k])
        #             for k in self.default_reset_parameters
        #         ]
        #     )
        #     if self.default_reset_parameters
        #     else "{}"
        # )
        # return f"Unity Academy name: {self.academy_name}\n\tDefault Reset Parameters:\n\t\t{reset_params_str}"
        return ""

    def _close(self):
        # self.environment.close()
        pass
