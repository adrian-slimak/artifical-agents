import logging
import subprocess
from typing import Dict, Optional

from .exception import (UnityEnvironmentException, UnityCommunicationException, UnityTimeOutException)

from mlagents.communicator_objects.unity_input_pb2 import UnityInputProto
from mlagents.communicator_objects.unity_output_pb2 import UnityOutputProto
from mlagents.communicator_objects.unity_initialization_input_pb2 import UnityInitializationInputProto
from mlagents.communicator_objects.unity_message_pb2 import  UnityMessageProto

from .np_communicator import NPCommunicator
import signal

logging.basicConfig(level=logging.INFO)
_logger = logging.getLogger("Environment")

unity_input = UnityInputProto()
unity_input.command = 0
message = UnityMessageProto()
message.status = 200
message.unity_input.CopyFrom(unity_input)
send_actions_message = message.SerializeToString()

class Environment:
    def __init__(self, file_path: Optional[str] = None, worker_id: int = 0, timeout_wait: int = 60, batch_mode=False):

        self.worker_id = worker_id
        self.academy_name = None
        self.file_path = file_path
        self.timeout_wait = timeout_wait
        self.communicator = NPCommunicator(self.worker_id, self.timeout_wait)
        self.batch_mode = batch_mode
        self._loaded = False
        self._process = None

    def initialize(self, initialization_input):
        if self.file_path is None and self.worker_id != 0:
            raise UnityEnvironmentException("If the environment name is None, the worker-id must be 0 in order to connect with the Editor.")

        if self.file_path is not None:
            self.executable_launcher(self.file_path)
        else:
            _logger.info("Start training by pressing the Play button in the Unity Editor.")

        self.communicator.create_pipe_connect()
        self._loaded = True

        try:
            unity_input = UnityInputProto()
            unity_input.initialization_input.CopyFrom(initialization_input)
            unity_output: UnityOutputProto = self.communicator.initialize(unity_input)
        except UnityTimeOutException:
            self._close()
            raise UnityEnvironmentException()

        self.academy_name = unity_output.initialization_output.name
        return unity_output

    def reset(self, environment_parameters: Dict = None) -> UnityOutputProto:
        if not self._loaded:
            raise UnityEnvironmentException("No Unity environment is loaded.")

        unity_input = UnityInputProto()
        unity_input.command = 1

        if environment_parameters is not None:
            unity_input.initialization_input.custom_reset_parameters = environment_parameters

        self.communicator.receive()  # Unity Environment is ready to receive new command
        self.communicator.send(unity_input)

        unity_output: UnityOutputProto = self.communicator.receive()

        if unity_output is None:
            raise UnityCommunicationException("Communicator has stopped.")

        return unity_output

    def status_receive_observations(self):
        self.communicator.receive()

    def status_send_actions(self):
        self.communicator.send_bytes(send_actions_message)

    def status_episode_completed(self):
        unity_input = UnityInputProto()
        unity_input.command = 2

        self.communicator.receive()  # Unity ready to take command
        self.communicator.send(unity_input)

        self.communicator.receive()
        self.communicator.send(unity_input)

    def executable_launcher(self, launch_string):
        _logger.debug(f"This is the launch string {launch_string}")

        # Launch Unity environment
        subprocess_args = [launch_string]
        subprocess_args += ["--port", str(self.worker_id)]
        if self.batch_mode:
            subprocess_args += ['-batchmode']
        try:
            self._process = subprocess.Popen(subprocess_args, start_new_session=True)
        except PermissionError as perm:
            # This is likely due to missing read or execute permissions on file.
            raise UnityEnvironmentException(f"Error when trying to launch environment - {launch_string}") from perm

    def close(self):
        if self._loaded:
            self._close()
        else:
            raise UnityEnvironmentException("No Unity environment is loaded.")

    def _close(self):
        self._loaded = False
        self.communicator.close()
        if self._process is not None:
            # Wait a bit for the process to shutdown, but kill it if it takes too long
            try:
                self._process.wait(timeout=self.timeout_wait)
                signal_name = self.returncode_to_signal_name(self._process.returncode)
                signal_name = f" ({signal_name})" if signal_name else ""
                return_info = f"Environment shut down with return code {self._process.returncode}{signal_name}."
                _logger.info(return_info)
            except subprocess.TimeoutExpired:
                _logger.info("Environment timed out shutting down. Killing...")
                self._process.kill()
            # Set to None so we don't try to close multiple times.
            self._process = None

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
