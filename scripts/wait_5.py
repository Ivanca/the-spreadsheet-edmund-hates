import time
from x64dbg_automate import X64DbgClient

client = X64DbgClient.connect_remote(
    host="127.0.0.1",
    req_rep_port=27066,
    pub_sub_port=27067,
)

# client = X64DbgClient()

# Window handles constants used by x64dbg:
# 0 = GUI_DISASSEMBLY (CPU Window)
# 1 = GUI_DUMP (Memory Dump Window)
# 2 = GUI_STACK (Stack Window)
GUI_DISASSEMBLY = 0

selected, success = client.eval_sync("dis.sel()")
print(f"Selected: 0x{selected:X}")
# This sleep occurs in Python, so x64dbg/debuggee continues running.
# Resume execution for a short period before pausing.
client.go()

time.sleep(5)

# Stop execution after 5 seconds.
client.pause()

# Activate a normal software breakpoint.
if not client.set_breakpoint(selected):
    raise RuntimeError(f"Failed to set breakpoint at 0x{selected:X}")

print(f"Breakpoint set at 0x{selected:X}")

# Continue normally.
client.go()

# disconnect
client.detach_session()