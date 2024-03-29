from mlagents.communicator_objects.unity_initialization_input_pb2 import UnityInitializationInputProto
from mlagents.communicator_objects.unity_output_pb2 import UnityOutputProto

from mlagents.exception import (UnityCommunicationException)
from mlagents.base_environment import Environment
from mlagents.brain import Brain

from other.utils import merge_environment_parameters
from typing import Dict, Optional
import numpy as np
import logging
import atexit
from time import clock

logging.basicConfig(level=logging.INFO)
_logger = logging.getLogger("Environment")


class UnityEnvironment:
    def __init__(self, file_path: Optional[str] = None, worker_id: int = 0,
                 engine_configuration=None, environment_parameters=None,
                 timeout_wait: int = 60, batch_mode=False):

        self.worker_id = worker_id
        self.external_brains: Dict[str, Brain] = None
        atexit.register(self._close)

        self.environment = Environment(file_path, worker_id, timeout_wait, batch_mode)

        # Initialize environment
        if environment_parameters is not None:
            environment_parameters = merge_environment_parameters(environment_parameters)
            self.set_brains(environment_parameters)

        unity_output: UnityOutputProto = self.environment.initialize(
            UnityInitializationInputProto(seed=1, engine_configuration=engine_configuration, custom_reset_parameters=environment_parameters)
        )

    def run_single_episode(self, brain_models, number_of_steps, environment_parameters=None, live_plot=None):
        self.reset(environment_parameters)
        agent_actions = {}

        for current_step in range(number_of_steps):
            agent_observations = self.step_receive_observations()
            # t = clock()

            # if live_plot is not None:
            # stats = self.step_receive_stats()
            #     for brain_name, brain_stats in stats.items():
            #         live_plot.update({f'{brain_name}2': [brain_stats[0][0], brain_stats[0][1], brain_stats[0][2], brain_stats[0][3]]})

            for brain_name, brain_model in brain_models.items():
                actions = brain_model(agent_observations[brain_name])
                agent_actions[brain_name] = actions

            self.step_send_actions(agent_actions)
            # print((clock() - t) * 1000)

        if live_plot is not None:
            stats = self.step_receive_stats()
            for brain_name, brain_stats in stats.items():
                brain_stats = np.mean(brain_stats, axis=0)
                live_plot.update({f'{brain_name}3': [brain_stats[0], brain_stats[1], brain_stats[2], brain_stats[3], brain_stats[4]]})

        fitness = self.episode_completed()
        return fitness

    def reset(self, environment_parameters: Dict = None):
        if environment_parameters is not None:
            environment_parameters = merge_environment_parameters(environment_parameters)
            self.set_brains(environment_parameters)

        _logger.info(f"\n\tAcademy \"{self.environment.academy_name}_{self.worker_id}\" reset.")

        unity_output: UnityOutputProto = self.environment.reset(environment_parameters)

        if unity_output is None:
            raise UnityCommunicationException("Communicator has stopped.")

    def step_receive_observations(self):
        self.environment.status_receive_observations()

        # Read observations from memory
        state = {}
        for brain_name, brain in self.external_brains.items():
            state[brain_name] = brain.get_observations()

        return state

    def step_receive_stats(self):
        # Read statistics from memory
        stats = {}
        for brain_name, brain in self.external_brains.items():
            stats[brain_name] = brain.get_stats()

        return stats

    def step_send_actions(self, agents_actions: Dict[str, np.ndarray] = None):

        # Write actions to memory
        for brain_name, actions in agents_actions.items():
            self.external_brains[brain_name].set_actions(actions)

        self.environment.status_send_actions()

    def episode_completed(self):
        self.environment.status_episode_completed()

        # Read fitness from memory
        fitness = {}
        for brain_name, brain in self.external_brains.items():
            fitness[brain_name] = brain.get_fitness()

        return fitness

    def set_brains(self, environment_parameters: Dict):
        self.external_brains = {}

        for brain_name in ['prey', 'predator']:
            self.external_brains[brain_name] = Brain(brain_name, self.worker_id)
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
