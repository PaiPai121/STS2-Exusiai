# Exusiai Current Progress

Updated: 2026-05-31 23:20 Asia/Shanghai

## Project State

- Public mod name, manifest, deployed folder, DLL, manifest, and PCK now use `Exusiai`.
- Internal resource paths under `myfirstmod/`, localization IDs under `MYFIRSTMOD-*`, and the internal `CardTemplate` class name are intentionally retained for compatibility with existing references and saves.
- Current deployed folder:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\Exusiai
```

- Expected deployed files:

```text
Exusiai.dll
Exusiai.json
Exusiai.pck
```

## Completed

- Rapid Fire now generates a one-time copy of the played card. Generated copies lose Rapid Fire and gain Ethereal plus Exhaust to prevent loops.
- Gunspark generation is restored and uses combat-owned card creation.
- Delivery Guaranteed no longer freezes after selecting from discard, and generated copies are free for the current turn.
- Overclock is now a Power: the current turn's first 2/3 attack cards become free, then the window refreshes every other turn.
- Rest-site visuals are patched at runtime so Exusiai appears instead of Ironclad, using the generated seated rest image.
- Card art was cropped and corrected for the wider in-game card frame. Known white-line issues were fixed on the affected cards.
- Spark Circuit card art was redone to show Exusiai.
- Sniper Chipset art was restored to the user-provided original Arknights asset.
- English localization is aligned with the active Chinese keys for cards, powers, relics, keywords, character text, and Watcher text.
- Chinese localization JSON files were rebuilt and validated against the English key set for cards, relics, character text, and Watcher text.
- New relics are implemented and in the relic pool:
  - `SparkCapacitor`
  - `TacticalMagazine`
  - `ReticleCalibrator`
- Status icons are implemented for the formal Powers:
  - `AngelsBlessingPower`
  - `SweepModePower`
  - `SparkCircuitPower`
  - `FireControlPower`
  - `OverclockPower`
  - `ChainReactionPower`
- Character, card portrait, energy, rest-site, relic, and Power assets are preloaded to reduce runtime cache warnings.
- Feishu design documentation was synced from the local current design. Remote revision recorded earlier: `46`.
- README was rewritten from template content into the current Exusiai project documentation.
- Stale template leftovers were removed:
  - deleted unused `RelicTemplate.cs`
  - deleted unused `RelicTemplate.cs.uid`
  - deleted orphan `TestRelic.cs.uid`
  - deleted orphan `TestCard.cs.uid`
  - renamed misleading `PlaceholderCards.cs` to `CoreCards.cs`
- Source maintenance cleanup completed:
  - cleaned mojibake comments in `Exusiai.cs`
  - cleaned mojibake comments in `RapidFireCardModel.cs`
  - removed empty character hook overrides that had no behavior
- Removed unused risky scenes:
  - deleted `myfirstmod/scenes/ui/card_trail_exusiai.tscn`, which referenced missing old `InesCardTrail` scripts
  - deleted old `myfirstmod/scenes/character/exusiai_rest_site.tscn`; rest site now uses the runtime patch only
- Removed unused legacy Spine and card-trail assets that were only producing export warnings.
- Removed unused legacy card-frame assets that referenced external `InesSilent` resources and missing exported `.res` files.
- Fixed duplicate card-image Godot import UIDs.
- Added the missing `export_files` key to `export_presets.cfg`, leaving only the known `sts2` assembly lookup noise during export.

## Current Card Pool Notes

- Current implemented card count: 31.
- Broad split from the last audit:
  - Basic: 2
  - Common: 13
  - Uncommon: 10
  - Rare: 6
  - Attack: 15
  - Skill: 12
  - Power: 4
- Common cards are already relatively dense. Further card expansion should prefer uncommon/rare mechanics or event/relic support instead of adding more common attacks.

## Verification

Latest build:

```powershell
dotnet build D:\work_console\MyFirstMod\MyFirstMod.csproj
```

Result: success, 3 existing warnings, 0 errors.

Known recurring build warnings:

- `IgnoresAccessChecksToAttribute` duplicate warnings from Publicizer/BaseLib.
- Generated Godot `Main` type warning.

## Export Command

```powershell
& 'D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe' --headless --path 'D:\work_console\MyFirstMod' --export-pack BasicExport 'D:\work_console\MyFirstMod\Exusiai.pck'
```

Known export noise can be ignored if the command exits successfully and `Exusiai.pck` is produced:

- `sts2` FileNotFoundException

## Deploy Command

```powershell
Copy-Item -LiteralPath 'D:\work_console\MyFirstMod\Exusiai.pck' -Destination 'C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\Exusiai\Exusiai.pck' -Force
```

`dotnet build` already copies the DLL and manifest through the project build flow.

## Runtime Log Path

```text
C:\Users\HunterAndDragon\AppData\Roaming\SlayTheSpire2\logs
```

## Still Worth Testing In Game

1. Overclock: verify attack cards become free immediately after playing the Power, and again every other turn.
2. Delivery Guaranteed: verify selected discard cards enter hand as 0-cost generated copies and do not freeze.
3. Sweep Mode: verify the Power icon appears and each attack damages all enemies by the correct amount.
4. Warfarin's Plasma: verify HP loss, draw, and Gunspark generation all work without combat-end freezes.
5. New relics: verify Spark Capacitor, Tactical Magazine, and Reticle Calibrator trigger exactly as described.
6. Rest site: verify Exusiai still appears correctly after multiple runs and does not freeze.
7. Card art: spot-check remaining crops in reward, shop, and combat hand views.
8. English mode: run one short game and confirm no missing localization keys appear.

## Next Suggestions

1. Commit the current cleanup after review.
2. Do one in-game smoke pass focused on the still-worth-testing list.
3. If gameplay is stable, the next work should be polish rather than broad balance churn:
   - fix remaining card-art crop issues if found
   - clean any remaining runtime log warnings that point to real missing assets
   - consider a real animated combat/rest character asset later

## Maintenance Rules

- Update `CARD_LIBRARY.md` whenever card cost, rarity, keywords, values, or effects change.
- Update `DESIGN_SOURCE.md` when the source-of-truth policy or cloud-document status changes.
- Update this file at phase boundaries, not after every tiny edit.
- Re-export PCK after asset, localization, Godot import, scene, or image changes.
- C#-only changes usually need only `dotnet build`, unless the change affects exported resources.
