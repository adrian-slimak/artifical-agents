import time
import sys
import win32pipe, win32file, pywintypes
from timeit import default_timer as timer

from mlagents.envs.communicator_objects.unity_message_pb2 import UnityMessageProto
from mlagents.envs.communicator_objects.unity_input_pb2 import UnityInputProto
from mlagents.envs.communicator_objects.unity_output_pb2 import UnityOutputProto

print("pipe server")
count = 0
pipe = win32pipe.CreateNamedPipe(r'\\.\pipe\Foo', win32pipe.PIPE_ACCESS_DUPLEX,
                                 win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
                                 1, 65536, 65536, 0, None)

print("waiting for client")
win32pipe.ConnectNamedPipe(pipe, None)
print("got client")

step=0

while(True):
    t = timer()
    # print(f"Waiting for message {step} ...")
    result, data = win32file.ReadFile(pipe, 100000, None)
    message = UnityMessageProto()
    message.ParseFromString(data)
    print(message)
    # print(f"Message received: {s}")

    # print(f"Processing message {step} ...")
    time.sleep(4)

    # print(f"Sending message back {step} ...")
    message = UnityMessageProto()
    message.header.status = 100

    win32file.WriteFile(pipe, message.SerializeToString())
    # print("\n\n\n")
    print(timer() - t)

    step+=1


print("finished now")
win32file.CloseHandle(pipe)

