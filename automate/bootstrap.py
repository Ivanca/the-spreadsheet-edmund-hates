# from x64dbg_automate import X64DbgClient
import psutil
import inspect
import os
import time
import pyautogui
import pygetwindow as gw


def send_command_to_window(window_title, command):
    # 1. Search for the window by its title
    windows = gw.getWindowsWithTitle(window_title)

    if not windows:
        print(f"Window '{window_title}' not found.")
        return False

    # 2. Target the first matching window
    target_window = windows[0]
    print(f"Found window '{window_title}' with handle: {target_window._hWnd}")
    try:
        # 3. Bring the window to the foreground
        target_window.activate()
        time.sleep(0.5)  # Short pause to allow window to focus

        # 4. Send the Ctrl+Alt+F2 keyboard command
        pyautogui.hotkey(*command)
        print(f"Successfully sent Ctrl+Alt+F2 to '{window_title}'.")
        return True

    except Exception as e:
        print(f"Failed to activate window or send command: {e}")
        return False

send_command_to_window("x64dbg", ["ctrl", "alt", "f2"]) 
time.sleep(1) 
send_command_to_window("x64dbg", ["alt", "f4"]) 

X64DBG_PATH = r"D:\Downloads\Software\snapshot_2026-04-20_19-04\release\x64\x64dbg.exe"
# TARGET_PROCESS = "Mewgenics"
# SEARCH_STRING = b"g duplication"
def find_pid(name):
    for proc in psutil.process_iter(["pid", "name"]):
        if proc.info["name"] and proc.info["name"].lower() == name.lower():
            return proc.info["pid"]
    raise RuntimeError(f"Process '{name}' not found")

from x64dbg_automate import X64DbgClient
from x64dbg_automate.models import (
   MemoryBreakpointType,
)
from x64dbg_automate import models
from pprint import pprint

pid = find_pid("Mewgenics.exe")   # or "Mewgenics" if that's the actual executable name


dbg = X64DbgClient(x64dbg_path=X64DBG_PATH)
# print(inspect.signature(dbg.set_hardware_breakpoint))

# print(models.HardwareBreakpointType)
# print(list(models.HardwareBreakpointType))

# print(models.HardwareBreakpointSize)
# print(list(models.HardwareBreakpointSize))
# exit(0)
dbg.start_session_attach(pid)

needle = "g duplication".encode("utf-16-le")

CHUNK_SIZE = 1024 * 1024  # 1 MB
TIMEOUT = 30              # seconds

dbg.go()


time.sleep(10)
needle = "g duplication".encode("utf-16-le")

for page in dbg.memmap():
    if not dbg.check_valid_read_ptr(page.base_address):
        continue

    try:
        data = dbg.read_memory(page.base_address, page.region_size)
    except Exception:
        continue

    if not data:
        continue

    idx = data.find(needle)
    if idx != -1:
        found = page.base_address + idx
        print(hex(found))
        # ok = dbg.cmd_sync(f"bpm {found:#x}, 8, a")
        ok = dbg.set_hardware_breakpoint(
            found,
            MemoryBreakpointType.r, # type: ignore
            8
        )
        print(ok)
        # dbg.pause()
        # dbg.wait_until_stopped()

        # dbg.set_memory_breakpoint(found, MemoryBreakpointType.r)
        break
# print("String not found.")