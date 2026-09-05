@echo off

cd /d "E:\SteamLibrary\steamapps\common\Mewgenics"
start "" "Mewgenics.exe" -dev_mode true -enable_debugconsole true

timeout /t 4

for /f "tokens=2 delims=," %%a in ('tasklist /fi "imagename eq Mewgenics.exe" /fo csv /nh') do (
    set "PID=%%~a"
)

start "" "D:\Downloads\Software\snapshot_2026-04-20_19-04\release\x64\x64dbg.exe" -p %PID%