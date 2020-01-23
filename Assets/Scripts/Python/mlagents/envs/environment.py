import atexit
import logging
import numpy as np
import subprocess
from typing import Dict, Optional
from tensorflow import expand_dims
from Memory import Memory

from .brain import BrainParameters
from .exception import (UnityEnvironmentException, UnityCommunicationException, UnityTimeOutException)
from timeit import default_timer as timer
import time

from mlagents.envs.communicator_objects.unity_input_pb2 import UnityInputProto
from mlagents.envs.communicator_objects.unity_output_pb2 import UnityOutputProto
from mlagents.envs.communicator_objects.unity_initialization_input_pb2 import UnityInitializationInputProto

from .rpc_communicator import RpcCommunicator
import signal

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("mlagents.envs")


class UnityEnvironment():

    def __init__(self, file_name: Optional[str] = None, worker_id: int = 0, base_port: int = 5000,
                 initialization_input: UnityInitializationInputProto = None, no_graphics: bool = False, timeout_wait: int = 60):

        atexit.register(self._close)
        self.port = base_port + worker_id
        self.worker_id = worker_id
        self._loaded = False
        self.proc1 = None
        self.timeout_wait: int = timeout_wait
        self.communicator = RpcCommunicator(self.worker_id, self.port, self.timeout_wait)
        self.external_brains = {}

        if file_name is None and worker_id != 0:
            raise UnityEnvironmentException("If the environment name is None, the worker-id must be 0 in order to connect with the Editor.")

        if file_name is not None:
            self.executable_launcher(file_name, no_graphics)
        else:
            logger.info("Start training by pressing the Play button in the Unity Editor.")
        self._loaded = True

        try:
            unity_input = UnityInputProto()
            unity_input.initialization_input.CopyFrom(initialization_input)
            unity_output: UnityOutputProto = self.communicator.initialize(unity_input)
            initialization_output = unity_output.initialization_output

        except UnityTimeOutException:
            self._close()
            raise

        self.memory = Memory(self.port)
        self.academy_name = initialization_output.name
        # self._log_path = aca_params.log_path
        self.defaultResetParameters = dict(initialization_output.default_reset_parameters)

        logger.info("\n'{0}' started successfully!\n{1}".format(self.academy_name, str(self)))


    def get_external_brains(self):
        return self.external_brains

    def reset(self, custom_reset_parameters: Dict = None):
        if not self._loaded:
            raise UnityEnvironmentException("No Unity environment is loaded.")

        unity_input = UnityInputProto()
        unity_input.command = 1

        if custom_reset_parameters is not None:
            logger.info("Academy \"{0}\" reset with custom parameters parameters: {1}".format(self.academy_name,
                    ", ".join([str(x) + " -> " + str(custom_reset_parameters[x]) for x in custom_reset_parameters])))

            for key, value in custom_reset_parameters.items():
                unity_input.initialization_input.custom_reset_parameters[key] = value


        self.communicator.exchange(unity_input)
        unity_output = self.communicator.exchange()

        if unity_output is None:
            raise UnityCommunicationException("Communicator has stopped.")

        for brain in unity_output.initialization_output.brain_parameters:
            self.external_brains[brain.brain_name] = BrainParameters(brain)

    def step_receive_observations(self):
        unity_output = self.communicator.receive()

        # Read observations from memory
        state = self.memory.ReadAgentsObservations(self.external_brains)

        if unity_output is None:
            raise UnityCommunicationException("Communicator has stopped.")
        return state

    def step_send_actions(self, agents_actions: Dict[str, np.ndarray] = None):

        # Write actions to memory
        self.memory.WriteAgentsActions(self.external_brains, agents_actions)

        unity_input = UnityInputProto()
        unity_input.command = 0

        self.communicator.send(unity_input)

    def episode_completed(self):
        self.communicator.receive()
        unity_input = UnityInputProto()
        unity_input.command = 3
        self.communicator.send(unity_input)

        self.communicator.receive()
        self.communicator.send(unity_input)

        fitness = self.memory.ReadAgentsFitness(self.external_brains)

        return fitness

    def run_single_episode(self, model, num_steps, custom_reset_parameters):
        self.reset(custom_reset_parameters)

        for step in range(num_steps):
            # s = timer()
            agent_observations = self.step_receive_observations()
            prey_observations = agent_observations['prey']

            prey_actions = model["prey"](agent_observations['prey'])
            agent_actions = {'prey': prey_actions}

            self.step_send_actions(agent_actions)
            # print(timer() - s)

        fitness = self.episode_completed()
        return fitness

    def executable_launcher(self, file_name, no_graphics):
        launch_string = file_name
        logger.debug("This is the launch string {}".format(launch_string))

        # Launch Unity environment
        subprocess_args = [launch_string]
        if no_graphics:
            subprocess_args += ["-nographics", "-batchmode"]
        subprocess_args += ["--port", str(self.port)]
        try:
            self.proc1 = subprocess.Popen(
                subprocess_args,
                # start_new_session=True means that signals to the parent python process
                # (e.g. SIGINT from keyboard interrupt) will not be sent to the new process on POSIX platforms.
                # This is generally good since we want the environment to have a chance to shutdown,
                # but may be undesirable in come cases; if so, we'll add a command-line toggle.
                # Note that on Windows, the CTRL_C signal will still be sent.
                start_new_session=True,
            )
        except PermissionError as perm:
            # This is likely due to missing read or execute permissions on file.
            raise UnityEnvironmentException(
                f"Error when trying to launch environment - make sure "
                f"permissions are set correctly. For example "
                f'"chmod -R 755 {launch_string}"'
            ) from perm

    def __str__(self):
        reset_params_str = (
            "\n\t\t".join(
                [
                    str(k) + " -> " + str(self.defaultResetParameters[k])
                    for k in self.defaultResetParameters
                ]
            )
            if self.defaultResetParameters
            else "{}"
        )
        return f"Unity Academy name: {self.academy_name}\n\tDefault Reset Parameters:\n\t\t{reset_params_str}"

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
