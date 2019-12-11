from mlagents.envs.environment import UnityEnvironment
import tensorflow as tf
import numpy as np
# tf.debugging.set_log_device_placement(True)
from LSTM import LSTMModel
from time import sleep

from timeit import default_timer as timer

# # @tf.function
# def createLSTMCell(units=32, input_shape=(1, 150)):
#     W = np.ones((units, units * 4))
#     U = np.ones((input_shape[1], units * 4))
#     b = np.ones((units * 4,))
#     cell = tf.keras.layers.LSTMCell(units=32)
#     cell.build(input_shape)
#     cell.set_weights([U, W, b])
#     return cell

# def createModels():
#     global brainNames, models, model_states
#     start = timer()
#     for brainName in brainNames:
#         cell = createLSTMCell()
#         state = [tf.zeros([1, 32]), tf.zeros([1, 32])]
#         models[brainName] = cell
#         model_states[brainName] = state
#     end = timer()
#     print("Models created in %.2f seconds" % (end - start))
# @tf.function
# def stepLSTMCells():
#     global models, model_states
#     model_outputs = {}
#     for brainName in brainNames:
#         (output, state) = models[brainName](input, model_states[brainName], training=False)
#         model_states[brainName] = state
#         model_outputs[brainName] = output
#         # tf.print(output)
#         # tf.py_function(func=assign, inp=[brainName, output])
#     return model_outputs

# def createStackedLSTM(models=200, units=32, input_shape=(1, 150)):
#     kernel = tf.constant(np.ones((models, input_shape[1], units * 4)))
#     recurrent_kernel = tf.constant(np.ones((models, units, units * 4)))
#     bias = tf.constant(np.ones((models, 1, units * 4)))
#     cell = StackedLSTMCell(models=models, units=units)
#     cell.build(input_shape)
#     cell.set_weights([kernel, recurrent_kernel, bias])
#     return cell
#
# @tf.function
# def stepLSTM(model, inputs):
#     (output, states) = model(inputs)
#     return states

brainNames = []

def main():
    # global brainNames
    # tf_utils.set_warnings_enabled(False)
    unity_environment = UnityEnvironment(worker_id=0, base_port=5004)
    unity_environment.reset()

    # brainNames = unity_environment.external_brain_names

    env_state = unity_environment.step()
    prey_observations = env_state['prey'].vector_observations

    prey_model = LSTMModel(32, prey_observations.shape[0])
    prey_model.build(input_shape=(1, prey_observations.shape[1]))

    while True:
        # start = timer()

        output = prey_model(tf.expand_dims(prey_observations, 1))
        action = {'prey': output.numpy()}
        env_state = unity_environment.step(action)
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
