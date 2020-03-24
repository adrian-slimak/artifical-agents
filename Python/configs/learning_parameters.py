from configs.environment_parameters import environment_parameters as _envparams
from utils import stick_reset_parameters, get_initialization_input

from networks.lstm import LSTMModel
from networks.rnn import RNNModel

# ENVIRONMENT PARAMETERS
unity_environment_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

environment_parameters = _envparams
merged_environment_parameters = stick_reset_parameters(environment_parameters)

initialization_input = get_initialization_input(_envparams)

brains = [brain_name for brain_name in _envparams if ('count' in _envparams[brain_name] and _envparams[brain_name]['count'] > 0)]

# GENERAL
network_model = 'rnn'
NetworkModel = None

units = 16
use_bias = True

number_of_generations = 1000
number_of_steps = 2000

show_plots = True

# MULTI
number_of_environments = 4

# GENETIC ALGORITHM PARAMETERS
# Random Init
init_min_genes = 0.5
init_max_genes = 0.75
init_loc = 0.
init_scale = 1.

# Selection
# _selectionMethod = 'Roulette Wheel'
selection_method = 'Fittest Half'

# Mating
mating_method = 'Two Points Per Part'
# _matingMethod = 'Two Points'
max_percent_length = 0.7

# Pairing
pairing_method = 'Fittest'

# Mutation
gen_mutation_chance = 0.04
gen_remove_chance = 0.015
gen_appear_chance = 0.03
gen_mutation_scale = 0.4


# CALCULATIONS

if network_model == 'lstm':
    NetworkModel = LSTMModel
if network_model == 'rnn':
    NetworkModel = RNNModel