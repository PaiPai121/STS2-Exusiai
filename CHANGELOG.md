# Changelog

## Unreleased

### Added

- Added the 2026-06-04 second-wave mechanism package, expanding the obtainable non-Ancient card pool to 45 cards with mark setup, multi-hit payoffs, delayed Gunspark supply, hand filtering, Rapid Fire access, Rapid Fire payoffs, and capped Gunspark finisher damage.
- Added a 2026-06-05 defensive bridge mini-pass, expanding the obtainable non-Ancient card pool to 48 cards with `SuppressionSignal`, `SparkAegis`, and `HaloRelay`.
- Added a 2026-06-05 marked-defense mini-pass, expanding the obtainable non-Ancient card pool to 50 cards with `MarkedAdvance` and `SpottersCover`.
- Added a 2026-06-05 rare utility mini-pass, expanding the obtainable non-Ancient card pool to 52 cards with `VectorReboot` and `SparkBarrier`.
- Added a 2026-06-05 Rapid Fire follow-up mini-pass, expanding the obtainable non-Ancient card pool to 55 cards with `FlashpointMark`, `RelayFootwork`, and `SparkCrossfire`.
- Added `SanctifiedCrossfire` as Exusiai's Ancient replacement for Crossfire through Archaic Tooth.
- Added independent card art for the second-wave and Ancient card set that previously reused duplicate placeholder images.

### Changed

- Small balance pass: strengthened `CrossfirePattern`, `PointBlankShot`, `FieldStrip`, and `RapidStance`, and gave `SparkBarrierPower` its own icon path.
- Replaced the remaining abstract placeholder art for `CrossfirePattern`, `FlashpointMark`, `MarkedAdvance`, `TracerRounds`, `FieldStrip`, `HaloFeint`, `RelayFootwork`, and `SparkPrimer`.
- Entered a convergence balance pass at 55 non-Ancient obtainable cards instead of continuing to expand the pool.
- Clarified Rapid Fire keyword text: generated copies keep their cost, gain Ethereal and Exhaust, and cannot trigger Rapid Fire again.
- Reworked `RelayFootwork` into a pure defensive Rapid Fire follow-up: it now gains 5/8 Block and gains 4 more Block if a Rapid Fire card was played this turn, instead of drawing a card.
- Tuned `SparkCrossfire` down from 5/7 to 4/6 damage to keep post-Gunspark Rapid Fire turns from over-scaling through both the original card and its copy.
- Strengthened `SanctifiedCrossfire` from 10/14 damage plus 1 Gunspark to 16/24 damage, 2/3 Vulnerable, 1 Weak, and 1 Gunspark so the Archaic Tooth transformation has boss-reward impact comparable to `Bash` becoming `Break`.
- Retuned `GunslingerRush` from 7/10 damage plus 2/3 Block to 5/8 damage plus 5/7 Block so it no longer strictly outclasses starter `Crossfire`.
- Reworked `DeliveryGuaranteed` from precise discard-pile copying into random draw-pile reveal: reveal 5/6 cards, choose up to 2/3 to copy into hand at 0 cost this turn with Ethereal and Exhaust.
- Strengthened Rapid Fire as its own card-pool axis with `QuickdrawDrill`, `RhythmTrigger`, and `OpenFireDiscipline` instead of routing every payoff through Gunspark generation.
- Added defensive route tolerance without raising burst ceilings: defensive mark setup, Rapid Fire defensive payoff, and Gunspark defensive payoff.
- Added marked-defense payoffs so Vulnerable setup can convert into block and draw, not only damage.
- Added Rapid Fire follow-up cards that turn Rapid Fire into mark setup, defensive continuity, and conditional post-Gunspark damage without adding more Gunspark growth.
- Raised the rare pool to 10 cards with non-finisher utility and defensive engine options.
- Tuned `RhythmTrigger` so the base card draws 1 after Rapid Fire and the upgraded card draws 2.
- Updated local build/deploy documentation and Windows defaults for the current `D:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\Exusiai` install path.

### Fixed

- `QuickdrawDrill` now returns quietly when the draw pile contains no Rapid Fire cards instead of opening an empty selection grid.
- `SparkRecycle` removes selected hand cards from combat before drawing replacements.
- `Overclock` no longer relies on lingering card-instance state for temporary attack cost reduction.

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
