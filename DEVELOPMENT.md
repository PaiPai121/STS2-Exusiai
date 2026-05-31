# Exusiai Development Notes

Updated: 2026-05-31

This file is the practical developer note for the current Exusiai mod. It replaces the old template notes.

## Fixed Paths

Project:

```text
D:\work_console\MyFirstMod
```

Game mod folder:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\Exusiai
```

Runtime logs:

```text
C:\Users\HunterAndDragon\AppData\Roaming\SlayTheSpire2\logs
```

## Build

```powershell
dotnet build D:\work_console\MyFirstMod\MyFirstMod.csproj
```

The build copies `Exusiai.dll` and `Exusiai.json` to the game mod folder.

Known recurring warnings:

- Publicizer/BaseLib duplicate `IgnoresAccessChecksToAttribute`.
- `OverclockPower` nullability mismatch on the overridden cost modifier method.
- Generated Godot `Main` type warning.

These are currently expected if the build exits with 0 errors.

## Export PCK

```powershell
& 'D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe' --headless --path 'D:\work_console\MyFirstMod' --export-pack BasicExport 'D:\work_console\MyFirstMod\Exusiai.pck'
```

Then deploy:

```powershell
Copy-Item -LiteralPath 'D:\work_console\MyFirstMod\Exusiai.pck' -Destination 'C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\Exusiai\Exusiai.pck' -Force
```

Known export noise can be ignored if the command succeeds and `Exusiai.pck` is produced:

- `sts2` FileNotFoundException
- UID duplicate warnings
- missing `export_files` key in `export_presets.cfg`

## Naming Rules

- Player-facing mod name: `Exusiai`.
- Manifest id: `exusiai`.
- Deployed files: `Exusiai.dll`, `Exusiai.json`, `Exusiai.pck`.
- Keep internal `myfirstmod/` resource paths for now.
- Keep `MYFIRSTMOD-*` localization/card IDs for now.
- Do not rename IDs to `EXUSIAI-*` without a save/reference migration plan.
- `CardTemplate` is intentionally still the internal class name for Crossfire.

## Source Layout

```text
Code/
  Entry.cs
  MyFirstModCardModel.cs
  MyFirstModRelicModel.cs
  Cards/
    CardTemplate.cs          # Crossfire, kept for compatibility
    CoreCards.cs             # early/core card set and Gunspark token
    ExpandedCards.cs         # later expansion cards
    RapidFireCardModel.cs
  Powers/
  Relics/
  CardPools/
  RelicPools/
  PotionPools/
myfirstmod/
  images/cards/
  images/relics/
  images/powers/
  localization/zhs/
  localization/eng/
```

## Card Rules

1. Inherit from `MyFirstModCardModel` or `RapidFireCardModel`.
2. Register real cards with `[Pool(typeof(ExusiaiCardPool))]`.
3. Register tokens with `[Pool(typeof(TokenCardPool))]`.
4. Add card art under `myfirstmod/images/cards/{ClassName}.jpg` or `.png`.
5. Add both Chinese and English localization keys.
6. Update `CARD_LIBRARY.md` after changing card cost, rarity, values, keywords, or effect text.

Current Rapid Fire behavior:

- Original Rapid Fire cards generate one copy when played.
- The copy loses Rapid Fire.
- The copy gains Ethereal and Exhaust.
- This prevents recursive Rapid Fire loops.

Current Gunspark creation pattern:

```csharp
CardModel spark = combatState.CreateCard<Gunspark>(owner);
await CardPileCmd.AddGeneratedCardToCombat(spark, PileType.Hand, addedByPlayer: true);
```

Use `CombatGuards.HasLivingEnemy` before adding post-play generated cards or delayed effects that should not run after combat is ending.

## Localization

Localization lives in:

```text
myfirstmod/localization/zhs/
myfirstmod/localization/eng/
```

When adding or changing content, keep `zhs` and `eng` aligned. The English mode should remain playable without missing keys or Chinese fallback text.

## Assets

- Card art: `myfirstmod/images/cards/`
- Relic icons: `myfirstmod/images/relics/`
- Power icons: `myfirstmod/images/powers/`
- Character images: `myfirstmod/images/exusiai/`
- Rest-site generated art: `myfirstmod/assets/character/generated/exusiai_rest_site.png`

After adding or changing images, export and deploy `Exusiai.pck`.

Unused legacy Spine resources, old card-trail assets, and old card-frame assets were removed. Do not reintroduce them unless the corresponding runtime scene/scripts are restored and verified.

## Rest Site

Rest-site display uses `ExusiaiRestSiteCreatePatch` to create the correct runtime node. Do not switch `CustomRestSiteAnimPath` directly to an unverified custom scene; the old custom scene-root mismatch caused rest-site freezes. The old custom rest-site scene has been removed from the repo so the runtime patch remains the only active path.

## Feishu Design Source

The remote Feishu document has existed as a design reference, but local files are now the current source of truth when the remote document is stale:

- `CARD_LIBRARY.md`
- `DESIGN_SOURCE.md`
- `PROGRESS.md`

If design changes are made, update local docs first, then sync the cloud document when needed.
