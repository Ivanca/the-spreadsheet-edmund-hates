import tkinter as tk; r = tk.Tk(); r.withdraw(); r.clipboard_clear(); r.clipboard_append(hex(ida_kernwin.get_screen_ea() - ida_nalt.get_imagebase())); r.update()
