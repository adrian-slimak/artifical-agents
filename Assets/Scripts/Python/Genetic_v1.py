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
        newone.genotype = self.genotype.copy()
        return newone

    def random_init(self, min=0.45, max=0.7, loc=0., scale=10.):
        percentOfGenes = random.uniform(min, max)
        numOfGenes = int(sum(self.lengths) * percentOfGenes)
        genesIdx = random.sample(range(0, sum(self.lengths) - 1), numOfGenes)
        genesVals = np.random.normal(loc=loc, scale=scale, size=(sum(self.lengths)))
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
    def __init__(self, input_dim, lstm_units, output_dim, population_size, use_bias=False):
        self.population = []
        if use_bias:
            self.shapes = [(input_dim, lstm_units * 4), (lstm_units, lstm_units * 4), (1, lstm_units * 4), (lstm_units, output_dim), (1, output_dim)]
            self.lengths = [input_dim * lstm_units * 4, lstm_units * lstm_units * 4, lstm_units * 4, lstm_units * output_dim, output_dim]
        else:
            self.shapes = [(input_dim, lstm_units * 4), (lstm_units, lstm_units * 4), (lstm_units, output_dim)]
            self.lengths = [input_dim * lstm_units * 4, lstm_units * lstm_units * 4, lstm_units * output_dim]

        self.population_size = population_size
        self.use_bias = use_bias

    def initial_population(self):
        self.population = [Genotype(self.shapes, self.lengths) for i in range(self.population_size)]
        for individual in self.population:
            individual.random_init(min=0.25, max=0.5, loc=0., scale=15.)

    def calc_fitness(self, fitness):
        for idx, value in enumerate(fitness):
            self.population[idx].fitness = value

        sorted_by_fitness = sorted(self.population, key=lambda individual: individual.fitness, reverse=True)
        self.population = sorted_by_fitness
        print(f"avg fitness: {np.average(fitness)}")
        print(f"best fitness: {np.max(fitness)}")
        return np.max(fitness), np.average(fitness)

    def next_generation(self):
        selected_individuals = GeneticAlgorithm.selection(self.population)

        parents = GeneticAlgorithm.pairing(selected_individuals)

        offsprings = [GeneticAlgorithm.mating(parents[x], maxPercentLength=0.2) for x in range(len(parents))]
        offsprings = [individual for sublist in offsprings for individual in sublist]

        elite_individuals = selected_individuals[:3]
        selected_individuals = selected_individuals[3:]

        next_gen = selected_individuals + offsprings
        for individual in next_gen:
            GeneticAlgorithm.mutation(individual, gen_mutation_chance=0.05, gen_remove_chance=0.01, gen_appear_chance=0.02, sigma=0.2)
        next_gen.extend(elite_individuals)

        if len(next_gen) != self.population_size:
            raise Exception("Next Gen size different than expected")

        self.population = next_gen

    @staticmethod
    def selection(population, method='Fittest Half'):
        if method == 'Fittest Half':
            selected_individuals = [population[i] for i in range(len(population) // 2)]
            return selected_individuals

    @staticmethod
    def pairing(selected, method='Fittest'):
        individuals = selected
        parents = []

        if method == 'Fittest':
            parents = [[individuals[x], individuals[x + 1]] for x in range(len(individuals) // 2)]

        return parents

    @staticmethod
    def mating(parents, maxPercentLength, method='Two Points Per Part'):
        offsprings = [parents[0].copy(), parents[1].copy()]

        if method == 'Two Points Per Part':
            partStart = 0
            for partLength in parents[0].lengths:
                l = int(random.uniform(0, maxPercentLength) * (partLength-1))
                s = random.randrange(partStart, partStart + partLength)
                if s+l>=len(parents[0].genotype):
                    l-=(s+l)-len(parents[0].genotype)
                offsprings[0].genotype[s:s+l] = parents[1].genotype[s:s+l].copy()
                offsprings[1].genotype[s:s+l] = parents[0].genotype[s:s+l].copy()
                partStart = partLength

        return offsprings

    @staticmethod
    def mutation(individual, gen_mutation_chance, gen_remove_chance, gen_appear_chance, sigma=1.):
        for id in range(len(individual.genotype)):
            if individual.genotype[id] == 0.:
                if random.random() <= gen_appear_chance:
                    individual.genotype[id] = random.gauss(mu=0., sigma=sigma)
            else:
                if random.random() <= gen_remove_chance:
                    individual.genotype[id] = 0.
                if random.random() <= gen_mutation_chance:
                    individual.genotype[id] += random.gauss(mu=0., sigma=sigma)

    def to_lstm_model(self):
        weights = [[],[],[]]
        biases = [[],[]]
        for indyvidual in self.population:
            numpies = indyvidual.to_numpy()
            weights[0].append(numpies[0])
            weights[1].append(numpies[1])
            if self.use_bias:
                biases[0].append(numpies[2])
                weights[2].append(numpies[3])
                biases[1].append(numpies[4])
            else:
                weights[2].append(numpies[2])

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