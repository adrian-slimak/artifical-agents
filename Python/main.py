from mlagents.environment import UnityEnvironment
import tensorflow as tf
# tf.debugging.set_log_device_placement(True)
from LSTM import LSTMModel
from GA import GeneticAlgorithm
from LivePlotting import LivePlot
from utils import load_custom_reset_parameters, get_initialization_input
import configs.custom_reset_parameters as custom_params

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

LSTM_units = 8

def main():
    # tf_utils.set_warnings_enabled(False)
    livePlot = LivePlot(labels={'prey': ['episode', 'fitness']}, lines={'prey': ['best', 'avg']})

    unity_environment = UnityEnvironment(file_name=unity_env_path, worker_id=0, initialization_input=get_initialization_input())
    unity_environment.reset(custom_params.custom_reset_parameters_1)

    external_brain = unity_environment.get_external_brains()
    prey_brain = external_brain['prey']
    # predator_brain = external_brain['predator']

    prey_GA = GeneticAlgorithm(prey_brain.observation_vector_size, LSTM_units, prey_brain.action_vector_size, prey_brain.agents_count)
    prey_GA.initial_population()

    # predator_GA = GeneticAlgorithm(predator_brain.observation_vector_size, LSTM_units, predator_brain.action_vector_size, predator_brain.agents_count)
    # predator_GA.initial_population()

    for generation in range(1000):
        prey_model_weights = prey_GA.to_lstm_model()
        # predator_model_weights = predator_GA.to_lstm_model()

        prey_model = LSTMModel(LSTM_units, prey_brain.action_vector_size, prey_brain.agents_count)
        prey_model.build(input_shape=(1, prey_brain.observation_vector_size), model_weights=prey_model_weights)

        # predator_model = LSTMModel(LSTM_units, predator_brain.action_vector_size, predator_brain.agents_count)
        # predator_model.build(input_shape=(1, predator_brain.observation_vector_size), model_weights=predator_model_weights)

        all_fitness = unity_environment.run_single_episode({"prey": prey_model}, 1000, custom_params.custom_reset_parameters_1)

        max1, avg1 = prey_GA.calc_fitness(all_fitness['prey'])
        # max2, avg2 = predator_GA.calc_fitness(all_fitness['predator'])
        livePlot.update({'prey': [max1, avg1]})
        prey_GA.next_generation()
        # predator_GA.next_generation()

    unity_environment.close()
    livePlot.close()


if __name__ == "__main__":
    main()
