from mlagents.envs.environment import UnityEnvironment
import tensorflow as tf
import numpy as np
# tf.debugging.set_log_device_placement(True)
from LSTM import LSTMModel
from time import sleep
from timeit import default_timer as timer
import mmap
from mlagents.envs.communicator_objects.unity_output_pb2 import UnityOutputProto
from mlagents.envs.communicator_objects.unity_message_pb2 import UnityMessageProto
brainNames = []

def main():
    # tf_utils.set_warnings_enabled(False)
    unity_environment = UnityEnvironment(worker_id=0, base_port=5004)
    unity_environment.reset()

    # brainNames = unity_environment.external_brain_names

    env_state = unity_environment.step()
    prey_observations = env_state['prey']


    # start = timer()
    # print(message.header.status)
    # print(timer() - start)

    # prey_observations = env_state['prey'].vector_observations
    # prey_observations = np.reshape(prey_observations, (100, 100))

    prey_model = LSTMModel(32, prey_observations.shape[0])
    prey_model.build(input_shape=(1, prey_observations.shape[1]))



    while True:
        # start = timer()
        output = prey_model(tf.expand_dims(prey_observations, 1))
        action = {'prey': output.numpy().flatten()}
        env_state = unity_environment.step(action)
        prey_observations = env_state['prey']
        # end = timer()
        # print(end - start)

    unity_environment.close()

def unityLogo():
    try:
        print(
            """

                        ▄▄▄▓▓▓▓
                   ╓▓▓▓▓▓▓█▓▓▓▓▓
              ,▄▄▄m▀▀▀'  ,▓▓▓▀▓▓▄                           ▓▓▓  ▓▓▌
            ▄▓▓▓▀'      ▄▓▓▀  ▓▓▓      ▄▄     ▄▄ ,▄▄ ▄▄▄▄   ,▄▄ ▄▓▓▌▄ ▄▄▄    ,▄▄
          ▄▓▓▓▀        ▄▓▓▀   ▐▓▓▌     ▓▓▌   ▐▓▓ ▐▓▓▓▀▀▀▓▓▌ ▓▓▓ ▀▓▓▌▀ ^▓▓▌  ╒▓▓▌
        ▄▓▓▓▓▓▄▄▄▄▄▄▄▄▓▓▓      ▓▀      ▓▓▌   ▐▓▓ ▐▓▓    ▓▓▓ ▓▓▓  ▓▓▌   ▐▓▓▄ ▓▓▌
        ▀▓▓▓▓▀▀▀▀▀▀▀▀▀▀▓▓▄     ▓▓      ▓▓▌   ▐▓▓ ▐▓▓    ▓▓▓ ▓▓▓  ▓▓▌    ▐▓▓▐▓▓
          ^█▓▓▓        ▀▓▓▄   ▐▓▓▌     ▓▓▓▓▄▓▓▓▓ ▐▓▓    ▓▓▓ ▓▓▓  ▓▓▓▄    ▓▓▓▓`
            '▀▓▓▓▄      ^▓▓▓  ▓▓▓       └▀▀▀▀ ▀▀ ^▀▀    `▀▀ `▀▀   '▀▀    ▐▓▓▌
               ▀▀▀▀▓▄▄▄   ▓▓▓▓▓▓,                                      ▓▓▓▓▀
                   `▀█▓▓▓▓▓▓▓▓▓▌
                        ¬`▀▀▀█▓

        """
        )
    except Exception:
        print("\n\n\tUnity Technologies\n")

if __name__ == "__main__":
    # unityLogo()
    main()