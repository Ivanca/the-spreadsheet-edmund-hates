#!/usr/bin/env bash
set -e

# Force kill any procceses of Mewgenics.exe if there are any
# dont show error if there are none
if ps -W | grep "Mewgenics.exe"; then
    echo "Killing Mewgenics.exe processes..."
    taskkill //IM Mewgenics.exe //F || true
else
    echo "No Mewgenics.exe processes found."
fi

# python exclude.py
# python find_dangerous_hooks.py
# python gen_hooks.py
dotnet publish -c Release -r win-x64 --self-contained
# "D:\Games\Mewgenics.v1.1\game\Mewgenics.exe"
# cp -f "bin/Release/net8.0-windows/win-x64/publish/catstable.dll" "/d/Games/Mewgenics.v1.1/game/Mods/catstable/catstable.dll"
cp -f "bin/Release/net8.0-windows/win-x64/publish/catstable.dll" "installer/mod/catstable.dll"
echo "Copied catstable.dll to installer/mod/catstable/catstable.dll"
cp -f "bin/Release/net8.0-windows/win-x64/publish/catstable.dll" "/e/SteamLibrary/steamapps/common/Mewgenics/mods/catstable/catstable.dll" # while live debugging

cp -f "swf/house_table_stats.swf" "installer/mod/catstable/swfs/house_table_stats.swf"
echo "Copied house_table_stats.swf to installer/mod/catstable/swfs/house_table_stats.swf"
cp -f "swf/swflist.gon.append" "installer/mod/catstable/swfs/swflist.gon.append"
echo "Copied swflist.gon.append to installer/mod/catstable/swfs/swflist.gon.append"
# cp -f "/c/Users/ivanc/AppData/Roaming/Glaiel Games/Mewgenics/76561197960287930/saves/steamcampaign01 - Copy.sav" "/c/Users/ivanc/AppData/Roaming/Glaiel Games/Mewgenics/76561197960287930/saves/steamcampaign01.sav"
# cp -f "/c/Users/ivanc/AppData/Roaming/Glaiel Games/Mewgenics/76561197960287930/saves/steamcampaign02 - Copy.sav" "/c/Users/ivanc/AppData/Roaming/Glaiel Games/Mewgenics/76561197960287930/saves/steamcampaign02.sav"

# wait one second to ensure memory is freed up
sleep 1

# clean log.txt
echo "" > log.txt

# run the game in new separate cmd window
# mintty bash -mc "cd /d/Games/Mewgenics.v1.1/game && start ./Mewgenics.exe -enable_debugconsole true -modpaths "D:/Games/Mewgenics.v1.1/game/mods/catstable""
