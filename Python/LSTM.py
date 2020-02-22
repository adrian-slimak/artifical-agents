import tensorflow as tf
import numpy as np
from timeit import default_timer as timer

class StackedLSTMCell(tf.keras.layers.Layer):
    def __init__(self, units, models, use_bias=False, **kwargs):
        super(StackedLSTMCell, self).__init__(**kwargs)

        self.models = models
        self.units = units
        self.use_bias = use_bias

    def build(self, input_shape, kernel=None, recurrent_kernel=None, bias=None):
        input_dim = input_shape[-1]

        if kernel is not None:
            self.kernel = self.add_weight(shape=(self.models, input_dim, self.units * 4),
                                          name='kernel', initializer=tf.constant_initializer(kernel))
        else:
            self.kernel = self.add_weight(shape=(self.models, input_dim, self.units * 4),
                                          name='kernel', initializer='random_normal')

        if recurrent_kernel is not None:
            self.recurrent_kernel = self.add_weight(shape=(self.models, self.units, self.units * 4),
                                          name='recurrent_kernel', initializer=tf.constant_initializer(recurrent_kernel))
        else:
            self.recurrent_kernel = self.add_weight(shape=(self.models, self.units, self.units * 4),
                                                    name='recurrent_kernel', initializer='random_normal')

        if bias is not None:
            self.bias = self.add_weight(shape=(self.models, 1, self.units * 4),
                                          name='bias', initializer=tf.constant_initializer(bias))
        else:
            if self.use_bias:
                self.bias = self.add_weight(shape=(self.models, 1, self.units * 4),
                                            name='bias', initializer='random_normal')
            else:
                self.bias = None

        self.built = True

    @tf.function
    def call(self, inputs, states):
        h_tm1 = states[0]  # previous memory state
        c_tm1 = states[1]  # previous carry state

        z = tf.matmul(inputs, self.kernel)
        z += tf.matmul(h_tm1, self.recurrent_kernel)

        if self.use_bias:
            z = tf.add(z, self.bias)

        z0 = z[:, :, :self.units]
        z1 = z[:, :, self.units: 2 * self.units]
        z2 = z[:, :, 2 * self.units: 3 * self.units]
        z3 = z[:, :, 3 * self.units:]

        i = tf.nn.sigmoid(z0)
        f = tf.nn.sigmoid(z1)
        c = f * c_tm1 + i * tf.nn.tanh(z2)
        o = tf.nn.sigmoid(z3)

        h = o * tf.nn.tanh(c)

        return h, [h, c]


class DenseLayer(tf.keras.layers.Layer):
    def __init__(self, units, models, use_bias=True, **kwargs):
        super(DenseLayer, self).__init__()

        self.models = models
        self.units = units
        self.use_bias = use_bias

    def build(self, input_shape, kernel=None, bias=None):
        input_dim = input_shape[-1]

        if kernel is not None:
            self.kernel = self.add_weight(shape=(self.models, input_dim, self.units),
                                          name='kernel', initializer=tf.constant_initializer(kernel))
        else:
            self.kernel = self.add_weight(shape=(self.models, input_dim, self.units),
                                          name='kernel', initializer='random_normal')

        if bias is not None:
            self.bias = self.add_weight(shape=(self.models, 1, self.units),
                                          name='bias', initializer=tf.constant_initializer(bias))
        else:
            if self.use_bias:
                self.bias = self.add_weight(shape=(self.models, 1, self.units),
                                            name='bias', initializer='random_normal')
            else:
                self.bias = None

        self.built = True

    @tf.function
    def call(self, inputs):
        z = tf.matmul(inputs, self.kernel) + self.bias
        return tf.nn.tanh(z)


class LSTMModel:
    def __init__(self, lstm_units, dense_units, numOfSubModels, use_bias=True):
        self.lstm_units = lstm_units
        self.numOfSubModels = numOfSubModels

        self.stacked_cell = StackedLSTMCell(lstm_units, numOfSubModels, use_bias)
        self.stacked_cell_states = None
        self.dense_layer = DenseLayer(dense_units, numOfSubModels, use_bias)

    def build(self, input_shape, model_weights):
        weights, biases = model_weights
        batch_size = 1

        self.stacked_cell_states = [tf.zeros((self.numOfSubModels, batch_size, self.lstm_units)),
                                    tf.zeros((self.numOfSubModels, batch_size, self.lstm_units))]

        if weights is not None and biases is not None:
            self.stacked_cell.build(input_shape, kernel=weights[0], recurrent_kernel=weights[1], bias=biases[0])
            self.dense_layer.build([self.stacked_cell.units], kernel=weights[2], bias=biases[1])
        elif weights is not None:
            self.stacked_cell.build(input_shape, kernel=weights[0], recurrent_kernel=weights[1])
            self.dense_layer.build([self.stacked_cell.units], kernel=weights[2])
        else:
            self.stacked_cell.build(input_shape)
            self.dense_layer.build([self.stacked_cell.units])

        self.built = True


    def __call__(self, inputs):
        return self.call(inputs).numpy()

    @tf.function
    def call(self, inputs):
        (output, states) = self.stacked_cell.call(inputs, self.stacked_cell_states)
        self.stacked_cell_states = states
        output = self.dense_layer.call(output)
        output = tf.squeeze(output, 1)
        return output


# lstm = LSTMModel(16, 4, 100, True)
# lstm.build()
