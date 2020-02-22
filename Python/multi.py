from GA import GeneticAlgorithm
import numpy as np

from mlagents.subprocess_env_manager import SubprocessEnvManager

from LivePlotting import LivePlot

unity_env_path = "C:/Users/adek1/Desktop/Env/ArtificalAnimals.exe"

prey_parameters = (15, 10, 2, 60)
# predator_parameters = (12, 10, 2, 4)
def main():
    livePlot = LivePlot(lines=[['max', 'avg']])

    numberOfEnvironments = 3
    env_manager = SubprocessEnvManager(numberOfEnvironments)

    prey_GA = GeneticAlgorithm(*prey_parameters)
    prey_GA.initial_population()

    # predator_GA = GeneticAlgorithm(*predator_parameters)
    # predator_GA.initial_population()

    for generation in range(1000):
        prey_model_weights = prey_GA.to_lstm_model()
        # predator_model_weights = predator_GA.to_lstm_model()

        models = {'prey': (prey_model_weights, prey_parameters)}#, 'predator': (predator_model_weights, predator_parameters)}

        all_fitness = env_manager.run_episode(models=models)

        prey_fitness = np.mean([fitness['prey'] for fitness in all_fitness.values()], axis=0)
        predator_fitness = np.mean([fitness['predator'] for fitness in all_fitness.values()], axis=0)

        max1, avg1 = prey_GA.calc_fitness(prey_fitness)
        # max2, avg2 = predator_GA.calc_fitness(predator_fitness)
        livePlot.update([[max1, avg1]])

        prey_GA.next_generation()
        # predator_GA.next_generation()

    env_manager.close()


if __name__ == "__main__":
    main()
