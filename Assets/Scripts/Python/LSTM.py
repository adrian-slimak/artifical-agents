import tensorflow as tf


class StackedLSTMCell(tf.keras.layers.Layer):
    def __init__(self, units, models, use_bias=True, **kwargs):
        super(StackedLSTMCell, self).__init__(**kwargs)

        self.models = models
        self.units = units
        self.use_bias = use_bias

    def build(self, input_shape, kernel=None, recurrent_kernel=None, bias=None):
        input_dim = input_shape[-1]

        if kernel:
            self.kernel = tf.constant(kernel)
        else:
            self.kernel = self.add_weight(shape=(self.models, input_dim, self.units * 4),
                                          name='kernel', initializer='random_normal') * 15
        if recurrent_kernel:
            self.recurrent_kernel = tf.constant(recurrent_kernel)
        else:
            self.recurrent_kernel = self.add_weight(shape=(self.models, self.units, self.units * 4),
                                                    name='recurrent_kernel', initializer='random_normal') * 15

        if bias:
            self.bias = tf.constant(bias)
        else:
            if self.use_bias:
                self.bias = self.add_weight(shape=(self.models, 1, self.units * 4),
                                            name='bias', initializer='random_normal') * 15
            else:
                self.bias = None

        self.built = True

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
    def __init__(self, units, numOfSubModels):
        self.units = units
        self.numOfSubModels = numOfSubModels

        self.stacked_cell = StackedLSTMCell(units, numOfSubModels)
        self.stacked_cell_states = None

    def build(self, input_shape, genes=None):
        batch_size = input_shape[-2]

        self.stacked_cell_states = [tf.zeros((self.numOfSubModels, batch_size, self.units)),
                                    tf.zeros((self.numOfSubModels, batch_size, self.units))]

        self.stacked_cell.build(input_shape)

    def __call__(self, inputs):
        return self.call(inputs)

    @tf.function
    def call(self, inputs):
        (output, states) = self.stacked_cell(inputs, self.stacked_cell_states)
        self.stacked_cell_states = states
        output = tf.squeeze(output, 1)
        return output

    def warmupTensorGraph(self):
        pass


# brainNames = [ str(x) for x in range(200)]
# createModels()
# start = timer()
# model = LSTMModel(units=32, numOfSubModels=400)
#
# input_shape = (400, 1, 150)
# inputs = np.ones(input_shape)
# model.build(input_shape)
# end = timer()
# print(end - start)
#
# start = timer()
# output = model(inputs)
# end = timer()
# print(end - start)
#
# start = timer()
# output = model(inputs)
# print(output.numpy())
# end = timer()
# print(end - start)