# Catstable Build Guide

This repository builds a Windows mod for Mewgenics. The project is driven by `build.bat`, which does the full build flow: it kills any running Mewgenics processes, compiles the native bridge DLL, publishes the .NET mod, copies the generated files into the game install and installer folders, and then launches the game.

## What the build does

The script performs these steps in order:

1. Stops any running `Mewgenics.exe` and `x64dbg.exe` processes if needed.
2. Locates the latest installed Visual Studio build environment that includes the x64 C++ toolchain.
3. Runs `vcvars64.bat` to set up the MSVC environment.
4. Compiles the native C++ bridge:
   - `MewjectorBridge.cpp` -> `MewjectorBridge.dll`
5. Copies the native DLL into the installer mod folder.
6. Optionally builds the installer with Inno Setup if `--include-installer` is provided.
7. Copies the native DLL and the mod assets to the live Mewgenics installation under the Steam path.
8. Publishes the .NET mod with:
   - `dotnet publish -c Release -r win-x64 --self-contained`
9. Copies the published `.dll` and `swf` files into both the installer folder and the live game mod folder.
10. Starts `Mewgenics.exe`.

## Requirements

You will need:

- Windows 10 or 11
- Visual Studio 2022 or Visual Studio Build Tools
- Desktop development with C++ workload installed
- MSVC x64/x86 build tools
- .NET 8 SDK
- Inno Setup 7 if using the installer build flag
- A working Steam installation of Mewgenics on the same path expected by the script

### Required Visual Studio components

The build script looks for:

- `C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe`
- A Visual Studio installation containing:
  - `Microsoft.VisualStudio.Component.VC.Tools.x86.x64`

This is used to find `vcvars64.bat` and load the MSVC environment.

## Build command

From the project root, run:

```bat
build.bat
```

To also build the Inno Setup installer:

```bat
build.bat --include-installer
```

## Output locations

The script writes files to several places:

- Native DLL output in the project root:
  - `MewjectorBridge.dll`
- Published .NET mod output:
  - `bin\x64\Release\net8.0-windows\win-x64\publish\the_spreadsheet_edmund_hates.dll`
- Installer files:
  - `installer\mod\the_spreadsheet_edmund_hates\`
- Live game install copies:
  - `E:\SteamLibrary\steamapps\common\Mewgenics\mods\the_spreadsheet_edmund_hates\`

## Important path assumptions

The script contains hardcoded paths for:

- `E:\SteamLibrary\steamapps\common\Mewgenics\mods\the_spreadsheet_edmund_hates\`
- `C:\Program Files\Inno Setup 7\ISCC.exe`

If your game or tools are installed in different locations, edit the corresponding paths in `build.bat` before running it.

## Project configuration

The .NET mod project is configured in `catstable.csproj` as:

- Target framework: `net8.0-windows`
- Output type: library
- Assembly name: `the_spreadsheet_edmund_hates`
- Runtime identifier: `win-x64`
- Self-contained: `true`
- Native dependencies include Vortice Direct3D packages for the overlay/hook work.

## Troubleshooting

### `vswhere.exe` not found

Install Visual Studio 2022 Build Tools or a full Visual Studio installation with the C++ workload.

### `vcvars64.bat` not found

Your Visual Studio install is missing the C++ build tools. Reinstall and include the x64 MSVC toolchain.

### Installer build fails

If you passed `--include-installer`, ensure Inno Setup 7 is installed at:

```text
C:\Program Files\Inno Setup 7\ISCC.exe
```

### Mewgenics installation not found

The script expects the mod folder under the Steam install path. If your game is elsewhere, update the copy targets in `build.bat`.

## Notes

- The script is intentionally a development build script, not a universal cross-platform build pipeline.
- This project is built for Windows and targets the `win-x64` runtime.
- The build starts the game automatically after a successful compilation, which is useful for rapid live debugging.

## Example

```bat
build.bat
```

or

```bat
build.bat --include-installer
```

These commands build the native bridge, publish the .NET mod, copy it into the game folder, and launch Mewgenics for testing.
