import logging
from typing import NamedTuple, List, Any
from mlagents.environment import UnityEnvironment
from mlagents.exception import UnityCommunicationException, UnityTimeOutException
from multiprocessing import Process, Pipe
from multiprocessing.connection import Connection
from utils import get_initialization_input, stick_reset_parameters
import configs.custom_reset_parameters as custom_params
from .brain import Brain
# multiprocessing.set_start_method('forkserver')

logger = logging.getLogger("mlagents.envs")

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

class MultiEnvManager:
    def __init__(self, n_env: int = 1):
        self.environments = []
        self.n_env = n_env

        for worker_id in range(n_env):
            self.environments.append(self.create_environment(worker_id))

        self.external_brains = {}
        self.init_brains()

    def init_brains(self):
        params = custom_params.custom_reset_parameters_1
        params = stick_reset_parameters(params)

        for brain_name in ['prey', 'predator']:
            self.external_brains[brain_name] = Brain(brain_name, 0)
            self.external_brains[brain_name].init_stacked(params, self.n_env)

    @staticmethod
    def create_environment(worker_id: int) -> UnityEnvironment:
        return UnityEnvironment(file_name=unity_env_path, worker_id=worker_id, initialization_input=get_initialization_input(custom_params.custom_reset_parameters_1))

    def run_episode(self, models):

        for env in self.environments:
            env.reset()

        for step in range(1000):
            for env in self.environments:
                env._step_receive_observations()

            agent_observations = {brain_name: self.external_brains[brain_name].get_stacked_observations() for brain_name in ['prey']}

            agent_actions = {brain_name: models[brain_name](agent_observations[brain_name]) for brain_name in ['prey']}

            for brain_name in ['prey']:
                self.external_brains[brain_name].set_agents_actions(agent_actions[brain_name])

            for env in self.environments:
                env._step_send_actions()

        for env in self.environments:
            env._episode_completed()

        fitness = {brain_name: self.external_brains[brain_name].get_stacked_fitness() for brain_name in ['prey']}

        return fitness

    def close(self) -> None:
        logger.debug(f"MultiEnvManager closing.")
        for env in self.environments:
            env.close()
