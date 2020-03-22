from multiprocessing import Process, Queue
from os import listdir
from re import findall
import matplotlib.pyplot as plt
import matplotlib
from pickle import dump
plt.style.use('ggplot')
matplotlib.use('tkagg')
from collections.abc import Iterable


class LivePlot:
    def __init__(self, plots={'fig1': (['xLabel', 'yLabel'], ['line1', 'line2', 'line3'])}, figsize=(10, 8)):
        self.fig, self.ax = plt.subplots(len(plots.keys()), 1, figsize=figsize)
        self.ID = None
        self.plots = {}

        if not isinstance(self.ax, Iterable):
            self.ax = [self.ax]

        colors = ['g', 'r', 'b']
        for (plot_name, plot_params), ax in zip(plots.items(), self.ax):
            self.plots[plot_name] = {'ax': ax}
            ax.set_title(plot_name)
            ax.set_xlabel(plot_params[0][0])
            ax.set_ylabel(plot_params[0][1])
            ax.set_xlim(0, 200)
            ax.set_ylim(0, 30)

            self.plots[plot_name]['Y'] = {}
            for line_name, color in zip(plot_params[1], colors):
                ax.plot([], [], color, label=line_name)
                self.plots[plot_name]['Y'][line_name] = []

            ax.legend()

        self.X = {plot_name: [] for plot_name in plots.keys()}

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
        for key in data.keys():
            self.X[key] = list(range(len(self.X[key])+1))

        for key, value in data.items():
            plot = self.plots[key]
            for line, Ys, v in zip(plot['ax'].lines, plot['Y'].values(), value):
                Ys.append(v)
                line.set_xdata(self.X[key])
                line.set_ydata(Ys)

        self.fig.canvas.draw()
        # self.fig.canvas.flush_events()

    def update(self, data):
        self.plot_queue.put(data)

    def close(self):
        self.plot_queue.put(None)
