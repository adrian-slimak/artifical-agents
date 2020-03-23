import atexit
import logging
import numpy as np
import subprocess
from typing import Dict, Optional

from .brain import Brain
from .exception import (UnityEnvironmentException, UnityCommunicationException, UnityTimeOutException)

from mlagents.communicator_objects.unity_input_pb2 import UnityInputProto
from mlagents.communicator_objects.unity_output_pb2 import UnityOutputProto
from mlagents.communicator_objects.unity_initialization_input_pb2 import UnityInitializationInputProto
from utils import stick_reset_parameters

from .np_communicator import NPCommunicator
import signal

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("Environment")

from timeit import default_timer as timer


class UnityEnvironment:

    def __init__(self, file_name: Optional[str] = None, worker_id: int = 0,
                 initialization_input: UnityInitializationInputProto = None,
                 reset_parameters: Dict = None,
                 no_graphics: bool = False, timeout_wait: int = 60):

        atexit.register(self._close)
        self.worker_id = worker_id
        self._loaded = False
        self.proc1 = None
        self.timeout_wait: int = timeout_wait
        self.communicator = NPCommunicator(self.worker_id, self.timeout_wait)
        self.external_brains: Dict[str, Brain] = None

        # if file_name is None and worker_id != 0:
        #     raise UnityEnvironmentException("If the environment name is None, the worker-id must be 0 in order to connect with the Editor.")

        if file_name is not None:
            self.executable_launcher(file_name, no_graphics)
        else:
            logger.info("Start training by pressing the Play button in the Unity Editor.")

        self.communicator.create_pipe_connect()
        self._loaded = True

        try:
            unity_input = UnityInputProto()
            unity_input.initialization_input.CopyFrom(initialization_input)

            if unity_input.initialization_input.custom_reset_parameters is not None:
                self.external_brains = {}

                for brain_name in ['prey', 'predator']:
                    self.external_brains[brain_name] = Brain(brain_name, self.worker_id)
                    self.external_brains[brain_name].init_from_parameters(unity_input.initialization_input.custom_reset_parameters)

            unity_output: UnityOutputProto = self.communicator.initialize(unity_input)

        except UnityTimeOutException:
            self._close()
            raise

        self.academy_name = unity_output.initialization_output.name
        # self.default_reset_parameters = dict(unity_output.initialization_output.default_reset_parameters)

        logger.info("\n'{0}' started successfully!\n{1}".format(self.academy_name, str(self)))


    def run_single_episode(self, models, num_steps, reset_parameters=None):
        self.reset(reset_parameters)

        for step in range(num_steps):
            agent_observations = self.step_receive_observations()

            agent_actions = {}
            for brain_name in models.keys():
                actions = models[brain_name](agent_observations[brain_name])
                agent_actions[brain_name] = actions

            self.step_send_actions(agent_actions)

        fitness = self.episode_completed()
        return fitness


    def reset(self, reset_parameters: Dict = None):
        if not self._loaded:
            raise UnityEnvironmentException("No Unity environment is loaded.")

        unity_input = UnityInputProto()
        unity_input.command = 1

        if reset_parameters is not None:
            self.external_brains = {}
            reset_parameters = stick_reset_parameters(reset_parameters)

            for brain_name in ['prey', 'predator']:
                self.external_brains[brain_name] = Brain(brain_name, self.worker_id)
                self.external_brains[brain_name].init_from_parameters(reset_parameters)

                unity_input.initialization_input.custom_reset_parameters = reset_parameters

        logger.info("\n\tAcademy \"{0}_{1}\" reset.".format(self.academy_name, self.worker_id))
        # if reset_parameters is not None:
        #     logger.info("\n\tAcademy \"{0}_{1}\" reset with custom parameters:\n{2}".format(self.academy_name, self.worker_id, "\n\t\t ".join([str(x) + " -> " + str(custom_reset_parameters[x]) for x in custom_reset_parameters])))

        self.communicator.receive()  # Unity Environment is ready to receive new command
        self.communicator.send(unity_input)

        unity_output = self.communicator.receive()

        if unity_output is None:
            raise UnityCommunicationException("Communicator has stopped.")

    def _step_receive_observations(self):
        self.communicator.receive()

    def step_receive_observations(self):
        self.communicator.receive()

        # Read observations from memory
        state = {}
        for brain_name, brain in self.external_brains.items():
            state[brain_name] = brain.get_agents_observations()

        return state

    def _step_send_actions(self):
        unity_input = UnityInputProto()
        unity_input.command = 0

        self.communicator.send(unity_input)

    def step_send_actions(self, agents_actions: Dict[str, np.ndarray] = None):

        # Write actions to memory
        for brain_name, actions in agents_actions.items():
            self.external_brains[brain_name].set_agents_actions(actions)

        unity_input = UnityInputProto()
        unity_input.command = 0

        self.communicator.send(unity_input)

    def _episode_completed(self):
        unity_input = UnityInputProto()
        unity_input.command = 2

        self.communicator.receive()  # Unity ready to take command
        self.communicator.send(unity_input)

        self.communicator.receive()
        self.communicator.send(unity_input)

    def episode_completed(self):
        self._episode_completed()

        fitness = {}
        for brain_name, brain in self.external_brains.items():
            fitness[brain_name] = brain.get_agents_fitness()

        return fitness

    def executable_launcher(self, file_name, no_graphics):
        launch_string = file_name
        logger.debug("This is the launch string {}".format(launch_string))

        # Launch Unity environment
        subprocess_args = [launch_string]
        if no_graphics:
            subprocess_args += ["-nographics", "-batchmode"]
        subprocess_args += ["--port", str(self.worker_id)]
        try:
            self.proc1 = subprocess.Popen(subprocess_args, start_new_session=True)
        except PermissionError as perm:
            # This is likely due to missing read or execute permissions on file.
            raise UnityEnvironmentException(
                f"Error when trying to launch environment - make sure "
                f"permissions are set correctly. For example "
                f'"chmod -R 755 {launch_string}"'
            ) from perm

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

    def close(self):
        if self._loaded:
            self._close()
        else:
            raise UnityEnvironmentException("No Unity environment is loaded.")

    def _close(self):
        self._loaded = False
        self.communicator.close()
        if self.proc1 is not None:
            # Wait a bit for the process to shutdown, but kill it if it takes too long
            try:
                self.proc1.wait(timeout=self.timeout_wait)
                signal_name = self.returncode_to_signal_name(self.proc1.returncode)
                signal_name = f" ({signal_name})" if signal_name else ""
                return_info = f"Environment shut down with return code {self.proc1.returncode}{signal_name}."
                logger.info(return_info)
            except subprocess.TimeoutExpired:
                logger.info("Environment timed out shutting down. Killing...")
                self.proc1.kill()
            # Set to None so we don't try to close multiple times.
            self.proc1 = None

    @staticmethod
    def returncode_to_signal_name(returncode: int) -> Optional[str]:
        """
        Try to convert return codes into their corresponding signal name.
        E.g. returncode_to_signal_name(-2) -> "SIGINT"
        """
        try:
            # A negative value -N indicates that the child was terminated by signal N (POSIX only).
            s = signal.Signals(-returncode)  # pylint: disable=no-member
            return s.name
        except Exception:
            # Should generally be a ValueError, but catch everything just in case.
            return None
