# Release Checklist

## Package Contents

Release folder:

```text
Exusiai/
  Exusiai.dll
  Exusiai.json
  Exusiai.pck
```

Do not include source folders, `.godot`, logs, or development scripts in the player-facing release package.

## Required Dependency

- BaseLib 3.0.1

## Player Install Requirements

The player-facing package must be a single `Exusiai` folder containing only:

```text
Exusiai.dll
Exusiai.json
Exusiai.pck
```

Before testing an uploaded package, delete the existing local `mods/Exusiai` folder and reinstall from the packaged files. Placeholder cards with text like `If you can read this, there is a bug` usually mean the DLL, PCK, or BaseLib failed to load.

## Pre-Release Checks

```powershell
dotnet build D:\work_console\MyFirstMod\MyFirstMod.csproj
.\tools\Validate-Exusiai.ps1
```

Export and deploy the latest PCK:

```powershell
& 'D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe' --headless --path 'D:\work_console\MyFirstMod' --export-pack BasicExport 'D:\work_console\MyFirstMod\Exusiai.pck'
Copy-Item -LiteralPath 'D:\work_console\MyFirstMod\Exusiai.pck' -Destination 'D:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\Exusiai\Exusiai.pck' -Force
```

Known acceptable export noise:

- `sts2` assembly lookup exception after `Exusiai.pck` has been written.

## Manifest

Current release manifest:

- id: `exusiai`
- name: `Exusiai`
- version: `v1.0.4`
- dll: `Exusiai.dll`
- dependency: `BaseLib`

## Final Manual Checks

- Launch game with only the release files in `mods/Exusiai`.
- Confirm Exusiai appears in character select.
- Confirm one combat starts, one card reward opens, one shop opens, and one rest site opens.
- Confirm cards display localized names and art in English and Chinese.
- Confirm the public beta branch separately, or clearly mark the release as stable-branch tested only.
- Check the newest log for mod-related missing assets or exceptions.

## Nexus Upload

### Nexus Credentials

Get `NEXUS_API_KEY` from the Nexus Mods website while signed in:

- Open Nexus Mods.
- Go to account settings / preferences.
- Open the API or personal API key section.
- Copy the personal API key.

Get `NEXUS_UPDATE_GROUP_ID` from the existing published mod after the API key is available:

```powershell
$env:NEXUS_API_KEY = '<personal Nexus API key>'
.\tools\Get-NexusUpdateGroups.ps1 -ModUrl 'https://www.nexusmods.com/<game_domain>/mods/<mod_id>'
```

For repeat releases, save both values to the user-level Exusiai Nexus config file:

```powershell
.\tools\Configure-Nexus.ps1 -ApiKey '<personal Nexus API key>' -ModUrl 'https://www.nexusmods.com/<game_domain>/mods/<mod_id>'
```

The configuration script is PowerShell-based and cross-platform. It writes the config outside the repository:

- Windows: `%APPDATA%\Exusiai\nexus-release.env`
- macOS: `~/Library/Application Support/Exusiai/nexus-release.env`
- Linux: `${XDG_CONFIG_HOME:-~/.config}/exusiai/nexus-release.env`

Use `-ProcessOnly` if credentials should only be set for the current PowerShell process and not written to disk.

List the Nexus file update groups for the published mod:

```powershell
$env:NEXUS_API_KEY = '<personal Nexus API key>'
.\tools\Get-NexusUpdateGroups.ps1 -ModUrl 'https://www.nexusmods.com/<game_domain>/mods/<mod_id>'
```

Create the release zip:

```powershell
.\tools\Package-Exusiai.ps1
```

Upload the zip as a new version of the existing Nexus file update group:

```powershell
.\tools\Publish-Nexus.ps1 -ZipPath .\dist\Exusiai-v1.0.4.zip -Description 'Release notes here.'
```

Use `-ArchiveExistingFile` if the previous version should be archived when the new version is created.
Do not commit API keys or local Nexus IDs unless they are intentionally public metadata.
