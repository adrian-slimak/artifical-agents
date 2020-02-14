from mlagents.environment import UnityEnvironment
import tensorflow as tf
tf.debugging.set_log_device_placement(True)
from LSTM import LSTMModel
from GA import GeneticAlgorithm
from LivePlotting import LivePlot
from utils import load_custom_reset_parameters, get_initialization_input

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

# gpus = tf.config.experimental.list_physical_devices('GPU')
# if gpus:
#   # Create 2 virtual GPUs with 1GB memory each
#   try:
#     tf.config.experimental.set_virtual_device_configuration( gpus[0], [tf.config.experimental.VirtualDeviceConfiguration(memory_limit=128)])
#     logical_gpus = tf.config.experimental.list_logical_devices('GPU')
#     print(len(gpus), "Physical GPU,", len(logical_gpus), "Logical GPUs")
#   except RuntimeError as e:
#     # Virtual devices must be set before GPUs have been initialized
#     print(e)

def main():
    # tf_utils.set_warnings_enabled(False)
    unity_environment = UnityEnvironment(file_name=unity_env_path, worker_id=0, initialization_input=get_initialization_input())
    unity_environment.reset(load_custom_reset_parameters())

    external_brain = unity_environment.get_external_brains()
    preys_brain = external_brain['prey']

    GA = GeneticAlgorithm(preys_brain.observation_vector_size, 8, preys_brain.action_vector_size, preys_brain.agents_count)
    GA.initial_population()

    # livePlot = LivePlot(lines=['max', 'avg'])

    for generation in range(1000):
        weights, _ = GA.to_lstm_model()

        prey_model = LSTMModel(8, preys_brain.action_vector_size, preys_brain.agents_count)
        prey_model.build(input_shape=(1, preys_brain.observation_vector_size), weights=weights)#, biases=biases)

        fitness = unity_environment.run_single_episode({"prey": prey_model}, 1000, load_custom_reset_parameters())['prey']

        max, avg = GA.calc_fitness(fitness)
        # livePlot.update([max,avg])
        GA.next_generation()

    unity_environment.close()


if __name__ == "__main__":
    main()
