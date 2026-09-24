#define MyAppName "The Spreadsheet Edmund Hates"
#define MyAppVersion "1.0.0"

[Setup]
AppId={{B9D2A6E0-4E3A-4A0A-9B35-2F6C8A5C1001}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=YourName
DefaultDirName={code:GetDefaultGameDir}
DisableProgramGroupPage=yes
DirExistsWarning=no
Uninstallable=no
OutputBaseFilename=TheSpreadsheetEdmundHates
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
ArchiveExtraction=full
CloseApplications=no

[Files]
; Your mod files:
;   mod\the_spreadsheet_edmund_hates\the_spreadsheet_edmund_hates.dll
;   mod\the_spreadsheet_edmund_hates\MewjectorBridge.dll
;   mod\the_spreadsheet_edmund_hates\swfs\swflist.gon.append
Source: "mod\the_spreadsheet_edmund_hates\*"; DestDir: "{code:GetModInstallDir}\the_spreadsheet_edmund_hates"; Flags: recursesubdirs createallsubdirs ignoreversion

; Vendor archives.
; Mewjector: Release-218-3-3-1778034265.zip\release\...
; Mewtator: Mewtator_Windows0.6.0.zip\Mewtator\...
Source: "vendor\Release-218-3-3-1778034265.zip"; Flags: dontcopy
Source: "vendor\Mewtator_Windows0.6.0.zip"; Flags: dontcopy
Source: "Find-Mewgenics.ps1"; Flags: dontcopy

[Code]
var
  MewtatorDir: String;
  MewtatorPage: TInputDirWizardPage;

function FindMewgenicsWithPowerShell: String; forward;

function FindMewgenicsWithPowerShell: String;
var
  ScriptPath, ResultPath, Command, Params: String;
  ResultCode: Integer;
  Lines: TArrayOfString;
begin
  Result := '';

  ExtractTemporaryFile('Find-Mewgenics.ps1');

  ScriptPath := ExpandConstant('{tmp}\Find-Mewgenics.ps1');
  ResultPath := ExpandConstant('{tmp}\Find-Mewgenics.result');
  DeleteFile(ResultPath);

  Command :=
    '$result = & ' + AddQuotes(ScriptPath) + '; ' +
    '$code = $LASTEXITCODE; ' +
    '$result | Set-Content -LiteralPath ' + AddQuotes(ResultPath) +
    ' -Encoding UTF8; exit $code';

  Params :=
    '-NoProfile -ExecutionPolicy Bypass -Command ' + AddQuotes(Command);

  if not Exec(
      ExpandConstant('{sysnative}\WindowsPowerShell\v1.0\powershell.exe'),
      Params,
      ExpandConstant('{tmp}'),
      SW_HIDE,
      ewWaitUntilTerminated,
      ResultCode) then
    Exit;

  if ResultCode <> 0 then begin
    DeleteFile(ResultPath);
    Exit;
  end;

  if not FileExists(ResultPath) then
    Exit;

  if not LoadStringsFromFile(ResultPath, Lines) then
    Exit;

  if GetArrayLength(Lines) > 0 then
    Result := Trim(Lines[0]);

  DeleteFile(ResultPath);
end;

function GetDefaultGameDir(Param: String): String;
var
  P: String;
begin
  P := GetPreviousData('GameDir', '');
  if (P <> '') and FileExists(PathCombine(P, 'Mewgenics.exe')) then begin
    Result := P;
    Exit;
  end;

  P := FindMewgenicsWithPowerShell;
  if P <> '' then begin
    Result := P;
    Exit;
  end;

  MsgBox(
    'Couldn''t find Mewgenics game, you may try to manually select it instead.',
    mbInformation, MB_OK);

  Result := 'C:\Program Files (x86)\Steam\steamapps\common\Mewgenics';
end;

function ReadModFolderFromConfig(ConfigPath: String): String;
var
  Lines: TArrayOfString;
  I, P, StartPos, EndPos: Integer;
  Line, Value: String;
begin
  Result := '';

  if not FileExists(ConfigPath) then
    Exit;

  if not LoadStringsFromFile(ConfigPath, Lines) then
    Exit;

  for I := 0 to GetArrayLength(Lines) - 1 do begin
    Line := Trim(Lines[I]);
    P := Pos('"mod_folder"', Line);

    if P = 0 then
      Continue;

    P := Pos(':', Line);
    if P = 0 then
      Continue;

    Value := Trim(Copy(Line, P + 1, Length(Line)));

    if (Length(Value) >= 2) and
       (Value[1] = '"') then begin
      StartPos := 2;
      EndPos := StartPos;
      while (EndPos <= Length(Value)) and (Value[EndPos] <> '"') do
        Inc(EndPos);

      if EndPos > StartPos then
        Result := Copy(Value, StartPos, EndPos - StartPos);
    end;

    if Result <> '' then
      Exit;
  end;
end;

function GetConfiguredModsDir: String;
var
  ConfigPath, ConfigModsDir: String;
begin
  Result := '';

  { If the user selected an existing Mewtator installation, its config.json
    is authoritative for where mods belong. }
  if MewtatorPage <> nil then begin
    ConfigPath := PathCombine(
      RemoveBackslashUnlessRoot(MewtatorPage.Values[0]),
      'config.json');

    ConfigModsDir := ReadModFolderFromConfig(ConfigPath);
    if ConfigModsDir <> '' then begin
      Result := RemoveBackslashUnlessRoot(ConfigModsDir);
      Exit;
    end;
  end;
end;

function GetModInstallDir(Param: String): String;
var
  ConfiguredModsDir: String;
begin
  ConfiguredModsDir := GetConfiguredModsDir;

  if ConfiguredModsDir <> '' then begin
    Result := ConfiguredModsDir;
    Exit;
  end;

  { New Mewtator installation / no config yet: use the game's mods folder. }
  Result := PathCombine(WizardDirValue, 'mods');
end;

function GetExistingModsDir(GameDir: String): String;
var
  ConfiguredModsDir: String;
begin
  { When Mewtator already exists, its config.json is authoritative. }
  ConfiguredModsDir := GetConfiguredModsDir;
  if ConfiguredModsDir <> '' then
    Result := ConfiguredModsDir
  else
    Result := PathCombine(GameDir, 'mods');

  if not DirExists(Result) then
    ForceDirectories(Result);
end;

function FindExistingMewtatorDir(GameDir: String): String;
begin
  { Common layouts: game\Mewtator.exe or game\Mewtator\Mewtator.exe. }
  if FileExists(PathCombine(GameDir, 'Mewtator.exe')) then begin
    Result := GameDir;
    Exit;
  end;

  if FileExists(PathCombine(GameDir, 'Mewtator\Mewtator.exe')) then begin
    Result := PathCombine(GameDir, 'Mewtator');
    Exit;
  end;

  Result := '';
end;

procedure InitializeWizard;
var
  Existing: String;
begin
  MewtatorPage := CreateInputDirPage(
    wpSelectDir,
    'Mewtator location',
    'Where is Mewtator installed?',
    'If Mewtator is already installed, select its existing directory so it can be updated. If it is not installed, leave the suggested directory and it will be installed there.',
    False,
    '');

  MewtatorPage.Add('Mewtator directory:');

  Existing := FindExistingMewtatorDir(WizardDirValue);
  if Existing <> '' then
    MewtatorPage.Values[0] := Existing
  else
    MewtatorPage.Values[0] := PathCombine(WizardDirValue, 'Mewtator');
end;

procedure CurPageChanged(CurPageID: Integer);
var
  Existing: String;
begin
  if CurPageID = MewtatorPage.ID then begin
    Existing := FindExistingMewtatorDir(WizardDirValue);

    if Existing <> '' then
      MewtatorPage.Values[0] := Existing
    else if MewtatorPage.Values[0] = '' then
      MewtatorPage.Values[0] := PathCombine(WizardDirValue, 'Mewtator');
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;

  if CurPageID = wpSelectDir then begin
    if not FileExists(PathCombine(WizardDirValue, 'Mewgenics.exe')) then begin
      MsgBox(
        'The selected folder does not appear to be the Mewgenics installation folder.'#13#10#13#10 +
        'Select the folder containing Mewgenics.exe.',
        mbError, MB_OK);
      Result := False;
      Exit;
    end;

  end
  else if CurPageID = MewtatorPage.ID then begin
    MewtatorDir := RemoveBackslashUnlessRoot(MewtatorPage.Values[0]);

    if MewtatorDir = '' then begin
      MsgBox('Please select a Mewtator directory.', mbError, MB_OK);
      Result := False;
    end;
  end;
end;

procedure RegisterPreviousData(PreviousDataKey: Integer);
begin
  SetPreviousData(PreviousDataKey, 'GameDir', WizardDirValue);
end;

function CopyDirectoryContents(SourceDir, DestDir: String): Boolean;
var
  FindData: TFindRec;
  SourcePath, DestPath: String;
begin
  Result := False;

  if not ForceDirectories(DestDir) then
    Exit;

  if FindFirst(SourceDir + '\*', FindData) then
  try
    repeat
      if (FindData.Name = '.') or (FindData.Name = '..') then
        Continue;

      SourcePath := AddBackslash(SourceDir) + FindData.Name;
      DestPath := AddBackslash(DestDir) + FindData.Name;

      if FindData.Attributes and FILE_ATTRIBUTE_DIRECTORY <> 0 then begin
        if not CopyDirectoryContents(SourcePath, DestPath) then
          Exit;
      end
      else begin
        if not CopyFile(SourcePath, DestPath, False) then
          Exit;
      end;
    until not FindNext(FindData);
  finally
    FindClose(FindData);
  end;

  Result := True;
end;

procedure InstallMewjector(GameDir: String);
var
  ZipPath, TempDir, ReleaseDir, VendorIni, DestIni, BackupIni: String;
  HadIni: Boolean;
begin
  ExtractTemporaryFile('Release-218-3-3-1778034265.zip');

  ZipPath := ExpandConstant('{tmp}\Release-218-3-3-1778034265.zip');
  TempDir := PathCombine(ExpandConstant('{tmp}'), 'MewjectorExtract');
  ReleaseDir := PathCombine(TempDir, 'release');
  ForceDirectories(TempDir);
  ExtractArchive(ZipPath, TempDir, '', True, nil);

  if not FileExists(PathCombine(ReleaseDir, 'version.dll')) then
    RaiseException(
      'Release-218-3-3-1778034265.zip does not contain release\version.dll.');

  VendorIni := PathCombine(ReleaseDir, 'chainloader.ini');
  DestIni := PathCombine(GameDir, 'chainloader.ini');
  BackupIni := PathCombine(ExpandConstant('{tmp}'), 'chainloader.ini.bak');
  HadIni := FileExists(DestIni);

  if HadIni then
    if not CopyFile(DestIni, BackupIni, False) then
      RaiseException('Could not back up the existing chainloader.ini.');

  { Install every file from release\, not just version.dll. }
  if not CopyDirectoryContents(ReleaseDir, GameDir) then
    RaiseException(
      'Could not install/update the Mewjector files.'#13#10#13#10 +
      'Make sure Mewgenics is closed.');

  { Preserve an existing chainloader.ini, including custom settings. }
  if HadIni then begin
    if not CopyFile(BackupIni, DestIni, False) then
      RaiseException('Could not restore the existing chainloader.ini.');
  end
  else if not FileExists(DestIni) and FileExists(VendorIni) then
    if not CopyFile(VendorIni, DestIni, False) then
      RaiseException('Could not install Mewjector chainloader.ini.');
end;

procedure InstallMewtator;
var
  ZipPath, TempDir, SourceDir, BackupConfig: String;
  HadConfig: Boolean;
begin
  MewtatorDir := RemoveBackslashUnlessRoot(MewtatorPage.Values[0]);
  ForceDirectories(MewtatorDir);

  { If Mewtator is already installed, NEVER overwrite it.
    Its existing config.json and executable must remain untouched. }
  if FileExists(PathCombine(MewtatorDir, 'Mewtator.exe')) then
    Exit;

  HadConfig := FileExists(PathCombine(MewtatorDir, 'config.json'));
  BackupConfig := PathCombine(
    ExpandConstant('{tmp}'), 'mewtator_config.json.bak');

  if HadConfig then
    if not CopyFile(
        PathCombine(MewtatorDir, 'config.json'),
        BackupConfig,
        False) then
      RaiseException('Could not back up Mewtator config.json.');

  ExtractTemporaryFile('Mewtator_Windows0.6.0.zip');

  ZipPath := ExpandConstant(
    '{tmp}\Mewtator_Windows0.6.0.zip');
  TempDir := PathCombine(ExpandConstant('{tmp}'), 'MewtatorExtract');
  SourceDir := PathCombine(TempDir, 'Mewtator');
  ForceDirectories(TempDir);
  ExtractArchive(ZipPath, TempDir, '', True, nil);

  if not FileExists(PathCombine(SourceDir, 'Mewtator.exe')) then
    RaiseException(
      'Mewtator_Windows0.6.0.zip does not contain ' +
      'Mewtator\Mewtator.exe.');

  if not CopyDirectoryContents(SourceDir, MewtatorDir) then
    RaiseException(
      'Could not install Mewtator files.'#13#10#13#10 +
      'Make sure Mewtator is not running.');

  { Preserve the user's existing config if one somehow existed
    even though Mewtator.exe did not. }
  if HadConfig then
    if not CopyFile(
        BackupConfig,
        PathCombine(MewtatorDir, 'config.json'),
        False) then
      RaiseException('Could not restore Mewtator config.json.');
end;

procedure UpdateModList(ModDir: String);
var
  ModList: String;
  Lines: TArrayOfString;
  I, N: Integer;
begin
  ModList := PathCombine(ModDir, 'modlist.txt');

  if FileExists(ModList) then
    LoadStringsFromFile(ModList, Lines)
  else
    SetArrayLength(Lines, 0);

  for I := 0 to GetArrayLength(Lines) - 1 do
    if SameText(Trim(Lines[I]), 'the_spreadsheet_edmund_hates') then
      Exit;

  N := GetArrayLength(Lines);
  SetArrayLength(Lines, N + 1);
  Lines[N] := 'the_spreadsheet_edmund_hates';

  if not SaveStringsToUTF8FileWithoutBOM(ModList, Lines, False) then
    RaiseException('Could not update ' + ModList);
end;

procedure ConfigureChainloader(GameDir, Manifest: String);
var
  IniPath, ManifestPath: String;
begin
  IniPath := PathCombine(GameDir, 'chainloader.ini');

  if not FileExists(IniPath) then
    Exit;

  ManifestPath := Manifest;
  StringChangeEx(ManifestPath, '\', '/', False);

  if not SetIniString(
      'Chainloader',
      'MewtatorManifest',
      ManifestPath,
      IniPath) then
    RaiseException('Could not update ' + IniPath);
end;

procedure UpdateManifest(GameDir, ModDir: String);
var
  Manifest, DllPath, Normalized: String;
  Lines: TArrayOfString;
  I, N: Integer;
begin

  Manifest := PathCombine(ModDir, 'mewtator_dll_manifest.txt');
  DllPath := PathCombine(
    PathCombine(ModDir, 'the_spreadsheet_edmund_hates'),
    'MewjectorBridge.dll');

  Normalized := DllPath;
  StringChangeEx(Normalized, '\', '/', False);

  if FileExists(Manifest) then
    LoadStringsFromFile(Manifest, Lines)
  else
    SetArrayLength(Lines, 0);

  for I := 0 to GetArrayLength(Lines) - 1 do
    if SameText(Trim(Lines[I]), Normalized) then begin
      ConfigureChainloader(GameDir, Manifest);
      Exit;
    end;

  N := GetArrayLength(Lines);
  SetArrayLength(Lines, N + 1);
  Lines[N] := Normalized;

  if not SaveStringsToUTF8FileWithoutBOM(
      Manifest, Lines, False) then
    RaiseException('Could not update ' + Manifest);

  ConfigureChainloader(GameDir, Manifest);
end;

function GetMewtatorModFolder(MewtatorDir, GameDir: String): String;
var
  ConfigPath, ResultPath, Command, Params: String;
  ResultCode: Integer;
  Lines: TArrayOfString;
begin
  Result := '';

  ConfigPath := PathCombine(MewtatorDir, 'config.json');
  if not FileExists(ConfigPath) then begin
    Result := PathCombine(GameDir, 'mods');
    Exit;
  end;

  ResultPath := ExpandConstant('{tmp}\mewtator_mod_folder.result');
  DeleteFile(ResultPath);

  Command :=
    '$c = Get-Content -LiteralPath ' + AddQuotes(ConfigPath) +
    ' -Raw | ConvertFrom-Json; ' +
    '$c.mod_folder | Set-Content -LiteralPath ' + AddQuotes(ResultPath) +
    ' -Encoding UTF8';

  Params :=
    '-NoProfile -ExecutionPolicy Bypass -Command ' + AddQuotes(Command);

  if not Exec(
      ExpandConstant('{sysnative}\WindowsPowerShell\v1.0\powershell.exe'),
      Params,
      ExpandConstant('{tmp}'),
      SW_HIDE,
      ewWaitUntilTerminated,
      ResultCode) then begin
    Result := PathCombine(GameDir, 'mods');
    Exit;
  end;

  if (ResultCode = 0) and FileExists(ResultPath) then begin
    if LoadStringsFromFile(ResultPath, Lines) then
      if GetArrayLength(Lines) > 0 then
        Result := Trim(Lines[0]);
  end;

  DeleteFile(ResultPath);

  if Result = '' then
    Result := PathCombine(GameDir, 'mods');
end;

procedure ConfigureMewtator(GameDir, ModDir: String);
var
  ScriptPath, Params, ScriptResult: String;
  ResultCode: Integer;
  PS: String;
begin
  ScriptPath := PathCombine(
    ExpandConstant('{tmp}'),
    'configure_mewtator.ps1');

  PS :=
    'param([string]$ConfigPath,[string]$GameDir,[string]$ModDir)' + #13#10 +
    'if (Test-Path -LiteralPath $ConfigPath) {' + #13#10 +
    '  try {' + #13#10 +
    '    $c = Get-Content -LiteralPath $ConfigPath -Raw | ConvertFrom-Json' + #13#10 +
    '  } catch {' + #13#10 +
    '    $c = [pscustomobject]@{}' + #13#10 +
    '  }' + #13#10 +
    '} else {' + #13#10 +
    '  $c = [pscustomobject]@{}' + #13#10 +
    '}' + #13#10 +
    'if (-not $c) { $c = [pscustomobject]@{} }' + #13#10 +
    '$c | Add-Member -NotePropertyName game_install_dir -NotePropertyValue $GameDir -Force' + #13#10 +
    'if (-not $c.mod_folder -or -not (Test-Path -LiteralPath $c.mod_folder -PathType Container)) {' + #13#10 +
    '  $c | Add-Member -NotePropertyName mod_folder -NotePropertyValue $ModDir -Force' + #13#10 +
    '}' + #13#10 +
    '$c | Add-Member -NotePropertyName dll_injection_enabled -NotePropertyValue $true -Force' + #13#10 +
    'if (-not $c.language) { $c | Add-Member -NotePropertyName language -NotePropertyValue "English" -Force }' + #13#10 +
    '$json = $c | ConvertTo-Json -Depth 10' + #13#10 +
    '[IO.File]::WriteAllText($ConfigPath, $json, (New-Object Text.UTF8Encoding($false)))' + #13#10;

  if not SaveStringToFile(
      ScriptPath, Utf8Encode(PS), False) then
    RaiseException('Could not create Mewtator configuration script.');

  Params :=
    '-NoProfile -ExecutionPolicy Bypass -File ' + AddQuotes(ScriptPath) +
    ' -ConfigPath ' + AddQuotes(PathCombine(MewtatorDir, 'config.json')) +
    ' -GameDir ' + AddQuotes(GameDir) +
    ' -ModDir ' + AddQuotes(ModDir);

  if not Exec(
      ExpandConstant('{sysnative}\WindowsPowerShell\v1.0\powershell.exe'),
      Params,
      ExpandConstant('{tmp}'),
      SW_HIDE,
      ewWaitUntilTerminated,
      ResultCode) then
    RaiseException('Could not run PowerShell to configure Mewtator.');

  if ResultCode <> 0 then begin
    ScriptResult := 'PowerShell returned exit code ' + IntToStr(ResultCode) + '.';
    RaiseException('Mewtator configuration failed.'#13#10#13#10 + ScriptResult);
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  GameDir, ModDir: String;
begin
  if CurStep <> ssPostInstall then
    Exit;

  GameDir := WizardDirValue;

  { Install/update Mewjector as before. }
  InstallMewjector(GameDir);

  { If Mewtator.exe already exists, InstallMewtator exits immediately and
    leaves the existing Mewtator installation untouched. }
  InstallMewtator;

  { Resolve the mod folder from the existing/new Mewtator config. }
  ModDir := GetMewtatorModFolder(MewtatorDir, GameDir);

  { Install this mod into the Mewtator-configured mod folder. }
  { Update only the mod metadata in that same configured folder. }
  UpdateModList(ModDir);
  UpdateManifest(GameDir, ModDir);

  { Configure only the Mewtator settings required by this mod. }
  ConfigureMewtator(GameDir, ModDir);

  MsgBox(
    'Mewgenics Cat Stats Mod was installed successfully.'#13#10#13#10 +
    'Happy cat hoarding!',
    mbInformation, MB_OK);
end;

