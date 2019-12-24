from mlagents.envs.environment import UnityEnvironment
import tensorflow as tf
import numpy as np
# tf.debugging.set_log_device_placement(True)
from LSTM import LSTMModel
from time import sleep

def main():
    # tf_utils.set_warnings_enabled(False)
    # unity_environment = UnityEnvironment(file_name="C:/Users/adek1/Desktop/Agents/ArtificalAnimals.exe", worker_id=0)
    unity_environment = UnityEnvironment(worker_id=0)
    unity_environment.reset()

    env_state = unity_environment.step()
    prey_observations = env_state['prey']

    prey_model = LSTMModel(32, prey_observations.shape[0])
    prey_model.build(input_shape=(1, prey_observations.shape[1]))

    i=1
    while True:
        i+=1
        if i % 1000==0:
            unity_environment.reset()
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