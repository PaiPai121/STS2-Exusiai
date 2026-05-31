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

## Pre-Release Checks

```powershell
dotnet build D:\work_console\MyFirstMod\MyFirstMod.csproj
.\tools\Validate-Exusiai.ps1
```

Export and deploy the latest PCK:

```powershell
& 'D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe' --headless --path 'D:\work_console\MyFirstMod' --export-pack BasicExport 'D:\work_console\MyFirstMod\Exusiai.pck'
Copy-Item -LiteralPath 'D:\work_console\MyFirstMod\Exusiai.pck' -Destination 'C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\Exusiai\Exusiai.pck' -Force
```

Known acceptable export noise:

- `sts2` assembly lookup exception after `Exusiai.pck` has been written.

## Manifest

Current release manifest:

- id: `exusiai`
- name: `Exusiai`
- version: `v1.0.1`
- dll: `Exusiai.dll`
- dependency: `BaseLib`

## Final Manual Checks

- Launch game with only the release files in `mods/Exusiai`.
- Confirm Exusiai appears in character select.
- Confirm one combat starts, one card reward opens, one shop opens, and one rest site opens.
- Check the newest log for mod-related missing assets or exceptions.
