import json
from mlagents.communicator_objects.unity_initialization_input_pb2 import UnityInitializationInputProto
from mlagents.communicator_objects.engine_configuration_pb2 import EngineConfigurationProto
import tensorflow as tf
import re

def get_initialization_input(reset_parameters = None):
    engine_config = EngineConfigurationProto()
    engine_config.width = 800
    engine_config.height = 600
    engine_config.quality_level = 1
    engine_config.time_scale = 1
    engine_config.target_frame_rate = -1
    engine_config.show_monitor = False

    if reset_parameters is not None:
        reset_parameters = get_reset_parameters(reset_parameters)

    return UnityInitializationInputProto(seed=1, engine_configuration=engine_config, custom_reset_parameters=reset_parameters)

def load_custom_reset_parameters(fileName='custom_reset_params_1'):
    with open('configs\\' + fileName, 'r') as file:
        custom_reset_parameters = json.loads(file.read())

    # Remove commented (by '#') parameters
    list_keys = list(custom_reset_parameters.keys())
    for k in list_keys:
        if k.startswith('#'):
            custom_reset_parameters.pop(k)
    return custom_reset_parameters

def get_reset_parameters(json_dict):
    def stack_dict(old_dict):
        new_dict = {}
        for key, item in old_dict.items():
            if isinstance(item, dict):
                temp = stack_dict(item)
                for k, i in temp.items():
                    new_dict[f'{key}_{k}'] = i
            else:
                new_dict[key] = item

        return new_dict

    parameters = stack_dict(json_dict)

    for brain in json_dict.keys():
        observations_vector_size = 0
        for k, v in parameters.items():
            if re.match(f'{brain}_observations_\w+_vector_size', k):
                observations_vector_size += v
        parameters[f'{brain}_observations_vector_size'] = observations_vector_size

    return parameters

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

def setup_tensorflow():
    gpus = tf.config.experimental.list_physical_devices('GPU')
    if gpus:
      # Create N virtual GPUs with 256MB memory each
      try:
        tf.config.experimental.set_virtual_device_configuration(gpus[0], [tf.config.experimental.VirtualDeviceConfiguration(memory_limit=256)])
        # logical_gpus = tf.config.experimental.list_logical_devices('GPU')
        # print(len(gpus), "Physical GPU,", len(logical_gpus), "Logical GPUs")
      except RuntimeError as e:
        # Virtual devices must be set before GPUs have been initialized
        print(e)

# import tensorflow as tf
# gpus = tf.config.experimental.list_physical_devices('GPU')
# if gpus:
#   # Create 2 virtual GPUs with 1GB memory each
#   try:
#     tf.config.experimental.set_virtual_device_configuration(
#         gpus[0],
#         [tf.config.experimental.VirtualDeviceConfiguration(memory_limit=1024),
#          tf.config.experimental.VirtualDeviceConfiguration(memory_limit=1024)])
#     logical_gpus = tf.config.experimental.list_logical_devices('GPU')
#     print(len(gpus), "Physical GPU,", len(logical_gpus), "Logical GPUs")
#   except RuntimeError as e:
#     # Virtual devices must be set before GPUs have been initialized
#     print(e)