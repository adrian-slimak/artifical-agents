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

from mlagents.envs.communicator_objects.unity_input_pb2 import UnityInputProto
from mlagents.envs.communicator_objects.unity_output_pb2 import UnityOutputProto
from mlagents.envs.communicator_objects.unity_initialization_input_pb2 import UnityInitializationInputProto
from mlagents.envs.communicator_objects.engine_configuration_pb2 import EngineConfigurationProto

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
        seed: int = 0,
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

        engine_config = EngineConfigurationProto()
        engine_config.width = 100
        engine_config.height = 100
        engine_config.quality_level = 1
        engine_config.time_scale = 1
        engine_config.target_frame_rate = -1
        engine_config.show_monitor = False
        initialization_input = UnityInitializationInputProto(seed=seed)

        try:
            unity_input = UnityInputProto()
            unity_input.initialization_input.CopyFrom(initialization_input)
            unity_output: UnityOutputProto = self.communicator.initialize(unity_input)
            initialization_output = unity_output.initialization_output

        except UnityTimeOutException:
            self._close()
            raise

        self._is_first_message = True
        self._academy_name = initialization_output.name
        # self._log_path = aca_params.log_path
        self._resetParameters = dict(initialization_output.environment_parameters.float_parameters)

        logger.info(
            "\n'{0}' started successfully!\n{1}".format(self._academy_name, str(self))
        )

    @property
    def academy_name(self):
        return self._academy_name
    @property
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
                    str(k) + " -> " + str(self._resetParameters[k])
                    for k in self._resetParameters
                ]
            )
            if self._resetParameters
            else "{}"
        )
        return f"""Unity Academy name: {self._academy_name}
        Reset Parameters : {reset_params_str}"""

    def reset(self, config: Dict = None):
        if config is None:
            config = self._resetParameters
        elif config:
            logger.info(
                "Academy reset with parameters: {0}".format(
                    ", ".join([str(x) + " -> " + str(config[x]) for x in config])
                )
            )
        for k in config:
            if (k in self._resetParameters) and (isinstance(config[k], (int, float))):
                self._resetParameters[k] = config[k]

        if not self._loaded:
            raise UnityEnvironmentException("No Unity environment is loaded.")

        unity_input = UnityInputProto()
        unity_input.command = 1
        self.communicator.exchange(unity_input)
        unity_output = self.communicator.exchange()

        if unity_output is None:
            raise UnityCommunicationException("Communicator has stopped.")

        initialization_output = unity_output.initialization_output
        for brain in initialization_output.brain_parameters:
            self.external_brains[brain.brain_name] = BrainParameters(brain)

        self._is_first_message = False

    @timed
    def step(self, vector_action: Dict[str, np.ndarray] = None):
        if self._is_first_message:
            return self.reset()

        state = {}

        # Check that environment is loaded, and episode is currently running.
        if not self._loaded:
            raise UnityEnvironmentException("No Unity environment is loaded.")

        start = timer()
        for brain in self.external_brains.values():
            mm = mmap.mmap(fileno=-1, length=200000, tagname='unity_input')
            if vector_action:
                byteArray = vector_action['prey'].tobytes()
                mm.seek(brain.mmf_offset_actions)
                mm.write(byteArray)
        # print(timer() - start)

        step_input = self._generate_step_input()
        with hierarchical_timer("communicator.exchange"):
            outputs = self.communicator.exchange(step_input)

        start = timer()
        for brain in self.external_brains.values():
            mm = mmap.mmap(fileno=-1, length=200000, tagname='unity_output')
            mm.seek(brain.mmf_offset_observations)
            observations = np.fromstring(mm.read(brain.mmf_size_observations), dtype='f')
            state[brain.brain_name] = observations.reshape(brain.agents_count, brain.observation_vectors_size)
        # print(timer() - start)

        if outputs is None:
            raise UnityCommunicationException("Communicator has stopped.")
        return state

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

    @timed
    def _generate_step_input(self) -> UnityInputProto:
        unity_input = UnityInputProto()
        unity_input.command = 0
        return unity_input

    def _generate_reset_input(self, config: Dict) -> UnityInputProto:
        unity_input = UnityInputProto()
        # unity_input.initialization_input.environment_parameters.CopyFrom(EnvironmentParametersProto())
        # for key in config:
        #     unity_input.initialization_input.environment_parameters.float_parameters[key] = config[key]
        unity_input.command = 1
        return unity_input

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
