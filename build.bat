@echo off
setlocal enabledelayedexpansion

set "SKIP_BRIDGE=false"
set "MEWGENICS_MODS_FOLDER=E:\SteamLibrary\steamapps\common\Mewgenics\Mewtator\mods"
set "ISCC_EXE=C:\Program Files\Inno Setup 7\ISCC.exe"

rem Run from the directory containing this .bat file
cd /d "%~dp0"

:parse_args
:: If there are no more arguments, stop looping
if "%~1"=="" goto end_parse

if /i "%~1"=="--skip-bridge" (
    set "SKIP_BRIDGE=true"

) else if /i "%~1"=="--dev" (
    set "DEVMODE=true"

) else if /i "%~1"=="--include-installer" (
    set "INCLUDE_INSTALLER=true"

) else (
    echo Unknown option: %1
)
:: Move to the next argument and repeat
shift
goto parse_args

:end_parse

if "%DEVMODE%"=="true" (
    rem Force kill any processes of Mewgenics.exe if there are any
    tasklist /FI "IMAGENAME eq Mewgenics.exe" 2>NUL | %windir%\System32\find.exe /I "Mewgenics.exe" >NUL
    if %ERRORLEVEL% EQU 0 (
        echo Killing Mewgenics.exe processes...
        taskkill /IM Mewgenics.exe /F >NUL 2>&1
    ) else (
        echo No Mewgenics.exe processes found.
    )

    @REM wait one second:
    %windir%\System32\timeout.exe /T 1 /NOBREAK >NUL

    @REM If mewgenics still open try to kill x64dbg.exe if there are any:
    tasklist /FI "IMAGENAME eq Mewgenics.exe" 2>NUL | %windir%\System32\find.exe /I "Mewgenics.exe" >NUL
    if %ERRORLEVEL% EQU 0 (
        echo Mewgenics.exe is still running.
        tasklist /FI "IMAGENAME eq x64dbg.exe" 2>NUL | %windir%\System32\find.exe /I "x64dbg.exe" >NUL
        if %ERRORLEVEL% EQU 0 (
            echo Killing x64dbg.exe processes...
            taskkill /IM x64dbg.exe /F >NUL 2>&1
        ) else (
            echo No x64dbg.exe processes found.
        )
        %windir%\System32\timeout.exe /T 1 /NOBREAK >NUL
    ) else (
        echo Mewgenics.exe is not running.
    )
)

rem python exclude.py
rem python find_dangerous_hooks.py
rem python gen_hooks.py

rem Remove obj and bin folders
if exist "obj" rmdir /S /Q "obj"
if exist "bin" rmdir /S /Q "bin"

if "%INCLUDE_INSTALLER%"=="true" (
    rem Create installer/mod if missing
    if not exist "installer\mod" mkdir "installer\mod"
    if not exist "installer\mod\the_spreadsheet_edmund_hates" mkdir "installer\mod\the_spreadsheet_edmund_hates"
    if not exist "installer\mod\the_spreadsheet_edmund_hates\swfs" mkdir "installer\mod\the_spreadsheet_edmund_hates\swfs"
)

if "%SKIP_BRIDGE%"=="false" (
    setlocal
    set "VSWHERE=C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe"
    rem Find the latest Visual Studio installation containing the x64 C++ tools

    if not exist "!VSWHERE!" (
        echo ERROR: vswhere.exe was not found:
        echo   "!VSWHERE!"
        exit /B 1
    )

    for /F "usebackq delims=" %%I in (`"!VSWHERE!" -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath`) do (
        set "VS_PATH=%%I"
    )

    if not defined VS_PATH (
        echo ERROR: Visual Studio with the required C++ tools was not found.
        exit /B 1
    )

    set "VCVARS=!VS_PATH!\VC\Auxiliary\Build\vcvars64.bat"

    if not exist "!VCVARS!" (
        echo ERROR: vcvars64.bat was not found:
        echo   "!VCVARS!"
        exit /B 1
    )

    rem Load the Visual Studio x64 build environment and compile the DLL
    call "!VCVARS!" || exit /B 1

    cl /LD /O2 /EHsc "MewjectorBridge.cpp" /Fe:"MewjectorBridge.dll"
    if errorlevel 1 exit /B 1

    rem Remove intermediate compiler files
    if exist "MewjectorBridge.exp" del /Q "MewjectorBridge.exp"
    if exist "MewjectorBridge.lib" del /Q "MewjectorBridge.lib"
    if exist "MewjectorBridge.obj" del /Q "MewjectorBridge.obj"

    rem Copy MewjectorBridge.dll to the installer
    copy /Y "MewjectorBridge.dll" "installer\mod\the_spreadsheet_edmund_hates\MewjectorBridge.dll" >NUL
    if errorlevel 1 exit /B 1
    echo Copied MewjectorBridge.dll to installer\mod\the_spreadsheet_edmund_hates\MewjectorBridge.dll
    @REM wait 1 second or we get "The process cannot access the file because it is being used by another process"
    %windir%\System32\timeout.exe /T 1 /NOBREAK >NUL
    endlocal
) else (
    echo Skipping MewjectorBridge build as per SKIP_BRIDGE flag.
)

if "%INCLUDE_INSTALLER%"=="true" (
    echo Compiling installer...
    "%ISCC_EXE%" "installer\MewgenicsCatStatsInstaller.iss"
    if errorlevel 1 exit /B 1
)

rem Publish the .NET mod
dotnet publish -c Release -r win-x64 --self-contained

rem print error:
if errorlevel 1 (
    echo Error occurred during dotnet publish -c Release -r win-x64 --self-contained
    exit /B 1
)

rem Copy the .NET mod to the installer
copy /Y "bin\Release\net8.0-windows\win-x64\publish\the_spreadsheet_edmund_hates.dll" "installer\mod\the_spreadsheet_edmund_hates\the_spreadsheet_edmund_hates.dll" >NUL
if errorlevel 1 (
    echo Error occurred during copy of the .NET mod to the installer.
    exit /B 1
)
echo Copied the_spreadsheet_edmund_hates.dll to installer\mod\the_spreadsheet_edmund_hates\the_spreadsheet_edmund_hates.dll

rem Copy SWF files to the installer
copy /Y "swf\house_table_stats.swf" "installer\mod\the_spreadsheet_edmund_hates\swfs\house_table_stats.swf" >NUL
if errorlevel 1 exit /B 1
echo Copied house_table_stats.swf to installer\mod\the_spreadsheet_edmund_hates\swfs\house_table_stats.swf

copy /Y "swf\swflist.gon.append" "installer\mod\the_spreadsheet_edmund_hates\swfs\swflist.gon.append" >NUL
if errorlevel 1 exit /B 1
echo Copied swflist.gon.append to installer\mod\the_spreadsheet_edmund_hates\swfs\swflist.gon.append

if "%DEVMODE%"=="true" (
    rem Copy the .NET mod to the live Mewgenics installation while live debugging
    copy /Y "bin\Release\net8.0-windows\win-x64\publish\the_spreadsheet_edmund_hates.dll" "!MEWGENICS_MODS_FOLDER!\the_spreadsheet_edmund_hates\the_spreadsheet_edmund_hates.dll" >NUL
    if errorlevel 1 (
        echo Error occurred during copy of the dll file to the live Mewgenics installation. Maybe the mod folder doesn't exist already?
        exit /B 1
    )
    echo Copied the_spreadsheet_edmund_hates.dll to !MEWGENICS_MODS_FOLDER!\the_spreadsheet_edmund_hates\the_spreadsheet_edmund_hates.dll

    rem Copy SWF files to the live install
    copy /Y "swf\house_table_stats.swf" "!MEWGENICS_MODS_FOLDER!\the_spreadsheet_edmund_hates\swfs\house_table_stats.swf" >NUL
    if errorlevel 1 exit /B 1
    echo Copied house_table_stats.swf to !MEWGENICS_MODS_FOLDER!\the_spreadsheet_edmund_hates\swfs\house_table_stats.swf

    copy /Y "swf\swflist.gon.append" "!MEWGENICS_MODS_FOLDER!\the_spreadsheet_edmund_hates\swfs\swflist.gon.append" >NUL
    if errorlevel 1 exit /B 1
    echo Copied swflist.gon.append to !MEWGENICS_MODS_FOLDER!\the_spreadsheet_edmund_hates\swfs\swflist.gon.append

    rem Example save-file copies:
    copy /Y "%APPDATA%\Glaiel Games\Mewgenics\76561198041742179\saves\steamcampaign01 - Copy.sav" "%APPDATA%\Glaiel Games\Mewgenics\76561198041742179\saves\steamcampaign01.sav" >NUL
    if errorlevel 1 exit /B 1
    rem copy /Y "%APPDATA%\Glaiel Games\Mewgenics\76561198041742179\saves\steamcampaign02 - Copy.sav" "%APPDATA%\Glaiel Games\Mewgenics\76561198041742179\saves\steamcampaign02.sav"

    rem Copy MewjectorBridge.dll to the live Mewgenics installation
    copy /Y "MewjectorBridge.dll" "!MEWGENICS_MODS_FOLDER!\the_spreadsheet_edmund_hates\MewjectorBridge.dll" >NUL
    @REM if errorlevel 1 exit /B 1

    @REM rem Wait one second to ensure memory is freed up
    @REM C:\Windows\System32\timeout.exe /T 1 /NOBREAK >NUL
    "E:\SteamLibrary\steamapps\common\Mewgenics\Mewgenics.exe"
)

echo.
echo Build completed successfully.
exit /B 0
