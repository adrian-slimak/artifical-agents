from LSTM import LSTMModel
import numpy as np
import random

# self.lstm_kernel = np.ones((input_dim, lstm_units * 4))
# self.lstm_recurrent_kernel = np.ones((lstm_units, lstm_units * 4))
# self.lstm_bias = np.ones((1, lstm_units * 4))
# self.dense_kernel = np.ones((lstm_units, dense_units))
# self.dense_bias = np.ones((1, dense_units))

class Genotype():
    def __init__(self, input_dim, lstm_units, dense_units, use_bias=True):
        if use_bias:
            self.shapes = [(input_dim, lstm_units * 4), (lstm_units, lstm_units * 4), (1, lstm_units * 4), (lstm_units, dense_units), (1, dense_units)]
            self.genotype = [[],[],[],[],[]]
        else:
            self.shapes = [(input_dim, lstm_units * 4), (lstm_units, lstm_units * 4), (lstm_units, dense_units)]
            self.genotype = [[],[],[]]

    def random_init(self, min, max):
        percentOfGenes = random.randrange(min, max)/100
        for part, shape in zip(self.genotype, self.shapes):
            numOfGenes = int(np.prod(shape) * percentOfGenes)
            genesId = random.sample(range(0, np.prod(shape)-1), numOfGenes)
            # genesX = random.sample(range(0, sum(shape[0])-1), numOfGenes)
            # genesY = random.sample(range(0, sum(shape[1])-1), numOfGenes)
            for id in genesId:
                part.append((id, random.random()))

    def to_weights(self):
        weights = []
        for shape, genes in zip(self.shapes, self.genotype):
            weights.append(np.zeros(shape))
            for gen in genes:
                x, y = int(gen[0] / shape[1]), gen[0] % shape[1]
                if shape[0]==1:
                    x=0
                weights[-1][x][y] = gen[1]
        return weights

class GeneticAlgorithm:
    def __init__(self, populationSize, input_dim, lstm_units, dense_units):
        self.population = [Genotype(input_dim, lstm_units, dense_units) for i in range(populationSize)]
        for object in self.population:
            object.random_init(40, 80)
        self.fitness = None

    # def to_lstm_model:
    #     weights = []
    #     for object in self.population:


# models = 10
# model = LSTMModel(units=32, output_dim=2, numOfSubModels=models)
#
# input_shape = (models, 1, 100)
# inputs = np.ones(input_shape, dtype='f')
# # for i in range(input_shape[0]):
# #     inputs[i,:,:] *= 15
#
# model.build(input_shape)
# output = model.call(inputs)
# print(output)