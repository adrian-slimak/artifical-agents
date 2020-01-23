from mlagents.envs.environment import UnityEnvironment
# tf.debugging.set_log_device_placement(True)
from LSTM import LSTMModel
from Genetic_v1 import GeneticAlgorithm
from timeit import default_timer as timer
from time import sleep
import pickle
import json
from mlagents.envs.communicator_objects.unity_initialization_input_pb2 import UnityInitializationInputProto
from mlagents.envs.communicator_objects.engine_configuration_pb2 import EngineConfigurationProto

def get_initialization_input():
    engine_config = EngineConfigurationProto()
    engine_config.width = 1080
    engine_config.height = 720
    engine_config.quality_level = 1
    engine_config.time_scale = 1
    engine_config.target_frame_rate = 60
    engine_config.show_monitor = False
    return UnityInitializationInputProto(seed=1, engine_configuration=engine_config)

def load_custom_reset_parameters(fileName='custom_reset_params_1'):
    with open('configs\\' + fileName, 'r') as file:
        custom_reset_parameters = json.loads(file.read())

    # Remove commented (by '#') parameters
    list_keys = list(custom_reset_parameters.keys())
    for k in list_keys:
        if k.startswith('#'):
            custom_reset_parameters.pop(k)
    return custom_reset_parameters

def main():
    # tf_utils.set_warnings_enabled(False)
    # unity_environment = UnityEnvironment(file_name="C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe", worker_id=0, initialization_input=get_initialization_input())
    unity_environment = UnityEnvironment(worker_id=0, initialization_input=get_initialization_input())
    unity_environment.reset(load_custom_reset_parameters())

    external_brain = unity_environment.get_external_brains()
    preys_brain = external_brain['prey']

    GA = GeneticAlgorithm(preys_brain.observation_vector_size, 16, preys_brain.action_vector_size, preys_brain.agents_count)
    GA.initial_population()

    for generation in range(1000):
        weights, _ = GA.to_lstm_model()
        with open('data/weights.pickle', 'wb') as file:
            pickle.dump(weights, file)
        # with open('data/biases.pickle', 'wb') as file:
        #     pickle.dump(biases, file)
        prey_model = LSTMModel(16, preys_brain.action_vector_size, preys_brain.agents_count)
        prey_model.build(input_shape=(1, preys_brain.observation_vector_size), weights=weights)#, biases=biases)

        fitness = unity_environment.run_single_episode({"prey": prey_model}, 1000, load_custom_reset_parameters())

        GA.calc_fitness(fitness['prey'])
        GA.next_generation()

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
