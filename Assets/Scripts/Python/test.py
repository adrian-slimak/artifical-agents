import mmap
import sys
import tensorflow as tf
import numpy as np

m = mmap.mmap(fileno=-1, length=200000, tagname='memory')
# a = np.zeros()
# b = a.tobytes()

# print(len(b))
# m.seek(0)
# m.write(b)

m.seek(0)
b = m.read(12)
t = tf.io.decode_raw(b, out_type=tf.float32)
print(t)

# m.seek(0)
# k = m.read(12)
# print(np.fromstring(k, dtype='f'))