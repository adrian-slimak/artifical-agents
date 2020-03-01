import logging
from typing import NamedTuple, List, Any
from mlagents.environment import UnityEnvironment
from mlagents.exception import UnityCommunicationException, UnityTimeOutException
from multiprocessing import Process, Pipe
from multiprocessing.connection import Connection
from utils import get_initialization_input, get_reset_parameters
import configs.custom_reset_parameters as custom_params
from .memory import Memory
from .brain import Brain
import multiprocessing
# multiprocessing.set_start_method('forkserver')

logger = logging.getLogger("mlagents.envs")

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"


class EnvironmentCommand(NamedTuple):
    name: str
    payload: Any = None


class EnvironmentResponse(NamedTuple):
    name: str
    worker_id: int
    payload: Any


class UnityEnvWorker:
    def __init__(self, process: Process, worker_id: int, conn: Connection):
        self.process = process
        self.worker_id = worker_id
        self.conn = conn

    def send(self, name: str, payload: Any = None) -> None:
        try:
            cmd = EnvironmentCommand(name, payload)
            self.conn.send(cmd)
        except (BrokenPipeError, EOFError):
            raise UnityCommunicationException("UnityEnvironment worker: send failed.")

    def recv(self) -> EnvironmentResponse:
        try:
            response: EnvironmentResponse = self.conn.recv()
            return response
        except (BrokenPipeError, EOFError):
            raise UnityCommunicationException("UnityEnvironment worker: recv failed.")

    def close(self):
        try:
            self.conn.send(EnvironmentCommand("close"))
        except (BrokenPipeError, EOFError):
            logger.debug(f"UnityEnvWorker {self.worker_id} got exception trying to close.")
        logger.debug(f"UnityEnvWorker {self.worker_id} joining process.")
        self.process.join()

def worker(parent_conn: Connection, worker_id: int) -> None:
    unity_environment: UnityEnvironment = UnityEnvironment(file_name=unity_env_path, worker_id=worker_id, initialization_input=get_initialization_input(custom_params.custom_reset_parameters_1))

    try:
        while True:
            cmd: EnvironmentCommand = parent_conn.recv()

            if cmd.name == 'reset':
                unity_environment.reset()
                parent_conn.send('reset')

            elif cmd.name == "receive_observations":
                unity_environment._step_receive_observations()
                parent_conn.send('receive_observations')

            elif cmd.name == 'send_actions':
                unity_environment._step_send_actions()
                parent_conn.send('send_actions')

            elif cmd.name == 'episode_completed':
                unity_environment._episode_completed()
                parent_conn.send('episode_completed')

            elif cmd.name == "close":
                break
    except (KeyboardInterrupt, UnityCommunicationException, UnityTimeOutException):
        logger.info(f"UnityEnvironment worker {worker_id}: environment stopping.")
    finally:
        logger.debug(f"UnityEnvironment worker {worker_id} closing.")
        unity_environment.close()
        logger.debug(f"UnityEnvironment worker {worker_id} done.")


class SubprocessEnvManager:
    def __init__(self, n_env: int = 1):
        self.env_workers: List[UnityEnvWorker] = []
        self.n_env = n_env
        for worker_id in range(n_env):
            self.env_workers.append(self.create_worker(worker_id))

        self.external_brains = {}
        self.init_brains()

    def init_brains(self):
        params = custom_params.custom_reset_parameters_1
        params = get_reset_parameters(params)

        for brain_name in ['prey', 'predator']:
            self.external_brains[brain_name] = Brain(brain_name, 0)
            self.external_brains[brain_name].init_stacked(params, self.n_env)
    @staticmethod
    def create_worker(worker_id: int) -> UnityEnvWorker:
        parent_conn, child_conn = Pipe()

        child_process = Process(target=worker, args=(child_conn, worker_id))
        child_process.start()
        return UnityEnvWorker(child_process, worker_id, parent_conn)

    def command_to_all(self, command):
        for env_worker in self.env_workers:
            env_worker.send(command)
        for env_worker in self.env_workers:
            env_worker.recv()

    def run_episode(self, models):

        self.command_to_all('reset')

        for step in range(1000):
            self.command_to_all('receive_observations')

            agent_observations = {brain_name: self.external_brains[brain_name].get_stacked_observations() for brain_name in ['prey']}

            agent_actions = {brain_name: models[brain_name](agent_observations[brain_name]) for brain_name in ['prey']}

            for brain_name in ['prey']:
                self.external_brains[brain_name].set_agents_actions(agent_actions[brain_name])

            self.command_to_all('send_actions')

        self.command_to_all('episode_completed')

        fitness = {'prey': self.external_brains['prey'].get_stacked_fitness() for brain_name in ['prey']}

        return fitness

    def close(self) -> None:
        logger.debug(f"SubprocessEnvManager closing.")
        for env_worker in self.env_workers:
            env_worker.close()
