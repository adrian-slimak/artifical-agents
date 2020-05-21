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
# import tensorflow as tf
from time import time
import numba

a = np.random.uniform(-100,100,(60, 2))
b = np.random.uniform(-100,100,(6, 2))
c = np.random.uniform(-100,100,(400, 2))

x = np.array([[0,2],[1,4],[1,6],[1,9]])
a = np.array([0,180,290,312])
y = np.array([[0,0],[32,3],[54,23],[23,12]])

def compute_distances_no_loops(X, Y):
    dists = -2 * np.dot(X, Y.T) + np.sum(Y**2, axis=1) + np.sum(X**2, axis=1)[:, np.newaxis]
    return dists

def distaces(X, Y):
    r = np.zeros((X.shape[0], Y.shape[0]))
    for i in range(X.shape[0]):
        for j in range(Y.shape[0]):
            d = np.linalg.norm(X[i] - Y[j])
            r[i][j] = d
    return r

def calc_angle(fromX, fromY, fromAngle, toX, toY):
    Ux, Uy = toX - fromX, toY - fromY
    Vx, Vy = np.cos(fromAngle), np.sin(fromAngle)
    firstTerm, secondTerm = (Ux * Vy) - (Uy * Vx), (Ux * Vx) + (Uy * Vy)
    return np.arctan2(firstTerm, secondTerm) * 180.0 / np.pi

def calc_angle_slow(X, A, Y):
    r = np.zeros((X.shape[0], Y.shape[0]))
    for i in range(X.shape[0]):
        for j in range(Y.shape[0]):
            d = calc_angle(X[i][0], X[i][1], A[i], Y[j][0], Y[j][1])
            r[i][j] = d
    return r

# N=1000
# t = time()
# for i in range(N):
#     distaces(a,b)
#     distaces(a,c)
# print(((time()-t)/N)*1000)

# t = time()
# for i in range(N):
#     t=compute_distances_no_loops(a,a)
#     t=compute_distances_no_loops(a,b)
#     t=compute_distances_no_loops(b,b)
#     t=compute_distances_no_loops(a,c)
#     t=compute_distances_no_loops(b,c)
# print(((time()-t)/N))