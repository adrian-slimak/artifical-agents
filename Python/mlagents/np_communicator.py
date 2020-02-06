import win32pipe, win32file
from typing import Optional

from mlagents.communicator_objects.unity_message_pb2 import UnityMessageProto
from mlagents.communicator_objects.unity_input_pb2 import UnityInputProto
from mlagents.communicator_objects.unity_output_pb2 import UnityOutputProto
from .exception import UnityWorkerInUseException, UnityCommunicationException

class NPCommunicator():
    def __init__(self, worker_id=0, timeout_wait=60):
        self.worker_id = worker_id
        self.pipe_name = f'\\\\.\\pipe\\named_pipe_{worker_id}'
        self.timeout_wait = timeout_wait
        self._pipe = None
        self.is_open = False


    def create_pipe_connect(self):
        """
        Creates Named Pipe.
        Error if Pipe with given name already exists.
        """
        try:
            self._pipe = win32pipe.CreateNamedPipe(self.pipe_name, win32pipe.PIPE_ACCESS_DUPLEX,
                                                   win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
                                                   1, 65536, 65536, 0, None)
            win32pipe.ConnectNamedPipe(self._pipe, None)
            self.is_open = True
        except Exception:
            raise UnityWorkerInUseException(self.worker_id)


    def initialize(self, unity_input: UnityInputProto) -> UnityOutputProto:
        if not self.is_open:
            raise UnityCommunicationException()

        self.send(unity_input)

        unity_output = self.receive()

        return unity_output


    def send(self, unity_input: UnityInputProto=None) -> Optional[UnityOutputProto]:
        message = UnityMessageProto()
        message.status = 200
        if unity_input:
            message.unity_input.CopyFrom(unity_input)

        win32file.WriteFile(self._pipe, message.SerializeToString())

    def receive(self):

        result, data = win32file.ReadFile(self._pipe, 100000, None)

        message = UnityMessageProto()
        message.ParseFromString(data)

        if message.status != 200:
            return None
        return message.unity_output

    def close(self):
        """
        Sends a shutdown signal to the unity environment, and closes named pipe connection.
        """
        if self.is_open:
            message = UnityMessageProto()
            message.status = 400
            win32file.WriteFile(self._pipe, message.SerializeToString())
            # self.unity_to_external.parent_conn.close()
            # self.server.stop(False)
            self.is_open = False