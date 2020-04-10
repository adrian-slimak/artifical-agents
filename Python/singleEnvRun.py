from mlagents.unity_environment import UnityEnvironment
import configs.learning_parameters as _lp
import configs.plots_parameters as _pp
from other.live_plot import LivePlot
from networks.GA import GeneticAlgorithm
import numpy as np


def main():
    live_plot = LivePlot(plots=_pp.plot_structure, subplots=_pp.plot_subplots, figsize=_pp.plot_size) if _lp.show_plots else None

    unity_environment = UnityEnvironment(file_path=None, worker_id=0,
                                         engine_configuration=_lp.engine_configuration,
                                         environment_parameters=_lp.environment_parameters)

    GAs = {}
    for brain_name in _lp.brains:
        brain = unity_environment.external_brains[brain_name]
        GAs[brain_name] = GeneticAlgorithm(brain.observations_vector_size, _lp.units, brain.actions_vector_size, brain.agents_count, model_name=_lp.NetworkModel.name, use_bias=_lp.use_bias)
        GAs[brain_name].initial_population()

    # Main Loop
    for generation in range(_lp.number_of_generations):
        models = {}
        for brain_name in _lp.brains:
            brain = unity_environment.external_brains[brain_name]
            model_weights = GAs[brain_name].to_model()
            models[brain_name] = _lp.NetworkModel(brain.observations_vector_size, _lp.units, brain.actions_vector_size, brain.agents_count, use_bias=_lp.use_bias)
            models[brain_name].build(model_weights=model_weights)

        all_fitness = unity_environment.run_single_episode(models, _lp.number_of_steps, live_plot=live_plot)

        for brain_name in _lp.brains:
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
