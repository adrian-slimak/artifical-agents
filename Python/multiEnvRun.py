from GA import GeneticAlgorithm
import numpy as np

from mlagents.multi_env_manager import MultiEnvManager
from LSTM import LSTMModel

from LivePlotting import LivePlot

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

n_envs = 4

prey_parameters = (45, 8, 2, 60, n_envs, False)
prey_parameters1 = (45, 8, 2, 60, False)

def main():
    livePlot = LivePlot(plots={'prey': (['episode', 'fitness'], ['avg', 'best', 'worst'])}, figsize=(6, 4))

    env_manager = MultiEnvManager(n_envs)

    prey_GA = GeneticAlgorithm(*prey_parameters1)
    prey_GA.initial_population()

    for generation in range(1000):
        prey_model_weights = prey_GA.to_lstm_model()

        prey_model = LSTMModel(*prey_parameters)
        prey_model.build(model_weights=prey_model_weights)

        models = {'prey': prey_model}

        all_fitness = env_manager.run_episode(models=models)

        prey_fitness = np.mean(all_fitness['prey'], axis=0)

        prey_GA.calc_fitness(prey_fitness)
        avg, max, min = np.average(prey_fitness), np.max(prey_fitness), np.min(prey_fitness)
        livePlot.update({'prey': [avg, max, min]})

        prey_GA.next_generation()

    env_manager.close()


if __name__ == "__main__":
    main()
