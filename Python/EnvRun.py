from mlagents.multi_unity_environment import MultiUnityEnvironment
import configs.learning_parameters as _lp
import configs.plots_parameters as _pp
from other.LivePlotting import LivePlot
from networks.GA import GeneticAlgorithm
import numpy as np


def main():
    live_plot = LivePlot(plots=_pp.plot_structure, subplots=_pp.plot_subplots, figsize=_pp.plot_size) if _lp.show_plots else None

    unity_environment = MultiUnityEnvironment(file_path=_lp.unity_environment_path, number_of_environments=_lp.number_of_environments,
                                              engine_configuration=_lp.engine_configuration,
                                              environment_parameters=_lp.environment_parameters)

    _brains = unity_environment.external_brains
    GAs = {}
    GAs['prey'] = GeneticAlgorithm(_brains['prey'].observations_vector_size, _lp.units, _brains['prey'].actions_vector_size, _brains['prey'].agents_count, model_name=_lp.NetworkModel.name, use_bias=_lp.use_bias)
    GAs['prey'].initial_population()
    GAs['predator'] = GeneticAlgorithm(_brains['predator'].observations_vector_size, _lp.units, _brains['predator'].actions_vector_size, _brains['predator'].agents_count * _lp.number_of_environments, model_name=_lp.NetworkModel.name, use_bias=_lp.use_bias)
    GAs['predator'].initial_population()

    for generation in range(_lp.number_of_generations):
        models = {}
        models['prey'] = _lp.NetworkModel(_brains['prey'].observations_vector_size, _lp.units, _brains['prey'].actions_vector_size, _brains['prey'].agents_count, n_envs=_lp.number_of_environments, use_bias=_lp.use_bias)
        models['prey'].build(GAs['prey'].to_model())

        models['predator'] = _lp.NetworkModel(_brains['predator'].observations_vector_size, _lp.units, _brains['predator'].actions_vector_size, _brains['predator'].agents_count * _lp.number_of_environments, n_envs=1, use_bias=_lp.use_bias)
        models['predator'].build(GAs['predator'].to_model())

        all_fitness = unity_environment.run_single_episode(models, _lp.number_of_steps, live_plot=live_plot)

        for brain_name in _lp.brains:
            if brain_name == 'prey':
                fitness = np.mean(all_fitness[brain_name], axis=0)
            else:
                fitness = all_fitness[brain_name]
            GAs[brain_name].calc_fitness(fitness)
            GAs[brain_name].next_generation()

            avg, max, min = np.average(fitness), np.max(fitness), np.min(fitness)
            print(f"AVG: {avg:.2f}   BEST: {max:.2f}   WORST: {min:.2f}")
            if live_plot:
                live_plot.update({f'{brain_name}1': [avg, max, min], f'{brain_name}2': None})

    # unity_environment.close()
    if live_plot:
        live_plot.close()


if __name__ == "__main__":
    main()
