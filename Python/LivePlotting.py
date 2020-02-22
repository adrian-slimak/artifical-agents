from multiprocessing import Process, Queue
import matplotlib.pyplot as plt
from matplotlib import use
plt.style.use('ggplot')
use('TkAgg')
from collections.abc import Iterable

class LivePlot:
    def __init__(self, labels={'prey': ['epochs', 'fitness']}, lines={'prey': ['max', 'avg']}, figsize=(10, 8)):
        self.fig, self.ax = plt.subplots(2, 1, figsize=figsize)
        if not isinstance(self.ax, Iterable):
            self.ax = [self.ax]
        else:
            self.ax = self.ax.tolist()

        for ax, (key, lbs) in zip(self.ax, labels.items()):
            ax.set_title(key)
            ax.set_xlabel(lbs[0])
            ax.set_ylabel(lbs[1])
            ax.set_xlim(0, 200)
            ax.set_ylim(0, 30)

        colors = ['r', 'b']
        for ax, ls in zip(self.ax, lines.values()):
            for l, color in zip(ls, colors):
                ax.plot([], [], color, label=l)

        for ax in self.ax:
            ax.legend()

        self.linePlotX = []
        self.linePlotY = {key: {k: [] for k in lines[key]} for key in labels.keys()}

        self._start_process()

    def _start_process(self):
        self.plot_queue = Queue()
        self.plot_process = Process(target=self, daemon=True)
        self.plot_process.start()

    def __call__(self):
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
        self.linePlotX = list(range(len(self.linePlotX)+1))

        for key, value in data.items():
            for k, v in zip(self.linePlotY[key].keys(), value):
                self.linePlotY[key][k].append(v)

        for ax, Y in zip(self.ax, self.linePlotY.values()):
            for line, values in zip(ax.lines, Y.values()):
                line.set_xdata(self.linePlotX)
                print(values)
                line.set_ydata(values)

        self.fig.canvas.draw()
        # self.fig.canvas.flush_events()

    def update(self, data):
        self.plot_queue.put(data)

    def close(self):
        self.plot_queue.put(None)
