# Next Version Fix Plan

Updated: 2026-06-13

Scope: follow-up content and UX pass after the Exusiai ancient-event crash fix and Nexus release tooling. The previous P0 bugfix list has been implemented or manually verified; do not treat those old bug reports as active unless a new repro appears.

## Current Fix / Verification Checklist

- [x] Darv / Dusty Tome no longer routes Exusiai to `SanctifiedCrossfire`; it now grants dedicated Ancient card `PenguinLogisticsParcel`.
- [x] `SanctifiedCrossfire` remains exclusive to Archaic Tooth's `Crossfire` transformation path.
- [x] `PenguinLogisticsParcel` generates mutable combat card instances with `CombatState.CreateCard`, fixing the crash when rolled cards such as `Hyperbeam` are selected.
- [x] `PenguinLogisticsParcel` has final Exusiai / Penguin Logistics themed card art and matching localization.
- [x] Conditional card activation indicators use native yellow card-edge glow through `ShouldGlowGoldInternal`, not a custom overlay.
- [x] In-game verification: Rapid Fire condition glow works.
- [x] In-game verification: Gunspark-played condition glow works.
- [x] In-game verification: Gunspark-in-hand condition glow works.
- [x] In-game verification: Vulnerable-enemy condition glow works after `LockOnOrder`.
- [x] Static validation: `tools/Validate-Exusiai.ps1` passes.
- [x] Build validation: `dotnet build MyFirstMod.csproj /p:ModsPath=dist/build-check/` passes.
- [x] Local deploy validation: `build.bat` successfully copied `Exusiai.dll`, `Exusiai.pck`, and `Exusiai.json` to the local Steam mod directory.

## Release Strategy

Ship the next version in two passes:

1. Content semantics pass: replace the temporary Dusty Tome fallback with a dedicated Exusiai Ancient card.
2. UX pass: add activation indicators / counters where the engine exposes a clean hook.

## P0 Bugfixes - Completed / Verified

### Sweep Mode Damage Modifiers

Report: Sweep Mode damage is affected by Weak, Vulnerable, and Strength, but the card text does not imply normal attack scaling. Similar vanilla effects are not affected.

Status: manually verified OK after the current implementation. Keep the fixed-effect text unless a new repro appears.

Additional fix shipped: `SweepModePower` now snapshots living enemies before applying damage, preventing `Collection was modified` exceptions during death/removal sequences.

### Spark Barrier + Nimble / Dexterity

Report:

- Upgraded block display can reduce from 4 to 3 when affected by Nimble 2.
- The card and applied power show different values.
- The applied power says 2 Block, while actual block gained can be 3, likely due to Oddly Smooth Stone / Dexterity.

Status: fixed in the current implementation. `SparkBarrier` and `SparkBarrierPower` use unpowered block values so card preview, power amount, and triggered block stay aligned.

### Nimble Visual Bugs on Hybrid Cards

Reports:

- Suppressive Fire: upgraded card shows green block value under Nimble, but block value does not change.
- Gunslinger Rush: same issue, upgraded card shows green block value but value does not change.

Status: manually verified OK.

### Breakthrough Vector Upgrade Text / Logic

Report: upgraded Breakthrough Vector grants 2 Gunsparks while text says 1.

Status: fixed. `BreakthroughVector` uses `CardsVar` and shows/generated 1/2 Gunsparks consistently.

### Vector Reboot Infinite Risk

Report: two copies of Vector Reboot may create an easy infinite, similar to Guaranteed Delivery.

Status: fixed by current design changes. `VectorReboot` now exhausts, selects non-Attacks from discard, returns them to hand at 0 cost, and generates Gunsparks based on selected count.

## P0 Content Semantics

### Dusty Tome / Darv Dedicated Ancient Card

Status: implemented, built, deployed, and verified in game.

Previous behavior: `DustyTome` was patched to give `SanctifiedCrossfire` for Exusiai. That was a crash-prevention fallback, not the final design.

Current behavior: `DustyTome` gives `PenguinLogisticsParcel`, a separate Exusiai Ancient skill. `SanctifiedCrossfire` remains the Archaic Tooth transformation for starter `Crossfire`.

Design correction:

- Keep `SanctifiedCrossfire` exclusively as the Archaic Tooth transformation for starter `Crossfire`.
- Add a separate Exusiai Ancient card for `DustyTome`: `PenguinLogisticsParcel`.
- Update `ExusiaiDustyTomePatch` to point to that dedicated card.
- Add localization entries.
- Update `CARD_LIBRARY.md`, `DESIGN_SOURCE.md`, and `CHANGELOG.md`.
- Add final art for `PenguinLogisticsParcel`.

Final design direction:

- A Penguin Logistics themed Ancient skill rather than another Crossfire upgrade.
- The card should feel like opening a combat-only delivery box, similar in spirit to a potion creating temporary cards.
- Base behavior: choose all 3 of 3 random Rare cards from available character pools and add them to hand; they cost 0 this turn.
- Upgrade behavior: choose 3 of 5 random Rare cards.
- Generated cards do not gain Ethereal or Exhaust; if not played this turn, they remain in the combat at normal cost.

## P1 Clarifications

### Overclock Stacking and Cost

Reports:

- Base cost is currently 2; player asks whether it should become 3.
- Multiple Overclocks overlap instead of stacking free attack counts.

Plan:

- Decide final cost first.
- Decide stacking model:
  - non-stacking cycle with explicit text
  - stacking free-attack count
  - independent timers per Overclock
- Preferred short-term fix: keep current behavior if balanced, but make the description or tooltip explicit that multiple Overclocks do not stack.

Expected result: players can predict whether a second Overclock adds value.

### Ignition Protocol Modifier Interactions

Report: buff value may not work correctly with Strength or other modifiers.

Plan:

- Test Gunspark damage with Ignition Protocol alone.
- Test with Strength, Weak, Vulnerable, Sweep Mode, and other damage modifiers.
- Decide whether Ignition Protocol modifies base Gunspark damage or final damage.

Expected result: tooltip value and actual Gunspark damage scale consistently.

## P2 Text and UX Polish

### Shorten Long English Card Text

Report: some English cards have wall-of-text descriptions.

Plan:

- Audit English card descriptions over roughly 130-150 characters.
- Shorten text by removing repeated keyword explanations and using consistent terms.
- Avoid changing mechanics in the same patch unless necessary.

Candidates to inspect first:

- Delivery Guaranteed
- Vector Reboot
- Final Salvo
- Overclock
- Full Auto

### Final Salvo Counter

Report: player wants visibility into current hit count after playing Gunsparks.

Plan:

- Add a dynamic counter or description update showing current extra repeats.
- Ensure counter updates after each Gunspark played.

### Reticle Calibrator Counter

Report: player suggests a visible counter for when the relic will activate.

Plan:

- Check whether the relic already tracks attack count internally.
- Add visible relic counter if supported by relic UI conventions.

### Conditional Card Activation Indicators

Report: cards like Relay Footwork and Rapid Stance would be clearer if the card outline changed when active.

Status: implemented, built, deployed, and verified in game.

Implemented:

- Uses the game's native `CardModel.ShouldGlowGoldInternal` hook so conditional cards use the same yellow card-edge glow as vanilla cards.
- The glow is visible only during the play phase when the card is currently playable and the card's conditional bonus is live.
- First wave: `RapidStance`, `HaloRelay`, `SparkAegis`, `RelayFootwork`, `RhythmTrigger`, `SparkCrossfire`, `FinalSalvo`, `MarkedAdvance`, `SpottersCover`, `HaloFeint`.
- Target-dependent Vulnerable cards highlight when any living enemy is Vulnerable.

Verified in game:

- Rapid Fire condition: playing a Rapid Fire card makes `RapidStance` and related Rapid Fire payoff cards glow gold when playable.
- Gunspark played condition: playing `Gunspark` makes `SparkAegis`, `SparkCrossfire`, and `FinalSalvo` glow gold when playable.
- Gunspark in hand condition: `HaloFeint` glows gold while `Gunspark` is in hand.
- Vulnerable condition: after `LockOnOrder` applies Vulnerable, `MarkedAdvance` and `SpottersCover` glow gold when playable.

## Balance / Archetype Review

### Full Auto and Discard Gunspark Package

Report: Full Auto feels too niche because building enough Gunsparks in deck/discard clogs draws.

Ideas to evaluate:

- Full Auto gains Retain on upgrade.
- Full Auto counts Gunsparks in exhaust pile.
- Full Auto fires twice per Gunspark, with damage adjusted if needed.
- Add more ways to convert deck/discard Gunsparks into immediate payoff.

Recommendation: try Retain on upgrade first. It is the smallest buff and improves setup reliability without rewriting the archetype.

### Open Fire Discipline vs Sweep Mode

Report: Sweep Mode appears to outperform Open Fire Discipline.

Plan:

- Re-evaluate only after Sweep Mode modifier bug is fixed.
- If Sweep Mode loses unintended Strength/Weak/Vulnerable scaling, the gap may shrink.
- If still weak, buff OFD rather than swapping effects immediately.

## Already Addressed in Current Release

### Orobas / Sniper Chipset

Report: Orobas option replaced Sniper's Chipset with Circlet.

Status: fixed in the current bugfix release by disabling Touch of Orobas for Exusiai and handling Crossfire through the standard Archaic Tooth / Transcendence card path.

Follow-up:

- Mention this explicitly in changelog / release notes.
- Retest after release to ensure old saves do not keep a stale Touch of Orobas state.

## Suggested Test Matrix

- New Exusiai run: starting deck and starter relic visible.
- Ordinary card rewards: Crossfire and New Covenant Crossfire do not appear.
- Archaic Tooth: Crossfire transforms into New Covenant Crossfire.
- Touch of Orobas: no Sniper's Chipset replacement option for Exusiai.
- Darv / Dusty Tome: Exusiai receives `PenguinLogisticsParcel`, not `SanctifiedCrossfire`, and opening the event does not crash.
- Archaic Tooth after Darv test: `Crossfire` still transforms into `SanctifiedCrossfire`, confirming the two Ancient sources are separate.
- Sweep Mode: compare damage with no modifiers, Strength, Weak, Vulnerable.
- Spark Barrier: compare card text, power text, and actual block with and without Nimble / Dexterity.
- Suppressive Fire and Gunslinger Rush: inspect upgraded values with Nimble.
- Breakthrough Vector: verify generated Gunspark count before and after upgrade.
- Vector Reboot: test two-copy loop potential.
- Overclock: test two active Overclocks across odd/even turns.
