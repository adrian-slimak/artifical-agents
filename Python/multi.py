from GA import GeneticAlgorithm
import numpy as np

from mlagents.subprocess_env_manager import SubprocessEnvManager
from LSTM import LSTMModel

from LivePlotting import LivePlot

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

n_envs = 4

prey_parameters = (15, 8, 2, 60, n_envs, False)
prey_parameters1 = (15, 8, 2, 60, False)

def main():
    livePlot = LivePlot(plots={'prey': (['episode', 'fitness'], ['best', 'avg'])}, figsize=(6, 4))

    env_manager = SubprocessEnvManager(n_envs)

    prey_GA = GeneticAlgorithm(*prey_parameters1)
    prey_GA.initial_population()

    for generation in range(1000):
        prey_model_weights = prey_GA.to_lstm_model()

        prey_model = LSTMModel(*prey_parameters)
        prey_model.build(model_weights=prey_model_weights)

        models = {'prey': prey_model}

        all_fitness = env_manager.run_episode(models=models)

        prey_fitness = np.mean(all_fitness['prey'], axis=0)

        max1, avg1 = prey_GA.calc_fitness(prey_fitness)
        livePlot.update({'prey': [max1, avg1]})

        prey_GA.next_generation()

    env_manager.close()


if __name__ == "__main__":
    main()
