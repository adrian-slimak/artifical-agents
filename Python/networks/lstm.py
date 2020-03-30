import tensorflow as tf
from networks.dense import DenseLayer


class LSTMCell(tf.keras.layers.Layer):
    def __init__(self, input_dim, units, models, use_bias=False):
        super(LSTMCell, self).__init__()

        self.input_dim = input_dim
        self.models = models
        self.units = units
        self.use_bias = use_bias

        self.kernel = None
        self.recurrent_kernel = None
        self.bias = None

    def build(self, kernel=None, recurrent_kernel=None, bias=None):

        if kernel is not None:
            self.kernel = self.add_weight(shape=(self.models, self.input_dim, self.units * 4),
                                          name='kernel', initializer=tf.constant_initializer(kernel))
        else:
            self.kernel = self.add_weight(shape=(self.models, self.input_dim, self.units * 4),
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


class LSTMModel:
    name = 'lstm'

    def __init__(self, input_dim, lstm_units, dense_units, models, n_envs=1, use_bias=True):
        self.lstm_units = lstm_units
        self.models = models
        self.n_envs = n_envs

        self.lstm_cell = LSTMCell(input_dim, lstm_units, models, use_bias)
        self.cell_states = None
        self.output_layer = DenseLayer(lstm_units, dense_units, models, use_bias)

    def build(self, model_weights=(None, None)):
        weights, biases = model_weights

        self.cell_states = [tf.zeros((self.models, self.n_envs, self.lstm_units)),
                            tf.zeros((self.models, self.n_envs, self.lstm_units))]

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
        return self.call(inputs).numpy()

    @tf.function
    def call(self, inputs):
        (output, states) = self.lstm_cell.call(inputs, self.cell_states)
        self.cell_states = states
        output = self.output_layer.call(output)
        if self.n_envs == 1:
            output = tf.squeeze(output, 1)
        else:
            output = tf.transpose(output, (1, 0, 2))
        return output
