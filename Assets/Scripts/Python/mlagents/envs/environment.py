import atexit
import glob
import logging
import numpy as np
import os
import subprocess
from typing import Dict, List, Optional

from mlagents.envs.timers import timed, hierarchical_timer
from .brain import BrainParameters
from .exception import (UnityEnvironmentException, UnityCommunicationException, UnityTimeOutException)
from timeit import default_timer as timer
import time

from mlagents.envs.communicator_objects.unity_input_pb2 import UnityInputProto
from mlagents.envs.communicator_objects.unity_output_pb2 import UnityOutputProto
from mlagents.envs.communicator_objects.unity_initialization_input_pb2 import UnityInitializationInputProto

import mmap

from .rpc_communicator import RpcCommunicator
from sys import platform
import signal

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("mlagents.envs")


class UnityEnvironment():

    def __init__(
        self,
        file_name: Optional[str] = None,
        worker_id: int = 0,
        base_port: int = 5004,
        initialization_input: UnityInitializationInputProto = None,
        no_graphics: bool = False,
        timeout_wait: int = 60,
        args: Optional[List[str]] = None,
    ):
        """
        Starts a new unity environment and establishes a connection with the environment.
        Notice: Currently communication between Unity and Python takes place over an open socket without authentication.
        Ensure that the network where training takes place is secure.

        :string file_name: Name of Unity environment binary.
        :int base_port: Baseline port number to connect to Unity environment over. worker_id increments over this.
        :int worker_id: Number to add to communication port (5005) [0]. Used for asynchronous agent scenarios.
        :bool docker_training: Informs this class whether the process is being run within a container.
        :bool no_graphics: Whether to run the Unity simulator in no-graphics mode
        :int timeout_wait: Time (in seconds) to wait for connection from environment
        :list args: Addition Unity command line arguments
        """
        args = args or []
        atexit.register(self._close)
        self.port = base_port + worker_id
        self.worker_id = worker_id
        # If true, this means the environment was successfully loaded
        self._loaded = False
        # The process that is started. If None, no process was started
        self.proc1 = None
        self.timeout_wait: int = timeout_wait
        self.communicator = RpcCommunicator(self.worker_id, self.port, self.timeout_wait)
        self.external_brains = {}


        if file_name is None and worker_id != 0:
            raise UnityEnvironmentException("If the environment name is None, the worker-id must be 0 in order to connect with the Editor.")

        if file_name is not None:
            self.executable_launcher(file_name, no_graphics, args)
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

        self._academy_name = initialization_output.name
        # self._log_path = aca_params.log_path
        self._defaultResetParameters = dict(initialization_output.default_reset_parameters)

        logger.info(
            "\n'{0}' started successfully!\n{1}".format(self._academy_name, str(self))
        )

    @property
    def academy_name(self):
        return self._academy_name

    def get_external_brains(self):
        return self.external_brains

    @property
    def reset_parameters(self):
        return self._resetParameters

    def executable_launcher(self, file_name, no_graphics, args):
        launch_string = file_name
        logger.debug("This is the launch string {}".format(launch_string))

        # Launch Unity environment
        subprocess_args = [launch_string]
        if no_graphics:
            subprocess_args += ["-nographics", "-batchmode"]
        subprocess_args += ["--port", str(self.port)]
        subprocess_args += args
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
                    str(k) + " -> " + str(self._defaultResetParameters[k])
                    for k in self._defaultResetParameters
                ]
            )
            if self._defaultResetParameters
            else "{}"
        )
        return f"""Unity Academy name: {self._academy_name}
        Default Reset Parameters : {reset_params_str}"""

    def reset(self, custom_reset_parameters: Dict = None):
        if not self._loaded:
            raise UnityEnvironmentException("No Unity environment is loaded.")

        unity_input = UnityInputProto()
        unity_input.command = 1

        if custom_reset_parameters is not None:
            logger.info(
                "Academy \"{0}\" reset with custom parameters parameters: {1}".format(self.academy_name,
                    ", ".join([str(x) + " -> " + str(custom_reset_parameters[x]) for x in custom_reset_parameters])
                )
            )
            for key, value in custom_reset_parameters.items():
                unity_input.initialization_input.custom_reset_parameters[key] = value


        self.communicator.exchange(unity_input)
        unity_output = self.communicator.exchange()

        if unity_output is None:
            raise UnityCommunicationException("Communicator has stopped.")

        initialization_output = unity_output.initialization_output
        for brain in initialization_output.brain_parameters:
            self.external_brains[brain.brain_name] = BrainParameters(brain)

    # @timed
    # def step(self, vector_action: Dict[str, np.ndarray] = None):
    #     # Check that environment is loaded, and episode is currently running.
    #     if not self._loaded:
    #         raise UnityEnvironmentException("No Unity environment is loaded.")
    #
    #     # Write actions to memory
    #     start = timer()
    #     for brain in self.external_brains.values():
    #         mm = mmap.mmap(fileno=-1, length=200000, tagname='unity_input')
    #         if vector_action:
    #             byteArray = vector_action['prey'].tobytes()
    #             mm.seek(brain.mmf_offset_actions)
    #             mm.write(byteArray)
    #     # print(timer() - start)
    #
    #     step_input = self._generate_step_input()
    #     unity_output = self.communicator.exchange(step_input)
    #
    #     # Read observations from memory
    #     state = {}
    #     start = timer()
    #     for brain in self.external_brains.values():
    #         mm = mmap.mmap(fileno=-1, length=200000, tagname='unity_output')
    #         mm.seek(brain.mmf_offset_observations)
    #         observations = np.fromstring(mm.read(brain.mmf_size_observations), dtype='f')
    #         state[brain.brain_name] = observations.reshape(brain.agents_count, brain.observation_vector_size)
    #     # print(timer() - start)
    #
    #     if unity_output is None:
    #         raise UnityCommunicationException("Communicator has stopped.")
    #     return state

    def step_receive_observations(self):
        unity_output = self.communicator.receive()

        # Read observations from memory
        state = {}
        for brain in self.external_brains.values():
            mm = mmap.mmap(fileno=-1, length=200000, tagname='unity_output')
            mm.seek(brain.mmf_offset_observations)
            observations = np.fromstring(mm.read(brain.mmf_size_observations), dtype='f')
            state[brain.brain_name] = observations.reshape(brain.agents_count, brain.observation_vector_size)

        if unity_output is None:
            raise UnityCommunicationException("Communicator has stopped.")
        return state

    def step_send_actions(self, vector_action: Dict[str, np.ndarray] = None):

        # Write actions to memory
        for brain in self.external_brains.values():
            mm = mmap.mmap(fileno=-1, length=200000, tagname='unity_input')
            if vector_action:
                byteArray = vector_action['prey'].tobytes()
                mm.seek(brain.mmf_offset_actions)
                mm.write(byteArray)

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

        fitness = {}
        with open(r"C:\Users\adek1\Desktop\fitness.txt", 'r') as file:
            for line in file.readlines():
                splitted = line.split(' ')
                if len(splitted) > 1:
                    f_list = [float(i) for i in splitted[1:-1]]
                    fitness[splitted[0]] = f_list

        return fitness

    def close(self):
        """
        Sends a shutdown signal to the unity environment, and closes the socket connection.
        """
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

    def send_academy_parameters(self, initialization_input: UnityInitializationInputProto) -> UnityOutputProto:
        inputs = UnityInputProto()
        inputs.initialization_input.CopyFrom(initialization_input)
        return self.communicator.initialize(inputs)

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
