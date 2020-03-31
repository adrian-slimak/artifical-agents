from multiprocessing import Process, Queue
from collections.abc import Iterable
import matplotlib.pyplot as plt
from pickle import dump
from os import listdir
from re import findall
import matplotlib
plt.style.use('ggplot')
matplotlib.use('tkagg')


class PlotPart:
    def __init__(self, name, params, ax):
        self.name = name
        self._ax = ax

        self._ax.set_title(params['title'])
        self._ax.set_xlabel(params['labels'][0])
        self._ax.set_ylabel(params['labels'][1])
        self._ax.set_xlim(0, params['lims'][0])
        self._ax.set_ylim(0, params['lims'][1])

        self.X = []
        self.Y = {}

        colors = ['g', 'r', 'b']
        for line_name, color in zip(params['lines'], colors):
            self._ax.plot([], [], color, label=line_name)
            self.Y[line_name] = []

        self._ax.legend()

    def update(self, data):
        self.X = list(range(len(self.X) + 1))
        for line, Ys, v in zip(self._ax.lines, self.Y.values(), data):
            Ys.append(v)
            line.set_xdata(self.X)
            line.set_ydata(Ys)


class LivePlot:
    def __init__(self, plots=None, subplots=(1,1), figsize=(10, 8)):
        self.fig, self.ax = plt.subplots(subplots[0], subplots[1], figsize=figsize)
        self.ID = None
        self.plots = {}

        print(self.ax)
        if not isinstance(self.ax, Iterable) or not isinstance(self.ax[0], Iterable):
            self.ax = [self.ax]

        for plot_name, params in plots.items():
            pos = params['position']
            self.plots[plot_name] = PlotPart(plot_name, params, self.ax[pos[0]][pos[1]])

        plt.tight_layout()
        self._start_process()

    def _onkey(self, event):
        if event.key == 'alt+f':
            if self.ID is None:
                self.ID = 0
                IDs = [int(findall('\d+', i)[0]) for i in listdir('results/plots')]
                if len(IDs) > 0:
                    self.ID = max(IDs)+1

            plt.savefig(f'results/plots/plot_{self.ID}.png')
            with open(f'results/plots/data_{self.ID}.pkl', 'wb') as file:
                copy = {key: item['Y'] for key, item in self.plots.items()}
                dump(copy, file)

    def _start_process(self):
        self.plot_queue = Queue()
        self.plot_process = Process(target=self, daemon=True)
        self.plot_process.start()

    def __call__(self):
        self.fig.canvas.mpl_connect('key_press_event', self._onkey)
        timer = self.fig.canvas.new_timer(interval=1000)
        timer.add_callback(self._call_back)
        timer.start()
        plt.show()

    def _call_back(self):
        while not self.plot_queue.empty():
            data = self.plot_queue.get()
            if data is None:  # Terminate
                plt.close('all')
                return False
            else:
                self._update(data)
        return True

    def _update(self, data):
        for data_key in data.keys():
            self.plots[data_key].update(data[data_key])

        self.fig.canvas.draw()
        # self.fig.canvas.flush_events()

    def update(self, data):
        self.plot_queue.put(data)

    def close(self):
        self.plot_queue.put(None)
