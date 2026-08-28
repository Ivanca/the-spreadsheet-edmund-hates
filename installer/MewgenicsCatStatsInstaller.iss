#define MyAppName "Mewgenics Cat Stats Mod"
#define MyAppVersion "1.0.0"

[Setup]
AppId={{B9D2A6E0-4E3A-4A0A-9B35-2F6C8A5C1001}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=YourName
DefaultDirName={code:GetDefaultGameDir}
DisableProgramGroupPage=yes
Uninstallable=no
OutputBaseFilename=MewgenicsCatStatsInstaller
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
;   mod\catstable\catstable.dll
;   mod\catstable\swfs\swflist.gon.append
Source: "mod\catstable\*"; DestDir: "{app}\mods\catstable"; Flags: recursesubdirs createallsubdirs ignoreversion

; Vendor archives. These should contain their normal release contents at the ZIP root.
Source: "vendor\Mewtator.zip"; Flags: dontcopy
Source: "vendor\Mewjector.zip"; Flags: dontcopy

[Code]
var
  MewtatorDir: String;
  MewtatorPage: TInputDirWizardPage;

function FindMewgenicsInSteam: String; forward;
procedure AddSteamLibrary(var Libraries: TArrayOfString; SteamRoot: String); forward;
procedure AddUniquePath(var Paths: TArrayOfString; P: String); forward;

procedure RegisterPreviousData(PreviousDataKey: Integer);
begin
  SetPreviousData(
    PreviousDataKey,
    'GameDir',
    WizardDirValue);
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

  P := FindMewgenicsInSteam;
  if P <> '' then begin
    Result := P;
    Exit;
  end;

  Result := 'C:\Program Files (x86)\Steam\steamapps\common\Mewgenics\game';
end;

function FindMewgenicsInSteam: String;
var
  SteamPath: String;
  Libraries: TArrayOfString;
  I: Integer;
  Candidate: String;
begin
  Result := '';
  SetArrayLength(Libraries, 0);

  if RegQueryStringValue(HKEY_CURRENT_USER, 'Software\Valve\Steam', 'SteamPath', SteamPath) then
    AddSteamLibrary(Libraries, SteamPath);
  if RegQueryStringValue(HKEY_CURRENT_USER_32, 'Software\Valve\Steam', 'SteamPath', SteamPath) then
    AddSteamLibrary(Libraries, SteamPath);
  if RegQueryStringValue(HKEY_CURRENT_USER_64, 'Software\Valve\Steam', 'SteamPath', SteamPath) then
    AddSteamLibrary(Libraries, SteamPath);
  if RegQueryStringValue(HKEY_LOCAL_MACHINE, 'Software\Valve\Steam', 'InstallPath', SteamPath) then
    AddSteamLibrary(Libraries, SteamPath);
  if RegQueryStringValue(HKEY_LOCAL_MACHINE_32, 'Software\Valve\Steam', 'InstallPath', SteamPath) then
    AddSteamLibrary(Libraries, SteamPath);
  if RegQueryStringValue(HKEY_LOCAL_MACHINE_64, 'Software\Valve\Steam', 'InstallPath', SteamPath) then
    AddSteamLibrary(Libraries, SteamPath);

  for I := 0 to GetArrayLength(Libraries) - 1 do begin
    Candidate := PathCombine(PathCombine(Libraries[I], 'steamapps\common\Mewgenics'), 'game');
    if FileExists(PathCombine(Candidate, 'Mewgenics.exe')) then begin
      Result := Candidate;
      Exit;
    end;
  end;
end;

procedure AddSteamLibrary(var Libraries: TArrayOfString; SteamRoot: String);
var
  VdfPath: String;
  Lines: TArrayOfString;
  I, P1, P2: Integer;
  Line, LibPath: String;
begin
  if SteamRoot = '' then
    Exit;

  StringChangeEx(SteamRoot, '/', '\', False);
  AddUniquePath(Libraries, SteamRoot);

  VdfPath := PathCombine(PathCombine(SteamRoot, 'steamapps'), 'libraryfolders.vdf');
  if not FileExists(VdfPath) then
    Exit;

  if not LoadStringsFromFile(VdfPath, Lines) then
    Exit;

  for I := 0 to GetArrayLength(Lines) - 1 do begin
    Line := Trim(Lines[I]);
    P1 := Pos('"path"', LowerCase(Line));
    if P1 = 0 then
      Continue;

    { Find the opening quote of the path value. }
    P1 := Pos('"', Copy(Line, P1 + 6, Length(Line))) + P1 + 5;
    if P1 <= 5 then
      Continue;

    P2 := Pos('"', Copy(Line, P1 + 1, Length(Line)));
    if P2 = 0 then
      Continue;

    LibPath := Copy(Line, P1 + 1, P2 - 1);
    StringChangeEx(LibPath, '\\', '\', False);
    AddUniquePath(Libraries, LibPath);
  end;
end;

procedure AddUniquePath(var Paths: TArrayOfString; P: String);
var
  I, N: Integer;
begin
  if P = '' then
    Exit;

  P := RemoveBackslashUnlessRoot(P);
  N := GetArrayLength(Paths);

  for I := 0 to N - 1 do
    if PathSame(Paths[I], P) then
      Exit;

  SetArrayLength(Paths, N + 1);
  Paths[N] := P;
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
        'The selected folder does not appear to be the Mewgenics game folder.'#13#10#13#10 +
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

procedure InstallMewjector(GameDir: String);
var
  ZipPath, TempDir, VersionDll, VendorIni, DestIni: String;
begin
  ExtractTemporaryFile('Mewjector.zip');

  ZipPath := ExpandConstant('{tmp}\Mewjector.zip');
  TempDir := PathCombine(ExpandConstant('{tmp}'), 'MewjectorExtract');
  ForceDirectories(TempDir);
  ExtractArchive(ZipPath, TempDir, '', True, nil);

  VersionDll := PathCombine(TempDir, 'version.dll');
  VendorIni := PathCombine(TempDir, 'chainloader.ini');
  DestIni := PathCombine(GameDir, 'chainloader.ini');

  if not FileExists(VersionDll) then
    RaiseException('Mewjector.zip does not contain version.dll at its root.');

  if not CopyFile(
      VersionDll,
      PathCombine(GameDir, 'version.dll'),
      False) then
    RaiseException(
      'Could not install/update Mewjector version.dll.'#13#10#13#10 +
      'Make sure Mewgenics is closed.');

  { Never blindly overwrite an existing chainloader.ini: users may have
    custom Mewjector settings. We only add/update MewtatorManifest later. }
  if (not FileExists(DestIni)) and FileExists(VendorIni) then
    if not CopyFile(VendorIni, DestIni, False) then
      RaiseException('Could not install Mewjector chainloader.ini.');
end;

procedure InstallMewtator;
var
  ZipPath, BackupConfig: String;
  HadConfig: Boolean;
begin
  MewtatorDir := RemoveBackslashUnlessRoot(MewtatorPage.Values[0]);

  ForceDirectories(MewtatorDir);

  HadConfig := FileExists(PathCombine(MewtatorDir, 'config.json'));
  BackupConfig := PathCombine(
    ExpandConstant('{tmp}'), 'mewtator_config.json.bak');

  if HadConfig then begin
    if not CopyFile(
        PathCombine(MewtatorDir, 'config.json'),
        BackupConfig,
        False) then
      RaiseException('Could not back up Mewtator config.json.');
  end;

  ExtractTemporaryFile('Mewtator.zip');

  ZipPath := ExpandConstant('{tmp}\Mewtator.zip');
  ExtractArchive(ZipPath, MewtatorDir, '', True, nil);

  { Mewtator updates should not overwrite the user's config. }
  if HadConfig then begin
    if not CopyFile(
        BackupConfig,
        PathCombine(MewtatorDir, 'config.json'),
        False) then
      RaiseException('Could not restore Mewtator config.json.');
  end;
end;

procedure UpdateModList(GameDir: String);
var
  ModDir, ModList: String;
  Lines: TArrayOfString;
  I, N: Integer;
begin
  ModDir := PathCombine(GameDir, 'mods');
  ForceDirectories(ModDir);
  ModList := PathCombine(ModDir, 'modlist.txt');

  if FileExists(ModList) then
    LoadStringsFromFile(ModList, Lines)
  else
    SetArrayLength(Lines, 0);

  for I := 0 to GetArrayLength(Lines) - 1 do
    if SameText(Trim(Lines[I]), 'catstable') then
      Exit;

  N := GetArrayLength(Lines);
  SetArrayLength(Lines, N + 1);
  Lines[N] := 'catstable';

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

procedure UpdateManifest(GameDir: String);
var
  ModDir, Manifest, DllPath, Normalized: String;
  Lines: TArrayOfString;
  I, N: Integer;
begin
  ModDir := PathCombine(GameDir, 'mods');
  ForceDirectories(ModDir);

  Manifest := PathCombine(ModDir, 'mewtator_dll_manifest.txt');
  DllPath := PathCombine(
    PathCombine(ModDir, 'catstable'),
    'catstable.dll');

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
    '$c | Add-Member -NotePropertyName mod_folder -NotePropertyValue $ModDir -Force' + #13#10 +
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
  ModDir := PathCombine(GameDir, 'mods');

  InstallMewjector(GameDir);
  InstallMewtator;

  { The [Files] section has already installed the catstable folder. }
  UpdateModList(GameDir);
  UpdateManifest(GameDir);
  ConfigureMewtator(GameDir, ModDir);

  MsgBox(
    'Mewgenics Cat Stats Mod was installed successfully.'#13#10#13#10 +
    'Mewjector and Mewtator were installed/updated, catstable was added ' +
    'to Mewtator''s modlist, and DLL loading was enabled.',
    mbInformation, MB_OK);
end;
