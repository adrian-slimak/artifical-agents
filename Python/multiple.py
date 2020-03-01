from mlagents.environment import UnityEnvironment
import tensorflow as tf
# tf.debugging.set_log_device_placement(True)
from LSTM import LSTMModel
import numpy as np
from GA import GeneticAlgorithm
from LivePlotting import LivePlot
from utils import get_initialization_input
import configs.custom_reset_parameters as custom_params
tf.random.set_seed(1000)

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

LSTM_units = 8
num_steps = 1000

def main():
    # tf_utils.set_warnings_enabled(False)
    # livePlot = LivePlot(plots={'prey': (['episode', 'fitness'], ['best', 'avg'])})

    unity_environment0 = UnityEnvironment(file_name=unity_env_path, worker_id=0, initialization_input=get_initialization_input(custom_params.custom_reset_parameters_1))
    unity_environment1 = UnityEnvironment(file_name=unity_env_path, worker_id=1, initialization_input=get_initialization_input(custom_params.custom_reset_parameters_1))
    unity_environment2 = UnityEnvironment(file_name=unity_env_path, worker_id=2, initialization_input=get_initialization_input(custom_params.custom_reset_parameters_1))

    prey_brain = unity_environment0.external_brains['prey']

    prey_GA = GeneticAlgorithm(prey_brain.observations_vector_size, LSTM_units, prey_brain.actions_vector_size, prey_brain.agents_count)
    prey_GA.initial_population()

    for generation in range(1000):
        prey_model_weights = prey_GA.to_lstm_model()

        prey_model0 = LSTMModel(prey_brain.observations_vector_size, LSTM_units, prey_brain.actions_vector_size, prey_brain.agents_count, use_bias=False)
        prey_model0.build(model_weights=prey_model_weights)

        prey_model1 = LSTMModel(prey_brain.observations_vector_size, LSTM_units, prey_brain.actions_vector_size, prey_brain.agents_count, use_bias=False)
        prey_model1.build(model_weights=prey_model_weights)

        prey_model2 = LSTMModel(prey_brain.observations_vector_size, LSTM_units, prey_brain.actions_vector_size, prey_brain.agents_count, use_bias=False)
        prey_model2.build(model_weights=prey_model_weights)

        prey_model = LSTMModel(prey_brain.observations_vector_size, LSTM_units, prey_brain.actions_vector_size, prey_brain.agents_count, envs=3, use_bias=False)
        prey_model.build(model_weights=prey_model_weights)

        # Single Episode Loop
        unity_environment0.reset(None)
        unity_environment1.reset(None)
        unity_environment2.reset(None)

        for step in range(num_steps):
            agent_observations0 = unity_environment0.step_receive_observations()
            agent_observations1 = unity_environment1.step_receive_observations()
            agent_observations2 = unity_environment2.step_receive_observations()

            agent_observations = tf.concat([agent_observations0['prey'], agent_observations1['prey'], agent_observations2['prey']], axis=1)

            prey_actions0 = prey_model0(agent_observations0['prey'])
            prey_actions1 = prey_model1(agent_observations1['prey'])
            prey_actions2 = prey_model2(agent_observations2['prey'])

            prey_actions = prey_model(agent_observations)

            unity_environment0.step_send_actions({'prey': prey_actions0})
            unity_environment1.step_send_actions({'prey': prey_actions1})
            unity_environment2.step_send_actions({'prey': prey_actions2})

        fitness0 = unity_environment0.episode_completed()
        fitness1 = unity_environment1.episode_completed()
        fitness2 = unity_environment2.episode_completed()
        # End Single Episode Loop


        max1, avg1 = prey_GA.calc_fitness(fitness0['prey'])
        # livePlot.update({'prey': [max1, avg1]})
        # prey_GA.next_generation()

    unity_environment.close()
    # livePlot.close()


if __name__ == "__main__":
    main()
