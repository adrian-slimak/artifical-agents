import atexit
import glob
import logging
import numpy as np
import os
import subprocess
from typing import Dict, List, Optional, Any

from mlagents.envs.base_unity_environment import BaseUnityEnvironment
from mlagents.envs.timers import timed, hierarchical_timer
from .brain import AllBrainInfo, BrainInfo, BrainParameters
from .exception import (
    UnityEnvironmentException,
    UnityCommunicationException,
    UnityActionException,
    UnityTimeOutException,
)
from timeit import default_timer as timer


from mlagents.envs.communicator_objects.unity_rl_input_pb2 import UnityRLInputProto
from mlagents.envs.communicator_objects.unity_rl_output_pb2 import UnityRLOutputProto
from mlagents.envs.communicator_objects.environment_parameters_pb2 import (
    EnvironmentParametersProto,
)
from mlagents.envs.communicator_objects.unity_output_pb2 import UnityOutputProto
from mlagents.envs.communicator_objects.unity_rl_initialization_input_pb2 import (
    UnityRLInitializationInputProto,
)

from mlagents.envs.communicator_objects.unity_input_pb2 import UnityInputProto

import mmap
import numpy as np


from .rpc_communicator import RpcCommunicator
from sys import platform
import signal

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("mlagents.envs")


class UnityEnvironment(BaseUnityEnvironment):

    def __init__(
        self,
        file_name: Optional[str] = None,
        worker_id: int = 0,
        base_port: int = 5005,
        seed: int = 0,
        docker_training: bool = False,
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
        :int timeout_wait: Time (in seconds) to wait for connection from environment.
        :bool train_mode: Whether to run in training mode, speeding up the simulation, by default.
        :list args: Addition Unity command line arguments
        """
        args = args or []
        atexit.register(self._close)
        self.port = base_port + worker_id
        self._buffer_size = 12000
        # If true, this means the environment was successfully loaded
        self._loaded = False
        # The process that is started. If None, no process was started
        self.proc1 = None
        self.timeout_wait: int = timeout_wait
        self.communicator = self.get_communicator(worker_id, base_port, timeout_wait)
        self.worker_id = worker_id

        # If the environment name is None, a new environment will not be launched
        # and the communicator will directly try to connect to an existing unity environment.
        # If the worker-id is not 0 and the environment name is None, an error is thrown
        if file_name is None and worker_id != 0:
            raise UnityEnvironmentException(
                "If the environment name is None, "
                "the worker-id must be 0 in order to connect with the Editor."
            )

        if file_name is not None:
            self.executable_launcher(file_name, docker_training, no_graphics, args)
        else:
            logger.info(
                "Start training by pressing the Play button in the Unity Editor."
            )
        self._loaded = True

        rl_init_parameters_in = UnityRLInitializationInputProto(seed=seed)
        try:
            aca_output = self.send_academy_parameters(rl_init_parameters_in)
            aca_params = aca_output.rl_initialization_output
        except UnityTimeOutException:
            self._close()
            raise

        self._n_agents: Dict[str, int] = {}
        self._is_first_message = True
        self._academy_name = aca_params.name
        self._log_path = aca_params.log_path
        self._resetParameters = dict(aca_params.environment_parameters.float_parameters)
        logger.info(
            "\n'{0}' started successfully!\n{1}".format(self._academy_name, str(self))
        )

    @property
    def logfile_path(self):
        return self._log_path

    @property
    def academy_name(self):
        return self._academy_name

    @staticmethod
    def get_communicator(worker_id, base_port, timeout_wait):
        return RpcCommunicator(worker_id, base_port, timeout_wait)

    @property
    def external_brains(self):
        external_brains = {}
        for brain_name in self.external_brain_names:
            external_brains[brain_name] = self.brains[brain_name]
        return external_brains

    @property
    def reset_parameters(self):
        return self._resetParameters

    def executable_launcher(self, file_name, docker_training, no_graphics, args):
        cwd = os.getcwd()
        file_name = (
            file_name.strip()
            .replace(".app", "")
            .replace(".exe", "")
            .replace(".x86_64", "")
            .replace(".x86", "")
        )
        true_filename = os.path.basename(os.path.normpath(file_name))
        logger.debug("The true file name is {}".format(true_filename))
        launch_string = None
        if platform == "linux" or platform == "linux2":
            candidates = glob.glob(os.path.join(cwd, file_name) + ".x86_64")
            if len(candidates) == 0:
                candidates = glob.glob(os.path.join(cwd, file_name) + ".x86")
            if len(candidates) == 0:
                candidates = glob.glob(file_name + ".x86_64")
            if len(candidates) == 0:
                candidates = glob.glob(file_name + ".x86")
            if len(candidates) > 0:
                launch_string = candidates[0]

        elif platform == "darwin":
            candidates = glob.glob(
                os.path.join(
                    cwd, file_name + ".app", "Contents", "MacOS", true_filename
                )
            )
            if len(candidates) == 0:
                candidates = glob.glob(
                    os.path.join(file_name + ".app", "Contents", "MacOS", true_filename)
                )
            if len(candidates) == 0:
                candidates = glob.glob(
                    os.path.join(cwd, file_name + ".app", "Contents", "MacOS", "*")
                )
            if len(candidates) == 0:
                candidates = glob.glob(
                    os.path.join(file_name + ".app", "Contents", "MacOS", "*")
                )
            if len(candidates) > 0:
                launch_string = candidates[0]
        elif platform == "win32":
            candidates = glob.glob(os.path.join(cwd, file_name + ".exe"))
            if len(candidates) == 0:
                candidates = glob.glob(file_name + ".exe")
            if len(candidates) > 0:
                launch_string = candidates[0]
        if launch_string is None:
            self._close()
            raise UnityEnvironmentException(
                "Couldn't launch the {0} environment. "
                "Provided filename does not match any environments.".format(
                    true_filename
                )
            )
        else:
            logger.debug("This is the launch string {}".format(launch_string))
            # Launch Unity environment
            if not docker_training:
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

            else:
                # Comments for future maintenance:
                #     xvfb-run is a wrapper around Xvfb, a virtual xserver where all
                #     rendering is done to virtual memory. It automatically creates a
                #     new virtual server automatically picking a server number `auto-servernum`.
                #     The server is passed the arguments using `server-args`, we are telling
                #     Xvfb to create Screen number 0 with width 640, height 480 and depth 24 bits.
                #     Note that 640 X 480 are the default width and height. The main reason for
                #     us to add this is because we'd like to change the depth from the default
                #     of 8 bits to 24.
                #     Unfortunately, this means that we will need to pass the arguments through
                #     a shell which is why we set `shell=True`. Now, this adds its own
                #     complications. E.g SIGINT can bounce off the shell and not get propagated
                #     to the child processes. This is why we add `exec`, so that the shell gets
                #     launched, the arguments are passed to `xvfb-run`. `exec` replaces the shell
                #     we created with `xvfb`.
                #
                docker_ls = (
                    "exec xvfb-run --auto-servernum"
                    " --server-args='-screen 0 640x480x24'"
                    " {0} --port {1}"
                ).format(launch_string, str(self.port))
                self.proc1 = subprocess.Popen(
                    docker_ls,
                    stdout=subprocess.PIPE,
                    stderr=subprocess.PIPE,
                    shell=True,
                )

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

    def reset(self, config: Dict = None, train_mode: bool = True, custom_reset_parameters: Any = None) -> AllBrainInfo:
        """
        Sends a signal to reset the unity environment.
        :return: AllBrainInfo  : A data structure corresponding to the initial reset state of the environment.
        """
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
            elif not isinstance(config[k], (int, float)):
                raise UnityEnvironmentException(
                    "The value for parameter '{0}'' must be an Integer or a Float.".format(
                        k
                    )
                )
            else:
                raise UnityEnvironmentException(
                    "The parameter '{0}' is not a valid parameter.".format(k)
                )

        if not self._loaded:
            raise UnityEnvironmentException("No Unity environment is loaded.")

        outputs = self.communicator.exchange(
            self._generate_reset_input(train_mode, config, custom_reset_parameters)
        )
        if outputs is None:
            raise UnityCommunicationException("Communicator has stopped.")

        self._is_first_message = False

        return None



    @timed
    def step(self, vector_action: Dict[str, np.ndarray] = None):
        if self._is_first_message:
            return self.reset()

        # Check that environment is loaded, and episode is currently running.
        if not self._loaded:
            raise UnityEnvironmentException("No Unity environment is loaded.")

        start = timer()
        mm = mmap.mmap(fileno=-1, length=200000, tagname='unity_input')
        if vector_action:
            byteArray = vector_action['prey'].tobytes()
            mm.write(byteArray)
        # print(timer() - start)

        step_input = self._generate_step_input()
        with hierarchical_timer("communicator.exchange"):
            outputs = self.communicator.exchange(step_input)

        start = timer()
        mm = mmap.mmap(fileno=-1, length=200000, tagname='unity_output')
        numBytes = 40000
        floats = np.fromstring(mm.read(numBytes), dtype='f')
        # print(timer() - start)

        if outputs is None:
            raise UnityCommunicationException("Communicator has stopped.")

        return {"prey": floats}

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
        rl_in = UnityRLInputProto()
        rl_in.command = 0
        return self.wrap_unity_input(rl_in)


    def _generate_reset_input(
        self, training: bool, config: Dict, custom_reset_parameters: Any
    ) -> UnityInputProto:
        rl_in = UnityRLInputProto()
        rl_in.is_training = training
        rl_in.environment_parameters.CopyFrom(EnvironmentParametersProto())
        for key in config:
            rl_in.environment_parameters.float_parameters[key] = config[key]
        if custom_reset_parameters is not None:
            rl_in.environment_parameters.custom_reset_parameters.CopyFrom(
                custom_reset_parameters
            )
        rl_in.command = 1
        return self.wrap_unity_input(rl_in)

    def send_academy_parameters(
        self, init_parameters: UnityRLInitializationInputProto
    ) -> UnityOutputProto:
        inputs = UnityInputProto()
        inputs.rl_initialization_input.CopyFrom(init_parameters)
        return self.communicator.initialize(inputs)

    @staticmethod
    def wrap_unity_input(rl_input: UnityRLInputProto) -> UnityInputProto:
        result = UnityInputProto()
        result.rl_input.CopyFrom(rl_input)
        return result

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
