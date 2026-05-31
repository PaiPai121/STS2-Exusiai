# Changelog

## v1.0.3 - 2026-05-31

### Fixed

- Added fallback localization for Exhaust, Ethereal, and keyword punctuation so cards like Quick Magazine no longer show raw keyword keys.

## v1.0.2 - 2026-05-31

### Fixed

- Packaged localization tables under the root `localization` path so custom keyword titles and descriptions, including Rapid Fire, resolve in-game.
- Added validation for the root localization mirror used by the exported PCK.

## v1.0.1 - 2026-05-31

### Fixed

- Removed duplicated auto-keyword text from card descriptions, including Rapid Fire, Exhaust, and Ethereal.
- Cleaned Barrage Fire's displayed description so the card text only describes its damage effect while Rapid Fire appears as a keyword.

## v1.0.0 - 2026-05-31

Initial playable Exusiai release candidate.

### Added

- Playable Exusiai character with custom card pool, starting deck, relic pool, and potion pool.
- Rapid Fire cards create one temporary copy; generated copies gain Ethereal and Exhaust and cannot recursively trigger Rapid Fire.
- Gunspark token engine with supported generation from cards, relics, and Power synergies.
- Power package: Angel's Blessing, Sweep Mode, Spark Circuit, Fire Control, Chain Reaction, and Overclock.
- Custom relics: Sniper's Chipset, Spark Capacitor, Tactical Magazine, and Reticle Calibrator.
- Chinese and English localization for cards, powers, relics, keywords, character text, and ancient dialogue.
- Custom card portraits, relic icons, Power icons, character select visuals, combat visuals, merchant visuals, and rest-site visuals.

### Fixed

- Rest-site freeze caused by the old custom rest scene path.
- Delivery Guaranteed freeze after selecting cards from discard.
- Overclock attack cost refresh behavior.
- Gunspark combat ownership and generated-card registration.
- Duplicate Godot card import UIDs and stale export resource warnings.
- Player-facing naming so manifest, DLL, PCK, and deployed folder use `Exusiai`.

### Changed

- Balance pass for draw-heavy cards, Gunspark generation, Overclock, Sweep Mode, Warfarin's Plasma, Quick Magazine, and Angelic Reload.
- Removed unused template files, risky legacy scenes, stale visual resources, duplicate portraits, and unused raw animation assets.
- Added `tools/Validate-Exusiai.ps1` for static localization, art, and resource-path checks.

### Known Notes

- Internal resource paths still use `myfirstmod/`, and localization IDs still use `MYFIRSTMOD-*` for compatibility.
- `CardTemplate` remains the internal class name for Crossfire.
- The headless exporter may print an `sts2` assembly lookup exception after a successful PCK export; this is expected when `Exusiai.pck` is produced.
