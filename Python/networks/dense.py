import tensorflow as tf


class DenseLayer(tf.keras.layers.Layer):
    def __init__(self, input_dim, units, models, use_bias=True):
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


class MLPModel:
    name = 'mlp'

    def __init__(self, input_dim, mlp_units, output_units, models, n_envs=1, use_bias=True):
        self.mlp_units = mlp_units
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
        return self.call(inputs).numpy()

    @tf.function
    def call(self, inputs):
        output = self.input_layer.call(inputs)
        output = self.output_layer.call(output)
        output = tf.transpose(output, (1, 0, 2))
        return output
