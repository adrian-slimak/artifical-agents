# from LSTM import LSTMModel
import numpy as np
import random
from timeit import default_timer as timer

# self.lstm_kernel = np.ones((input_dim, lstm_units * 4))
# self.lstm_recurrent_kernel = np.ones((lstm_units, lstm_units * 4))
# self.lstm_bias = np.ones((1, lstm_units * 4))
# self.dense_kernel = np.ones((lstm_units, dense_units))
# self.dense_bias = np.ones((1, dense_units))

class Genotype():
    def __init__(self, shapes, lengths):
        self.shapes = shapes
        self.lengths = lengths
        self.genotype = np.zeros((sum(self.lengths)), dtype='f')
        self.fitness = None

    def copy(self):
        newone = type(self)(self.shapes, self.lengths)
        return newone

    def random_init(self, min, max):
        percentOfGenes = random.randrange(min, max)/100
        numOfGenes = int(sum(self.lengths) * percentOfGenes)
        genesIdx = random.sample(range(0, sum(self.lengths) - 1), numOfGenes)
        genesVals = np.random.normal(size=(sum(self.lengths) - 1))
        for id in genesIdx:
            self.genotype[id] = genesVals[id]

    def to_numpy(self):
        numpies = []
        s = 0
        for shape, length in zip(self.shapes, self.lengths):
            numpies.append(np.reshape(self.genotype[s:s+length], shape))
            s = length
        return numpies

class GeneticAlgorithm:
    def __init__(self, input_dim, lstm_units, output_dim, population_size, use_bias=True):
        self.population = []
        if use_bias:
            self.shapes = [(input_dim, lstm_units * 4), (lstm_units, lstm_units * 4), (1, lstm_units * 4), (lstm_units, output_dim), (1, output_dim)]
            self.lengths = [input_dim * lstm_units * 4, lstm_units * lstm_units * 4, lstm_units * 4, lstm_units * output_dim, output_dim]
        else:
            self.shapes = [(input_dim, lstm_units * 4), (lstm_units, lstm_units * 4), (lstm_units, output_dim)]
            self.lengths = [input_dim * lstm_units * 4, lstm_units * lstm_units * 4, lstm_units * output_dim]
        # self.shapes = [(1, input_dim)]
        # self.lengths = [input_dim]
        self.population_size = population_size

    def initial_population(self):
        self.population = [Genotype(self.shapes, self.lengths) for i in range(self.population_size)]
        for individual in self.population:
            individual.random_init(30, 70)

    def calc_fitness(self, fitness):
        for idx, value in enumerate(fitness):
            self.population[idx].fitness = value

        sorted_by_fitness = sorted(self.population, key=lambda individual: individual.fitness)
        self.population = sorted_by_fitness
        best = max(self.population, key=lambda individual: individual.fitness)
        fitness_list = [i.fitness for i in self.population]
        # print(f"avg fitness: {sum(fitness_list)/len(fitness_list)}")
        print(f"best fitness: {max(fitness_list)}")

    def next_generation(self):
        elit_individual = self.population.pop(-1)
        selected_individuals = GeneticAlgorithm.selection(self.population)

        parents = GeneticAlgorithm.pairing(elit_individual, selected_individuals)

        offsprings = [GeneticAlgorithm.mating(parents[x]) for x in range(len(parents))]
        offsprings = [individual for sublist in offsprings for individual in sublist]

        unmutated = selected_individuals + offsprings
        for individual in unmutated:
            GeneticAlgorithm.mutation(individual, 0.01, 0., 0.)
        mutated = unmutated

        next_gen = mutated + [elit_individual]
        self.population = next_gen

    @staticmethod
    def selection(population, method='Fittest Half'):
        if method == 'Fittest Half':
            selected_individuals = [population[-x - 1] for x in range(len(population) // 2)]
            return selected_individuals

    @staticmethod
    def pairing(elit, selected, method='Fittest'):
        individuals = [elit] + selected
        parents = []

        if method == 'Fittest':
            parents = [[individuals[x], individuals[x + 1]] for x in range(len(individuals) // 2)]

        return parents

    @staticmethod
    def mating(parents, maxPercentLength=60, method='Two Points Per Part'):
        offsprings = [parents[0].copy(), parents[1].copy()]

        if method=='Two Points Per Part':
            partStart=0
            for partLength in parents[0].lengths:
                l = int(random.randrange(0, maxPercentLength)/100 * (partLength-1))
                s = random.randrange(partStart, partStart + partLength)
                if s+l>=len(parents[0].genotype):
                    l-=(s+l)-len(parents[0].genotype)
                offsprings[1].genotype[s:s+l] = parents[0].genotype[s:s+l].copy()
                offsprings[0].genotype[s:s+l] = parents[1].genotype[s:s+l].copy()
                partStart = partLength

        return offsprings

    @staticmethod
    def mutation(individual, gen_mutation_chance, gen_remove_chance, gen_appear_chance, standard_deviation=0.1):
        for id in range(len(individual.genotype)):
            if individual.genotype[id] == 0.:
                if random.random() <= gen_appear_chance:
                    individual.genotype[id] = random.gauss(mu=0., sigma=standard_deviation)
            else:
                if random.random() <= gen_remove_chance:
                    individual.genotype[id] = 0.
                if random.random() <= gen_mutation_chance:
                    individual.genotype[id] += random.gauss(mu=0., sigma=standard_deviation)

    def to_lstm_model(self):
        weights = [[],[],[]]
        biases = [[],[]]
        for indyvidual in self.population:
            numpies = indyvidual.to_numpy()
            weights[0].append(numpies[0])
            weights[1].append(numpies[1])
            biases[0].append(numpies[2])
            weights[2].append(numpies[3])
            biases[1].append(numpies[4])

        weights = [np.array(weight, dtype='f') for weight in weights]
        biases = [np.array(bias, dtype='f') for bias in biases]
        return weights, biases



# from LSTM import LSTMModel
#
# input_size = 20
# lstm_units = 32
# output_size = 2
# models = 100
# ga = GeneticAlgorithm(input_size, lstm_units, output_size, 100)
# ga.initial_population()
#
# start = timer()
# for i in range(1000):
#     # print(i)
#     w, b = ga.to_lstm_model()
#     model = LSTMModel(lstm_units, output_size, models)
#     model.build((1,input_size), w, b)
#
#     output = model.call(np.ones((models, 1, input_size), dtype='f'))
#     fitness = np.sum(output, axis=1)
#     ga.calc_fitness(fitness)
#     ga.next_generation()
# print(timer() - start)