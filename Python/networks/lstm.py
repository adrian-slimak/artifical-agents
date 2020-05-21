# import tensorflow as tf
from networks.dense import DenseLayer
import numpy as np


class LSTMCell:
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
        #     self.kernel = tf.Variable(shape=(self.models, self.input_dim, self.units * 4),
        #                                   name='kernel', initializer='random_normal')

        if recurrent_kernel is not None:
            self.recurrent_kernel = recurrent_kernel
        # else:
        #     self.recurrent_kernel = tf.Variable(shape=(self.models, self.units, self.units * 4),
        #                                             name='recurrent_kernel', initializer='random_normal')

        if bias is not None:
            self.bias = bias
        else:
            # if self.use_bias:
            #     self.bias = tf.Variable(shape=(self.models, 1, self.units * 4),
            #                                 name='bias', initializer='random_normal')
            # else:
                self.bias = None

        self.built = True

    # @tf.function
    def call(self, inputs, states):
        h_tm1 = states[0]  # previous memory state
        c_tm1 = states[1]  # previous carry state

        z = np.matmul(inputs, self.kernel, dtype=np.float32)
        z += np.matmul(h_tm1, self.recurrent_kernel, dtype=np.float32)

        if self.use_bias:
            z = np.add(z, self.bias, dtype=np.float32)

        z0 = z[:, :, :self.units]
        z1 = z[:, :, self.units: 2 * self.units]
        z2 = z[:, :, 2 * self.units: 3 * self.units]
        z3 = z[:, :, 3 * self.units:]

        i = np.sigmoid(z0, dtype=np.float32)
        f = np.sigmoid(z1, dtype=np.float32)
        c = f * c_tm1 + i * np.tanh(z2, dtype=np.float32)
        o = np.sigmoid(z3, dtype=np.float32)

        h = o * np.tanh(c, dtype=np.float32)

        return h, [h, c]


class LSTMModel:
    name = 'lstm'

    def __init__(self, input_dim, lstm_units, dense_units, models, batch_size=1, use_bias=True):
        self.lstm_units = lstm_units
        self.input_dim = input_dim
        self.models = models
        self.batch_size = batch_size

        self.lstm_cell = LSTMCell(input_dim, lstm_units, models, use_bias)
        self.cell_states = None
        self.output_layer = DenseLayer(lstm_units, dense_units, models, use_bias)

    def build(self, model_weights=(None, None)):
        weights, biases = model_weights

        self.cell_states = [np.zeros((self.models, self.batch_size, self.lstm_units)),
                            np.zeros((self.models, self.batch_size, self.lstm_units))]

        if weights is not None and biases is not None:
            self.lstm_cell.build(kernel=weights[0], recurrent_kernel=weights[1], bias=biases[0])
            self.output_layer.build(kernel=weights[2], bias=biases[1])
        elif weights is not None:
            self.lstm_cell.build(kernel=weights[0], recurrent_kernel=weights[1])
            self.output_layer.build(kernel=weights[2])
        else:
            self.lstm_cell.build()
            self.output_layer.build()

        self.built = True

    def __call__(self, inputs):
        return self.call(inputs)

    # @tf.function
    def call(self, inputs):
        # if self.n_envs == 1:
        #     inputs = tf.reshape(inputs, (self.models, 1, self.input_dim))
        # else:
        # HERE
        inputs = np.reshape(inputs, (self.models, self.batch_size, self.input_dim))
        # inputs = tf.reshape(inputs, (self.batch_size, self.models, self.input_dim))
        # inputs = tf.transpose(inputs, [1, 0, 2])

        (output, states) = self.lstm_cell.call(inputs, self.cell_states)
        self.cell_states = states
        output = self.output_layer.call(output)
        output = np.transpose(output, (1, 0, 2))
        return output
