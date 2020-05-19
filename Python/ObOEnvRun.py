from mlagents.multi_unity_environment import MultiUnityEnvironment
import configs.learning_parameters as _lp
from networks.GA import GeneticAlgorithm
import configs.plots_parameters as _pp
from other.live_plot import LivePlot
import numpy as np
from random import shuffle
from other.utils import save_genes


def main():
    live_plot = LivePlot(plots=_pp.plot_structure, subplots=_pp.plot_subplots, figsize=_pp.plot_size) if _lp.show_plots else None

    unity_environment = MultiUnityEnvironment(file_path=_lp.unity_environment_path,
                                              number_of_environments=_lp.number_of_environments,
                                              engine_configuration=_lp.engine_configuration,
                                              environment_parameters=_lp.environment_parameters)

    _brains = unity_environment.external_brains
    GAs = {}
    GAs['prey'] = GeneticAlgorithm(_brains['prey'].observations_vector_size, _lp.units, _brains['prey'].actions_vector_size, _lp.population_size, model_name=_lp.NetworkModel.name, use_bias=_lp.use_bias)
    GAs['prey'].initial_population()
    GAs['predator'] = GeneticAlgorithm(_brains['predator'].observations_vector_size, _lp.units, _brains['predator'].actions_vector_size, _lp.population_size, model_name=_lp.NetworkModel.name, use_bias=_lp.use_bias)
    GAs['predator'].initial_population()
    # GAs['predator'].load_population_from_file('genes_5.pkl')

    _lp.hotkey_listener.add('<ctrl>+<alt>+a', lambda: save_genes(GAs, _lp.save_ID))
    # SPRAWDZIĆ Z JAKIEGO KĄTA NAJCZĘŚCIEJ ATAKUJĄ DRAPIEŻCY
    # SPRAWDZAĆ RÓŻNE WARIANTY POSZUKIWANIA POŻYWIENIA. GENEROWAĆ TYLKO W JENDYM MIEJSCU I PO JAKIMŚ CZASIE DOPIERO. MOZE AGENCI ZACZNĄ SIĘ KOMUNIKOWAĆ I INFORMOWAĆ O POŻYWIENIU.

    for generation in range(_lp.number_of_generations):
        prey_weights = GAs['prey'].to_model()
        predator_weights = GAs['predator'].to_model()

        preys_to_evaluate = list(range(_lp.population_size))
        shuffle(preys_to_evaluate)
        predators_to_evaluate = list(range(_lp.population_size))
        shuffle(predators_to_evaluate)
        all_fitness = {'prey': np.zeros((_lp.population_size, _brains['prey'].agents_count)), 'predator': np.zeros((_lp.population_size, _brains['predator'].agents_count))}
        for i in range(int(_lp.population_size/_lp.number_of_environments)):
            models = {}
            models['prey'] = _lp.NetworkModel(_brains['prey'].observations_vector_size, _lp.units, _brains['prey'].actions_vector_size, _lp.number_of_environments, batch_size=_brains['prey'].agents_count, use_bias=_lp.use_bias)
            models['predator'] = _lp.NetworkModel(_brains['predator'].observations_vector_size, _lp.units, _brains['predator'].actions_vector_size, _lp.number_of_environments, batch_size=_brains['predator'].agents_count, use_bias=_lp.use_bias)

            n, n1 = i*_lp.number_of_environments, (i+1)*_lp.number_of_environments
            current_preys = preys_to_evaluate[n:n1]
            current_predators = predators_to_evaluate[n:n1]
            models['prey'].build(([prey_weights[0][0][current_preys], prey_weights[0][1][current_preys], prey_weights[0][2][current_preys]], [prey_weights[1][0][current_preys], prey_weights[1][1][current_preys]]))
            models['predator'].build(([predator_weights[0][0][current_predators], predator_weights[0][1][current_predators], predator_weights[0][2][current_predators]], [predator_weights[1][0][current_predators], predator_weights[1][1][current_predators]]))
            fitness = unity_environment.run_single_episode(models, _lp.number_of_steps, live_plot=live_plot)

            all_fitness['prey'][current_preys] = fitness['prey']
            all_fitness['predator'][current_predators] = fitness['predator']

        for brain_name in _lp.brains:
            fitness = np.mean(all_fitness[brain_name], axis=1)

            GAs[brain_name].calc_fitness(fitness)

            avg, max, min = np.average(fitness), np.max(fitness), np.min(fitness)
            if live_plot:
                live_plot.update({f'{brain_name}1': [avg, max, min], f'{brain_name}2': None})

        GAs['prey'].next_generation()
        GAs['predator'].next_generation()

    # unity_environment.close()
    if live_plot:
        live_plot.close()


if __name__ == "__main__":
    main()
