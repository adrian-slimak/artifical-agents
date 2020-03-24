from mlagents.environment import UnityEnvironment
import configs.learning_parameters as _lp
from networks.lstm import LSTMModel
import numpy as np
from GA import GeneticAlgorithm
from LivePlotting import LivePlot


def main():
    # livePlot = LivePlot(plots={'prey': (['episode', 'fitness'], ['avg', 'best', 'worst']), 'predator': (['episode', 'fitness'], ['avg', 'best', 'worst'])}, figsize=(7, 9))
    live_plot = LivePlot(plots={'prey': (['episode', 'fitness'], ['avg', 'best', 'worst'])}, figsize=(7, 4)) if _lp.show_plots else None

    unity_environment = UnityEnvironment(file_name=_lp.unity_environment_path, worker_id=0, initialization_input=_lp.initialization_input)

    GAs = {}
    for brain_name in _lp.brains:
        brain = unity_environment.external_brains[brain_name]
        GAs[brain_name] = GeneticAlgorithm(brain.observations_vector_size, _lp.units, brain.actions_vector_size, brain.agents_count, model=_lp.network_model, use_bias=_lp.use_bias)
        GAs[brain_name].initial_population()

    # Main Loop
    for generation in range(_lp.number_of_generations):
        models = {}
        for brain_name in _lp.brains:
            brain = unity_environment.external_brains[brain_name]
            model_weights = GAs[brain_name].to_lstm_model()
            models[brain_name] = _lp.NetworkModel(brain.observations_vector_size, _lp.units, brain.actions_vector_size, brain.agents_count, use_bias=_lp.use_bias)
            models[brain_name].build(model_weights=model_weights)

        all_fitness = unity_environment.run_single_episode(models, _lp.number_of_steps)

        for brain_name in _lp.brains:
            fitness = all_fitness[brain_name]
            GAs[brain_name].calc_fitness(fitness)
            GAs[brain_name].next_generation()

            avg, max, min = np.average(fitness), np.max(fitness), np.min(fitness)
            print(f"AVG: {avg:.2f}   BEST: {max:.2f}   WORST: {min:.2f}")
            if live_plot:
                live_plot.update({brain_name: [avg, max, min]})

    unity_environment.close()
    if live_plot:
        live_plot.close()


if __name__ == "__main__":
    main()
