# import tensorflow as tf
import numpy as np

class DenseLayer:
    def __init__(self, input_dim, units, models, use_bias=True):
        self.input_dim = input_dim
        self.models = models
        self.units = units
        self.use_bias = use_bias

        self.kernel = None
        self.bias = None

    def build(self, kernel=None, bias=None):

        if kernel is not None:
            self.kernel = kernel
        # else:
        #     self.kernel = tf.Variable(shape=(self.models, self.input_dim, self.units), dtype=tf.float32,
        #                                   name='kernel', initializer='random_normal')

        if bias is not None:
            self.bias = bias
        else:
            # if self.use_bias:
            #     self.bias = tf.Variable(shape=(self.models, 1, self.units), dtype=tf.float32,
            #                                 name='bias', initializer='random_normal')
            # else:
                self.bias = None

        self.built = True

    # @tf.function
    def call(self, inputs):
        z = np.matmul(inputs, self.kernel, dtype=np.float32)

        if self.use_bias:
            z = np.add(z, self.bias, dtype=np.float32)

        return np.tanh(z, dtype=np.float32)


class MLPModel:
    name = 'mlp'

    def __init__(self, input_dim, mlp_units, output_units, models, n_envs=1, use_bias=True):
        self.mlp_units = mlp_units
        self.input_dim = input_dim
        self.models = models
        self.n_envs = n_envs

        self.input_layer = DenseLayer(input_dim, mlp_units, models, use_bias)
        self.output_layer = DenseLayer(mlp_units, output_units, models, use_bias)

    def build(self, model_weights=(None, None)):
        weights, biases = model_weights

        if weights is not None and biases is not None:
            self.input_layer.build(kernel=weights[0], bias=biases[0])
            self.output_layer.build(kernel=weights[1], bias=biases[1])
        elif weights is not None:
            self.input_layer.build(kernel=weights[0])
            self.output_layer.build(kernel=weights[1])
        else:
            self.input_layer.build()
            self.output_layer.build()

    def __call__(self, inputs):
        return self.call(inputs)

    # @tf.function
    def call(self, inputs):
        inputs = np.reshape(inputs, (self.n_envs, self.models, self.input_dim))
        inputs = np.transpose(inputs, [1, 0, 2])
        output = self.input_layer.call(inputs)
        output = self.output_layer.call(output)
        output = np.transpose(output, (1, 0, 2))
        return output
