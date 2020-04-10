from other.hotkey_listener import HotKeyListener

hkl = HotKeyListener()

hkl.add('a', lambda : print(1))
hkl.add('a', lambda : print(2))

while True:
    pass