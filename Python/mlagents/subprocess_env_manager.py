import logging
from typing import NamedTuple, List, Any

from mlagents.environment import UnityEnvironment
from mlagents.exception import UnityCommunicationException, UnityTimeOutException
from multiprocessing import Process, Pipe
from multiprocessing.connection import Connection
from utils import get_initialization_input, load_custom_reset_parameters, setup_tensorflow
from LSTM import LSTMModel
import configs.custom_reset_parameters as custom_params
import multiprocessing
multiprocessing.set_start_method('forkserver')

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
    unity_environment: UnityEnvironment = UnityEnvironment(file_name=unity_env_path, worker_id=worker_id, initialization_input=get_initialization_input())
    setup_tensorflow()

    try:
        while True:
            cmd: EnvironmentCommand = parent_conn.recv()

            if cmd.name == "run_single_episode":
                models = cmd.payload
                prey_weights, prey_parameters = models['prey']
                input_dims = prey_parameters[0]
                prey_parameters = tuple(prey_parameters[-3:])
                prey_model = LSTMModel(*prey_parameters)
                prey_model.build(input_shape=(1, input_dims), model_weights=prey_weights)

                # predator_weights, predator_parameters = models['predator']
                # input_dims = predator_parameters[0]
                # predator_parameters = tuple(predator_parameters[-3:])
                # predator_model = LSTMModel(*predator_parameters)
                # predator_model.build(input_shape=(1, input_dims), model_weights=predator_weights)

                fitness = unity_environment.run_single_episode({'prey': prey_model}, 1000, custom_params.custom_reset_parameters_1)

                parent_conn.send(EnvironmentResponse("fitness_response", worker_id, fitness))

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
        for worker_id in range(n_env):
            self.env_workers.append(self.create_worker(worker_id))

    @staticmethod
    def create_worker(worker_id: int) -> UnityEnvWorker:
        parent_conn, child_conn = Pipe()

        child_process = Process(target=worker, args=(child_conn, worker_id))
        child_process.start()
        return UnityEnvWorker(child_process, worker_id, parent_conn)

    def run_episode(self, models):

        for env_worker in self.env_workers:
            env_worker.send("run_single_episode", models)

        fitness = {}
        for env_worker in self.env_workers:
            response: EnvironmentResponse = env_worker.recv()
            fitness[env_worker.worker_id] = response.payload

        return fitness

    def close(self) -> None:
        logger.debug(f"SubprocessEnvManager closing.")
        for env_worker in self.env_workers:
            env_worker.close()
