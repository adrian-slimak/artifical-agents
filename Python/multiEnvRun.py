from mlagents.multi_env_manager import MultiEnvManager
import configs.learning_parameters as _lp
from LivePlotting import LivePlot
from GA import GeneticAlgorithm
import numpy as np


def main():
    live_plot = LivePlot(plots={'prey': (['episode', 'fitness'], ['avg', 'best', 'worst']), 'predator': (['episode', 'fitness'], ['avg', 'best', 'worst'])}, figsize=(6, 9)) if _lp.show_plots else None

    env_manager = MultiEnvManager(_lp.number_of_environments)

    GAs = {}
    for brain_name in _lp.brains:
        brain = env_manager.environments[0].external_brains[brain_name]
        GAs[brain_name] = GeneticAlgorithm(brain.observations_vector_size, _lp.units, brain.actions_vector_size, brain.agents_count, model_name=_lp.NetworkModel.name, use_bias=_lp.use_bias)
        GAs[brain_name].initial_population()

    for generation in range(_lp.number_of_generations):
        models = {}
        for brain_name in _lp.brains:
            brain = env_manager.environments[0].external_brains[brain_name]
            model_weights = GAs[brain_name].to_lstm_model()
            models[brain_name] = _lp.NetworkModel(brain.observations_vector_size, _lp.units, brain.actions_vector_size, brain.agents_count, n_envs=_lp.number_of_environments, use_bias=_lp.use_bias)
            models[brain_name].build(model_weights=model_weights)

        all_fitness = env_manager.run_episode(models, _lp.number_of_steps)

        for brain_name in _lp.brains:
            fitness = np.mean(all_fitness[brain_name], axis=0)
            GAs[brain_name].calc_fitness(fitness)
            GAs[brain_name].next_generation()

            avg, max, min = np.average(fitness), np.max(fitness), np.min(fitness)
            print(f"AVG: {avg:.2f}   BEST: {max:.2f}   WORST: {min:.2f}")
            if live_plot:
                live_plot.update({brain_name: [avg, max, min]})

    env_manager.close()
    if live_plot:
        live_plot.close()


if __name__ == "__main__":
    main()
