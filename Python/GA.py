import configs.learning_parameters as _lp
import numpy as np
import random


def get_shapes_lengths(input_dim, units, output_dim, model='lstm', use_bias=True):
    shapes, lengths = None, None

    k = 4 if model == 'lstm' else 1

    if model == 'lstm' or model == 'rnn':
        if use_bias:
            shapes = [(input_dim, units * k), (units, units * k), (1, units * k), (units, output_dim), (1, output_dim)]
            lengths = [input_dim * units * k, units * units * k, units * k, units * output_dim, output_dim]
        else:
            shapes = [(input_dim, units * k), (units, units * k), (units, output_dim)]
            lengths = [input_dim * units * k, units * units * k, units * output_dim]

    return shapes, lengths

class Genotype:
    def __init__(self, shapes, lengths):
        self.shapes = shapes
        self.lengths = lengths
        self.genotype = np.zeros((sum(self.lengths)), dtype='f')
        self.fitness = None

    def copy(self):
        new_one = type(self)(self.shapes, self.lengths)
        new_one.genotype = self.genotype.copy()
        return new_one

    def random_init(self, min=_lp.init_min_genes, max=_lp.init_max_genes, loc=_lp.init_loc, scale=_lp.init_scale):
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
            s += length
        return numpies

class GeneticAlgorithm:
    def __init__(self, input_dim, units, output_dim, population_size, model='lstm', use_bias=True):
        self.population = []

        self.shapes, self.lengths = get_shapes_lengths(input_dim, units, output_dim, model, use_bias)

        self.population_size = population_size
        self.use_bias = use_bias

    def initial_population(self):
        self.population = [Genotype(self.shapes, self.lengths) for i in range(self.population_size)]
        for individual in self.population:
            individual.random_init()


    def calc_fitness(self, fitness):
        for idx, value in enumerate(fitness):
            self.population[idx].fitness = value

        # Sort population by fitness, best to worst order
        self.population = sorted(self.population, key=lambda individual: individual.fitness, reverse=True)

    def next_generation(self):
        selected_individuals = GeneticAlgorithm.selection(self.population, method=_lp.selection_method)

        parents = GeneticAlgorithm.pairing(selected_individuals)

        offsprings = [GeneticAlgorithm.mating(parents[x]) for x in range(len(parents))]
        offsprings = [individual for sublist in offsprings for individual in sublist]

        elite_individuals = self.population[:2]
        selected_individuals = selected_individuals[2:]

        next_gen = selected_individuals + offsprings
        for individual in next_gen:
            GeneticAlgorithm.mutation(individual)
        next_gen.extend(elite_individuals)

        if len(next_gen) != self.population_size:
            raise Exception("Next Gen size different than expected")

        self.population = next_gen

    @staticmethod
    def selection(population, method='Fittest Half'):
        if method == 'Fittest Half':
            selected_individuals = [population[i] for i in range(len(population) // 2)]
            return selected_individuals

        elif method == 'Roulette Wheel':
            fitness = [indiv.fitness for indiv in population]
            selected_individuals = random.choices(population, weights=fitness, k=len(population)//2)
            return selected_individuals

        else:
            raise Exception('Not such selection method found')

    @staticmethod
    def pairing(selected, method=_lp.pairing_method):
        individuals = selected
        parents = []

        if method == 'Fittest':
            parents = [[individuals[x], individuals[x + 1]] for x in range(len(individuals) // 2)]

        return parents

    @staticmethod
    def mating(parents, max_percent_length=_lp.max_percent_length, method=_lp.mating_method):
        offsprings = [parents[0].copy(), parents[1].copy()]

        if method == 'Two Points Per Part':
            partStart = 0
            for partLength in parents[0].lengths:
                l = int(random.uniform(0, max_percent_length) * (partLength-1))
                s = random.randrange(partStart, partStart + partLength)
                if s+l>=len(parents[0].genotype):
                    l-=(s+l)-len(parents[0].genotype)
                offsprings[0].genotype[s:s+l] = parents[1].genotype[s:s+l].copy()
                offsprings[1].genotype[s:s+l] = parents[0].genotype[s:s+l].copy()
                partStart += partLength

        elif method == 'Two Points':
            genotype_length = len(parents[0].genotype)
            l = int(random.uniform(0, max_percent_length) * genotype_length)
            s = random.randrange(0, genotype_length)
            if s + l >= len(parents[0].genotype):
                l -= (s+l)-len(parents[0].genotype)
            offsprings[0].genotype[s:s+l] = parents[1].genotype[s:s+l]
            offsprings[1].genotype[s:s+l] = parents[0].genotype[s:s+l]

        else:
            raise Exception('Not such mating method found')

        return offsprings

    @staticmethod
    def mutation(individual, gen_mutation_chance=_lp.gen_mutation_chance, gen_remove_chance=_lp.gen_remove_chance, gen_appear_chance=_lp.gen_appear_chance, sigma=_lp.gen_mutation_scale):
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
        weights = [[], [], []]
        biases = None if self.use_bias is False else [[], []]

        for individual in self.population:
            numpies = individual.to_numpy()
            weights[0].append(numpies[0])
            weights[1].append(numpies[1])
            if self.use_bias:
                biases[0].append(numpies[2])
                weights[2].append(numpies[3])
                biases[1].append(numpies[4])
            else:
                weights[2].append(numpies[2])

        weights = [np.array(weight, dtype='f') for weight in weights]
        if self.use_bias:
            biases = [np.array(bias, dtype='f') for bias in biases]

        return weights, biases
