# from other.hotkey_listener import HotKeyListener
#
# hkl = HotKeyListener()
#
# hkl.add('a', lambda : print(1))
# hkl.add('a', lambda : print(2))
#
# while True:
#     pass

import numpy as np
import random

a = np.array([[1,1,1,1],[2,2,2,2],[3,3,3,3],[4,4,4,4],[5,5,5,5],[6,6,6,6]])
a = np.zeros((10, 6))

n_env = 2
for i in range(int(10/n_env)):
    n, n1 = i*n_env, (i+1)*n_env
    fitness = np.ones((n_env, 6))*i
    a[[0,5]] = fitness

print(a)