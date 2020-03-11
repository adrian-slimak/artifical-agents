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

BRAINS = ['prey', 'predator']

def main():
    # tf_utils.set_warnings_enabled(False)
    # livePlot = LivePlot(plots={'prey': (['episode', 'fitness'], ['best', 'avg']), 'predator': (['episode', 'fitness'], ['best', 'avg'])}, figsize=(7, 9))

    unity_environment = UnityEnvironment(file_name=None, worker_id=0, initialization_input=get_initialization_input(custom_params.custom_reset_parameters_1))

    GAs = {}
    for brain_name in BRAINS:
        brain = unity_environment.external_brains[brain_name]
        GAs[brain_name] = GeneticAlgorithm(brain.observations_vector_size, LSTM_units, brain.actions_vector_size, brain.agents_count, use_bias=False)
        GAs[brain_name].initial_population()

    # Main Loop
    for generation in range(1000):
        Models = {}
        for brain_name in BRAINS:
            brain = unity_environment.external_brains[brain_name]
            model_weights = GAs[brain_name].to_lstm_model()
            Models[brain_name] = LSTMModel(brain.observations_vector_size, LSTM_units, brain.actions_vector_size, brain.agents_count, use_bias=False)
            Models[brain_name].build(model_weights=model_weights)

        all_fitness = unity_environment.run_single_episode(Models, 1000)

        for brain_name in BRAINS:
            max, avg = GAs[brain_name].calc_fitness(all_fitness[brain_name])
            GAs[brain_name].next_generation()
            # livePlot.update({brain_name: [max, avg]})

    unity_environment.close()
    # livePlot.close()


if __name__ == "__main__":
    main()
