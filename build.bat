@echo off
setlocal

rem Run from the directory containing this .bat file
cd /d "%~dp0"

rem Force kill any processes of Mewgenics.exe if there are any
tasklist /FI "IMAGENAME eq Mewgenics.exe" 2>NUL | find /I "Mewgenics.exe" >NUL
if %ERRORLEVEL% EQU 0 (
    echo Killing Mewgenics.exe processes...
    taskkill /IM Mewgenics.exe /F >NUL 2>&1
) else (
    echo No Mewgenics.exe processes found.
)

rem python exclude.py
rem python find_dangerous_hooks.py
rem python gen_hooks.py

rem Remove obj and bin folders
if exist "obj" rmdir /S /Q "obj"
if exist "bin" rmdir /S /Q "bin"

if "%1"=="--include-installer" (
    rem Create installer/mod if missing
    if not exist "installer\mod" mkdir "installer\mod"
    if not exist "installer\mod\catstable" mkdir "installer\mod\catstable"
    if not exist "installer\mod\catstable\swfs" mkdir "installer\mod\catstable\swfs"
    echo Compiling installer...
    "C:\Program Files\Inno Setup 7\ISCC.exe" "installer\MewgenicsCatStatsInstaller.iss"
    if errorlevel 1 exit /B 1
)


rem Find the latest Visual Studio installation containing the x64 C++ tools
set "VSWHERE=C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe"

if not exist "%VSWHERE%" (
    echo ERROR: vswhere.exe was not found:
    echo   "%VSWHERE%"
    exit /B 1
)

for /F "usebackq delims=" %%I in (`"%VSWHERE%" -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath`) do (
    set "VS_PATH=%%I"
)

if not defined VS_PATH (
    echo ERROR: Visual Studio with the required C++ tools was not found.
    exit /B 1
)

set "VCVARS=%VS_PATH%\VC\Auxiliary\Build\vcvars64.bat"

if not exist "%VCVARS%" (
    echo ERROR: vcvars64.bat was not found:
    echo   "%VCVARS%"
    exit /B 1
)

rem Load the Visual Studio x64 build environment and compile the DLL
call "%VCVARS%" || exit /B 1

cl /LD /O2 /EHsc "CatsTableBridge.cpp" /Fe:"CatsTableBridge.dll"
if errorlevel 1 exit /B 1

rem Remove intermediate compiler files
if exist "CatsTableBridge.exp" del /Q "CatsTableBridge.exp"
if exist "CatsTableBridge.lib" del /Q "CatsTableBridge.lib"
if exist "CatsTableBridge.obj" del /Q "CatsTableBridge.obj"

rem Copy CatsTableBridge.dll to the installer
copy /Y "CatsTableBridge.dll" "installer\mod\catstable\CatsTableBridge.dll" >NUL
if errorlevel 1 exit /B 1
echo Copied CatsTableBridge.dll to installer\mod\catstable\CatsTableBridge.dll

rem Copy CatsTableBridge.dll to the live Mewgenics installation
copy /Y "CatsTableBridge.dll" "E:\SteamLibrary\steamapps\common\Mewgenics\mods\catstable\CatsTableBridge.dll" >NUL
if errorlevel 1 exit /B 1

rem Publish the .NET mod
dotnet publish -c Release -r win-x64 --self-contained
if errorlevel 1 exit /B 1

rem Copy the .NET mod to the installer
copy /Y "bin\x64\Release\net8.0-windows\win-x64\publish\catstable.dll" "installer\mod\catstable\catstable.dll" >NUL
if errorlevel 1 exit /B 1
echo Copied catstable.dll to installer\mod\catstable\catstable.dll

rem Copy the .NET mod to the live Mewgenics installation while live debugging
copy /Y "bin\x64\Release\net8.0-windows\win-x64\publish\catstable.dll" "E:\SteamLibrary\steamapps\common\Mewgenics\mods\catstable\catstable.dll" >NUL
if errorlevel 1 exit /B 1

rem Copy SWF files to the installer
copy /Y "swf\house_table_stats.swf" "installer\mod\catstable\swfs\house_table_stats.swf" >NUL
if errorlevel 1 exit /B 1
echo Copied house_table_stats.swf to installer\mod\catstable\swfs\house_table_stats.swf

copy /Y "swf\swflist.gon.append" "installer\mod\catstable\swfs\swflist.gon.append" >NUL
if errorlevel 1 exit /B 1
echo Copied swflist.gon.append to installer\mod\catstable\swfs\swflist.gon.append

rem Example save-file copies:
rem copy /Y "%APPDATA%\Glaiel Games\Mewgenics\76561197960287930\saves\steamcampaign01 - Copy.sav" "%APPDATA%\Glaiel Games\Mewgenics\76561197960287930\saves\steamcampaign01.sav"
rem copy /Y "%APPDATA%\Glaiel Games\Mewgenics\76561197960287930\saves\steamcampaign02 - Copy.sav" "%APPDATA%\Glaiel Games\Mewgenics\76561197960287930\saves\steamcampaign02.sav"

@REM rem Wait one second to ensure memory is freed up
@REM C:\Windows\System32\timeout.exe /T 1 /NOBREAK >NUL

echo.
echo Build completed successfully.
exit /B 0
