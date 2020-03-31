from configs.environment_parameters import environment_parameters as _envparams
from configs.engine_configuration import engine_config as _engconf
from configs.plots_parameters import plot_structure, plot_subplots, plot_size
from hotkeysLogger import HotKeyListener

from networks.lstm import LSTMModel
from networks.rnn import RNNModel
from networks.dense import MLPModel

# ENVIRONMENT PARAMETERS
unity_environment_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

environment_parameters = _envparams
engine_configuration = _engconf

brains = [brain_name for brain_name in environment_parameters if ('count' in _envparams[brain_name] and _envparams[brain_name]['count'] > 0)]

# GENERAL
NetworkModel = LSTMModel

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
selection_method = 'Roulette Wheel'
# selection_method = 'Fittest Half'

# Mating
mating_method = 'Two Points Per Part'
# mating_method = 'Two Points'
max_percent_length = 0.7

# Pairing
pairing_method = 'Fittest'

# Mutation
gen_mutation_chance = 0.04
gen_remove_chance = 0.015
gen_appear_chance = 0.03
gen_mutation_scale = 0.4