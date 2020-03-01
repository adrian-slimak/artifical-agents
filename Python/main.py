from mlagents.environment import UnityEnvironment
# import tensorflow as tf
# tf.debugging.set_log_device_placement(True)
from LSTM import LSTMModel
from GA import GeneticAlgorithm
from LivePlotting import LivePlot
from utils import get_initialization_input
import configs.custom_reset_parameters as custom_params

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

LSTM_units = 8

def main():
    # tf_utils.set_warnings_enabled(False)
    # livePlot = LivePlot(plots={'prey': (['episode', 'fitness'], ['best', 'avg'])}, figsize=(7, 4))

    unity_environment = UnityEnvironment(file_name=unity_env_path, worker_id=0, initialization_input=get_initialization_input(custom_params.custom_reset_parameters_1))

    prey_brain = unity_environment.external_brains['prey']

    prey_GA = GeneticAlgorithm(prey_brain.observations_vector_size, LSTM_units, prey_brain.actions_vector_size, prey_brain.agents_count, use_bias=False)
    prey_GA.initial_population()

    for generation in range(1000):
        prey_model_weights = prey_GA.to_lstm_model()

        prey_model = LSTMModel(prey_brain.observations_vector_size, LSTM_units, prey_brain.actions_vector_size, prey_brain.agents_count, use_bias=False)
        prey_model.build(model_weights=prey_model_weights)

        all_fitness = unity_environment.run_single_episode({"prey": prey_model}, 1000)

        max1, avg1 = prey_GA.calc_fitness(all_fitness['prey'])
        # livePlot.update({'prey': [max1, avg1]})
        prey_GA.next_generation()

    unity_environment.close()
    # livePlot.close()


if __name__ == "__main__":
    main()
