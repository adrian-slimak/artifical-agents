from GA import GeneticAlgorithm
import numpy as np

from mlagents.subprocess_env_manager import SubprocessEnvManager

from LivePlotting import LivePlot

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"


def main():
    numberOfEnvironments = 2
    env_manager = SubprocessEnvManager(numberOfEnvironments)

    prey_GA = GeneticAlgorithm(15, 10, 2, 60)
    prey_GA.initial_population()

    livePlot = LivePlot(lines=['max', 'avg'])

    for generation in range(1000):
        preys_weights, _ = prey_GA.to_lstm_model()

        all_fitness = env_manager.run_episode(preys_weights)

        prey_fitness = [fitness['prey'] for fitness in all_fitness.values()]
        prey_fitness = np.mean(prey_fitness, axis=0)

        max, avg = prey_GA.calc_fitness(prey_fitness)
        livePlot.update([max,avg])

        prey_GA.next_generation()

    env_manager.close()


if __name__ == "__main__":
    main()
