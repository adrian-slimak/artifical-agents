import tensorflow as tf


class DenseLayer(tf.keras.layers.Layer):
    def __init__(self, input_dim, units, models, use_bias=True, **kwargs):
        super(DenseLayer, self).__init__()

        self.input_dim = input_dim
        self.models = models
        self.units = units
        self.use_bias = use_bias

        self.kernel = None
        self.bias = None

    def build(self, kernel=None, bias=None):

        if kernel is not None:
            self.kernel = self.add_weight(shape=(self.models, self.input_dim, self.units),
                                          name='kernel', initializer=tf.constant_initializer(kernel))
        else:
            self.kernel = self.add_weight(shape=(self.models, self.input_dim, self.units),
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
        z = tf.matmul(inputs, self.kernel)

        if self.use_bias:
            z = tf.add(z, self.bias)

        return tf.nn.tanh(z)