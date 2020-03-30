from pynput.keyboard import Key, KeyCode, Listener

class HotKeyListener:
    def __init__(self, function):
        self.current_keys = set()

        self.combination_to_function = \
            {
                frozenset([Key.alt_l, KeyCode(char='p')]): function
            }

        self.listener = Listener(on_press=self.on_press, on_release=self.on_release)
        self.listener.start()

    def on_press(self, key):
        self.current_keys.add(key)
        if frozenset(self.current_keys) in self.combination_to_function:
            self.combination_to_function[frozenset(self.current_keys)]()

    def on_release(self, key):
        if key in self.current_keys:
            self.current_keys.remove(key)

    def stop(self):
        self.listener.join()
