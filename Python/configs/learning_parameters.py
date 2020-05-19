from configs.environment_parameters import environment_parameters as environment_parameters
from configs.engine_configuration import engine_config as engine_configuration
from other.hotkey_listener import HotKeyListener
from networks.lstm import LSTMModel
from networks.rnn import RNNModel
from networks.dense import MLPModel
from other.utils import save_parameters, current_save_ID


save_ID = current_save_ID()
hotkey_listener = HotKeyListener()
hotkey_listener.add('<ctrl>+<alt>+a', lambda: save_parameters(save_ID))


# ENVIRONMENT PARAMETERS
unity_environment_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"
# unity_environment_path = None

brains = [brain_name for brain_name in environment_parameters if ('count' in environment_parameters[brain_name] and environment_parameters[brain_name]['count'] > 0)]

show_plots = True

# GENERAL
NetworkModel = RNNModel

units = 8
use_bias = True

number_of_generations = 500
number_of_steps = 4000

# MULTI
number_of_environments = 4

# GENETIC ALGORITHM PARAMETERS
# Population
population_size = 60

# Random Init
init_min_genes = 0.50
init_max_genes = 0.75
init_loc = 0.
init_scale = 1.

# Selection
# selection_method = 'Roulette Wheel'
selection_method = 'Fittest Half'

# Mating
mating_method = 'Two Points Per Part'
# mating_method = 'Two Points'
max_percent_length = 0.6

# Pairing
pairing_method = 'Fittest'

# Mutation
gen_mutation_chance = 0.03
gen_remove_chance = 0.01
gen_appear_chance = 0.03
gen_mutation_scale = 0.3
