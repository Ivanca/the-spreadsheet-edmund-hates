$ErrorActionPreference = "SilentlyContinue"

$ExeSize  = [int64]21981184
$GpakSize = [int64]5034503564

function Find-ResourcesGpak {
    param(
        [Parameter(Mandatory)]
        [string]$Root
    )

    if (-not (Test-Path -LiteralPath $Root -PathType Container)) {
        return
    }

    $directories = [System.Collections.Generic.Stack[string]]::new()
    $directories.Push($Root)

    while ($directories.Count -gt 0) {
        $directory = $directories.Pop()

        # Look for resources.gpak in this directory.
        try {
            foreach ($path in [System.IO.Directory]::EnumerateFiles(
                $directory,
                "resources.gpak",
                [System.IO.SearchOption]::TopDirectoryOnly
            )) {
                try {
                    $gpak = [System.IO.FileInfo]::new($path)

                    if ($gpak.Length -ne $GpakSize) {
                        continue
                    }

                    # resources.gpak must have Mewgenics.exe next to it.
                    $exePath = Join-Path $gpak.DirectoryName "Mewgenics.exe"

                    if (-not [System.IO.File]::Exists($exePath)) {
                        continue
                    }

                    try {
                        $exe = [System.IO.FileInfo]::new($exePath)

                        if ($exe.Length -eq $ExeSize) {
                            return $exe.DirectoryName
                        }
                    }
                    catch {
                        continue
                    }
                }
                catch {
                    continue
                }
            }
        }
        catch {
            # Cannot enumerate this directory's files.
        }

        # Enumerate subdirectories.
        try {
            foreach ($subdirectory in [System.IO.Directory]::EnumerateDirectories(
                $directory,
                "*",
                [System.IO.SearchOption]::TopDirectoryOnly
            )) {
                try {
                    $attributes = [System.IO.File]::GetAttributes($subdirectory)

                    # Don't follow junctions, symbolic links, etc.
                    if (($attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) {
                        continue
                    }

                    # At the root of a drive, skip directories that are
                    # irrelevant to finding a Steam installation.
                    if ($directory -match '^[A-Za-z]:\\$') {
                        $leaf = [System.IO.Path]::GetFileName(
                            $subdirectory.TrimEnd('\')
                        )

                        if ($leaf -in @(
                            "Windows",
                            "Users",
                            "ProgramData",
                            "System Volume Information",
                            "WindowsApps"
                        )) {
                            continue
                        }
                    }

                    $directories.Push($subdirectory)
                }
                catch {
                    continue
                }
            }
        }
        catch {
            continue
        }
    }
}

# ============================================================
# 1. Search X:\SteamLibrary on every filesystem drive
# ============================================================

try {
    $drives = [System.IO.DriveInfo]::GetDrives() |
        Where-Object {
            $_.IsReady -and (
                $_.DriveType -eq [System.IO.DriveType]::Fixed -or
                $_.DriveType -eq [System.IO.DriveType]::Network
            )
        }

    foreach ($drive in $drives) {
        $steamLibrary = Join-Path $drive.RootDirectory.FullName "SteamLibrary"

        $result = Find-ResourcesGpak -Root $steamLibrary

        if ($null -ne $result -and $result -ne "") {
            Write-Output $result
            exit 0
        }
    }
}
catch {
    # Continue to the next search strategy.
}

# ============================================================
# 2. Search the default Steam installation directly
# ============================================================

$defaultMewgenicsPath =
    "C:\Program Files (x86)\Steam\steamapps\common\Mewgenics"

$result = Find-ResourcesGpak -Root $defaultMewgenicsPath

if ($null -ne $result -and $result -ne "") {
    Write-Output $result
    exit 0
}

# ============================================================
# 3. Full filesystem search
# ============================================================

try {
    foreach ($drive in $drives) {
        $result = Find-ResourcesGpak -Root $drive.RootDirectory.FullName

        if ($null -ne $result -and $result -ne "") {
            Write-Output $result
            exit 0
        }
    }
}
catch {
    # Not found.
}

# ============================================================
# Not found
# ============================================================

exit 1