import numpy as np
import seaborn as sns
sns.set_style()
import matplotlib.pyplot as plt
import matplotlib
matplotlib.use('TkAgg')
import pickle
import time
import random


class LivePlot:
    def __init__(self, xLabel='X', yLabel='Y', lines=['line']):
        self.fig, self.ax = plt.subplots(1,1)
        self.ax.set_xlabel(xLabel)
        self.ax.set_ylabel(yLabel)
        self.ax.set_xlim(0, 200)
        self.ax.set_ylim(0, 60)

        colors = ['r', 'b']
        for line, color in zip(lines, colors):
            self.ax.plot([], [], color, label=line)

        plt.ion()
        plt.show()
        self.X = []
        self.Y = [[] for i in range(len(lines))]

    def update(self, y):
        for Yi, yi in zip(self.Y, y):
            Yi.append(yi)
        self.X = list(range(len(self.Y[0])))

        for line, values in zip(self.ax.lines, self.Y):
            line.set_xdata(self.X)
            line.set_ydata(values)
        self.fig.canvas.draw()
        self.fig.canvas.flush_events()


# with open('results.pickle', 'rb') as file:
#     results = np.array(pickle.load(file))[:100]/10.
#
# # with open('60-x3-8units_2.pickle', 'rb') as file:
# #     results2 = np.array(pickle.load(file))[:100]/10.
# #
# # with open('60-x3-8units_1.pickle', 'rb') as file:
# #     results3 = np.array(pickle.load(file))[:100]/10.
#
# # mean1, mean2, mean3 = np.mean(results1, axis=1), np.mean(results2, axis=1), np.mean(results3, axis=1)
# # max1, max2, max3 = np.max(results1, axis=1), np.max(results2, axis=1), np.max(results3, axis=1)
# # mean = (mean1+mean2+mean3)/3.
# # max = (max1+max2+max3)/3.
#
# mean = np.mean(results, axis=1)
# max = np.max(results, axis=1)
#
# fig, axes = plt.subplots(1,2, figsize=(15,7))
# sns.lineplot(data=mean, ax=axes[0])
# axes[0].set(ylim=(0, 10))
# axes[0].axhline(5, ls='--')
# axes[0].set_title("Średnia liczba zdobytego pożywienia", fontsize=15)
# axes[0].set_ylabel("Liczba pożywienia", fontsize=13)
# axes[0].set_xlabel("Populacja", fontsize=13)
#
# sns.lineplot(data=max, ax=axes[1], color='orange')
# axes[1].set(ylim=(0, 30))
# axes[1].axhline(15, ls='--')
# axes[1].set_title("Maksymalna liczba zdobytego pożywienia (najlepszy osobnik)", fontsize=15)
# axes[1].set_ylabel("Liczba pożywienia", fontsize=13)
# axes[1].set_xlabel("Populacja", fontsize=13)
#
# plt.show()


