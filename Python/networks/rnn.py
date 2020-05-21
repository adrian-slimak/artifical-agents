from networks.dense import DenseLayer
from time import clock
import numpy as np


class RNNCell:
    def __init__(self, input_dim, units, models, use_bias=False):
        self.input_dim = input_dim
        self.models = models
        self.units = units
        self.use_bias = use_bias

        self.kernel = None
        self.recurrent_kernel = None
        self.bias = None

    def build(self, kernel=None, recurrent_kernel=None, bias=None):

        if kernel is not None:
            self.kernel = kernel
        # else:
        #     self.kernel = tf.Variable(shape=(self.models, self.input_dim, self.units),
        #                                   name='kernel', initializer='random_normal')

        if recurrent_kernel is not None:
            self.recurrent_kernel = recurrent_kernel
        # else:
        #     self.recurrent_kernel = tf.Variable(shape=(self.models, self.units, self.units), dtype=tf.float32,
        #                                             name='recurrent_kernel', initializer='random_normal')

        if bias is not None:
            self.bias = bias
        else:
            # if self.use_bias:
            #     self.bias = tf.Variable(shape=(self.models, 1, self.units), dtype=tf.float32,
            #                                 name='bias', initializer='random_normal')
            # else:
                self.bias = None

        self.built = True

    def call(self, inputs, states):
        o_tm1 = states[0]  # previous output

        h = np.matmul(inputs, self.kernel, dtype=np.float32)

        if self.use_bias:
            h = np.add(h, self.bias, dtype=np.float32)

        o = np.tanh(h + np.matmul(o_tm1, self.recurrent_kernel, dtype=np.float32), dtype=np.float32)

        return o, [o]


class RNNModel:
    name = 'rnn'

    def __init__(self, input_dim, rnn_units, dense_units, models, batch_size=1, use_bias=True):
        self.rnn_units = rnn_units
        self.input_dim = input_dim
        self.models = models
        self.batch_size = batch_size

        self.rnn_cell = RNNCell(input_dim, rnn_units, models, use_bias)
        self.cell_states = None
        self.output_layer = DenseLayer(rnn_units, dense_units, models, use_bias)

    def build(self, model_weights=(None, None)):
        weights, biases = model_weights

        self.cell_states = [np.zeros((self.models, self.batch_size, self.rnn_units))]

        if weights is not None and biases is not None:
            self.rnn_cell.build(kernel=weights[0], recurrent_kernel=weights[1], bias=biases[0])
            self.output_layer.build(kernel=weights[2], bias=biases[1])
        elif weights is not None:
            self.rnn_cell.build(kernel=weights[0], recurrent_kernel=weights[1])
            self.output_layer.build(kernel=weights[2])
        else:
            self.rnn_cell.build()
            self.output_layer.build()

    def __call__(self, inputs):
        return self.call(inputs)

    def call(self, inputs):
        # if self.batch_size == 1:
        #     inputs = tf.reshape(inputs, (self.models, 1, self.input_dim))
        # else:
        # inputs = tf.reshape(inputs, (self.models, self.batch_size, self.input_dim))
        # HERE
        inputs = np.reshape(inputs, (self.batch_size, self.models, self.input_dim))
        inputs = np.transpose(inputs, (1, 0, 2))

        (output, states) = self.rnn_cell.call(inputs, self.cell_states)
        self.cell_states = states
        output = self.output_layer.call(output)
        output = np.transpose(output, (1, 0, 2))
        return output
