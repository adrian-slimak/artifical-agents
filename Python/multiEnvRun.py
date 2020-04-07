from mlagents.multi_unity_environment import MultiUnityEnvironment
import configs.learning_parameters as _lp
from other.LivePlotting import LivePlot
from networks.GA import GeneticAlgorithm
import numpy as np


def main():
    live_plot = LivePlot(plots=_lp.plot_structure, subplots=_lp.plot_subplots, figsize=_lp.plot_size) if _lp.show_plots else None

    unity_environment = MultiUnityEnvironment(file_path=_lp.unity_environment_path, number_of_environments=_lp.number_of_environments,
                                              engine_configuration=_lp.engine_configuration,
                                              environment_parameters=_lp.environment_parameters)

    GAs = {}
    for brain_name in _lp.brains:
        brain = unity_environment.external_brains[brain_name]
        GAs[brain_name] = GeneticAlgorithm(brain.observations_vector_size, _lp.units, brain.actions_vector_size, brain.agents_count, model_name=_lp.NetworkModel.name, use_bias=_lp.use_bias)
        GAs[brain_name].initial_population()

    for generation in range(_lp.number_of_generations):
        models = {}
        for brain_name in _lp.brains:
            brain = unity_environment.external_brains[brain_name]
            model_weights = GAs[brain_name].to_model()
            models[brain_name] = _lp.NetworkModel(brain.observations_vector_size, _lp.units, brain.actions_vector_size, brain.agents_count, n_envs=_lp.number_of_environments, use_bias=_lp.use_bias)
            models[brain_name].build(model_weights=model_weights)

        all_fitness = unity_environment.run_single_episode(models, _lp.number_of_steps)

        for brain_name in _lp.brains:
            fitness = np.mean(all_fitness[brain_name], axis=0)
            GAs[brain_name].calc_fitness(fitness)
            GAs[brain_name].next_generation()

            avg, max, min = np.average(fitness), np.max(fitness), np.min(fitness)
            print(f"AVG: {avg:.2f}   BEST: {max:.2f}   WORST: {min:.2f}")
            if live_plot:
                live_plot.update({f'{brain_name}1': [avg, max, min]})

    # unity_environment.close()
    if live_plot:
        live_plot.close()


if __name__ == "__main__":
    main()
