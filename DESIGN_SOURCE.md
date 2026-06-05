# Design Source

## Source Status

The legacy Feishu document below is a historical design source, not the current source of truth.
The current playable implementation has diverged intentionally in several places.

- Current Feishu card library: https://ycn3zoaa2bmg.feishu.cn/docx/CGTHdycpQoFFDpxS819cAc4Vnib
- Current Feishu document ID: CGTHdycpQoFFDpxS819cAc4Vnib
- Legacy Feishu title: 能天使 MOD - 卡牌设计
- Legacy URL: https://feishu.cn/docx/E1hHd1rnrovTJGxkXUYcwRJfnUb
- Legacy document ID: E1hHd1rnrovTJGxkXUYcwRJfnUb
- Last fetched: 2026-05-30 23:15 Asia/Shanghai
- Revision ID at last fetch: 35

## Current Canonical Decisions

These local decisions override the stale Feishu document:

- `速射·打击` is abandoned. The current starting rapid-fire card is `交叉火力` / `CardTemplate`.
- The current rapid-fire implementation is canonical: rapid-fire cards clone themselves into hand, the copy keeps its cost, loses rapid-fire, and gains Ethereal + Exhaust. Rapid-fire copies are not free unless another card effect, such as `QuickdrawDrill`, explicitly makes them free.
- `Gunspark` current implementation is canonical: `枪火火花`, 0 cost, 4 damage, Ethereal + Exhaust, and combat-scaling damage from `IgnitionProtocol`.
- `PursuitOrder` currently uses the name `追猎指令`; keep this name unless explicitly renamed later.
- `IgnitionProtocol` and the updated `SparkCircuit` are now the canonical Gunspark scaling route: `IgnitionProtocol` provides damage growth; `SparkCircuit` provides draw and, when upgraded, extra Gunspark supply.
- Keep long-term `Gunspark` damage growth on `IgnitionProtocol`. Do not move persistent damage scaling back onto `SparkCircuit` unless a later balance pass intentionally redefines `SparkCircuit` as the main damage engine; its current role is circulation, draw, and upgraded supply.
- Card balance should be evaluated by synergy packages, not only by standalone rate. Small focused decks should be able to produce strong turns through card cooperation, especially via low-cost attacks, rapid-fire copies, generated Gunsparks, `ChainReaction`, `AngelsBlessing`, `SweepMode`, `Overclock`, and `DeliveryGuaranteed`.
- When responding to "too few cards / too many repeated roles" feedback, do not reduce total card count. Rework duplicate roles into distinct mechanisms, especially delayed supply, discard-pile setup, non-Power midgame engines, and non-Exhaust rare cards.
- Rare cards should not all be Exhaust burst cards. Keep some one-shot finishers, but preserve repeatable rare attacks and skills so the rare pool is not only left-hand/right-hand resource routing.
- Starter relic power should define Exusiai's rapid-fire/Gunspark identity without copying arbitrary high-value Attacks. The current starter relic rewards the first Rapid Fire card each combat with one Gunspark instead of replaying the first Attack.
- Card pool size target is staged. Official STS2 character pools are about 87-88 cards; Exusiai should not jump there in one pass. The current short-term target is 45-50 obtainable non-Ancient cards, then 60-70 for a robust mod run, and only later 80-88 if pursuing full official-class breadth.
- The 2026-06-04 second expansion batch is canonical: add 13 cards focused on mark/Vulnerable setup, multi-hit payoffs, non-Power tempo payoff, delayed Gunspark supply, hand filtering, Rapid Fire access/payoffs, and capped Gunspark finisher damage.
- The 2026-06-05 defensive bridge mini-pass is canonical: add 3 Skill cards focused on defensive mark setup, Rapid Fire defensive payoff, and Gunspark defensive payoff. This pass improves survival and route tolerance without raising rare finisher or Gunspark burst ceilings.
- The 2026-06-05 marked-defense mini-pass is canonical: add 2 cards that convert existing Vulnerable setup into block or draw, bringing the non-Ancient obtainable pool to 50 cards without adding a new finisher or Power.
- The 2026-06-05 rare utility mini-pass is canonical: add 2 rare non-finisher cards, `VectorReboot` and `SparkBarrier`, bringing the non-Ancient obtainable pool to 52 cards and the rare pool to 10 cards.
- The 2026-06-05 Rapid Fire follow-up mini-pass is canonical: add `FlashpointMark`, `RelayFootwork`, and `SparkCrossfire`, bringing the non-Ancient obtainable pool to 55 cards while strengthening Rapid Fire as mark setup, defensive continuity, and conditional post-Gunspark damage instead of adding more token growth.
- Second-wave rare payoff cards should have explicit caps or startup costs. `FinalSalvo` caps extra hits from Gunsparks at 3; `OpenFireDiscipline` costs 2 and rewards Rapid Fire density instead of being a generic every-turn Gunspark engine.
- Rapid Fire must remain a real card-pool axis, not only a way to produce `Gunspark`. The current Rapid Fire support package is `QuickdrawDrill` for draw-pile access and temporary cost reduction, `RhythmTrigger` for post-Rapid-Fire draw payoff, and `OpenFireDiscipline` for persistent direct damage payoff.
- Do not convert every Rapid Fire payoff into Gunspark generation. Future Rapid Fire cards should consider direct copy value, temporary cost manipulation, card access, target damage, and attack-trigger synergies before adding more token output.
- The 2026-06-05 convergence balance pass is canonical: do not expand beyond 55 non-Ancient obtainable cards for now. `RelayFootwork` is a common defensive follow-up, not a second copy of `HaloRelay`; `SparkCrossfire` is capped by lower 4/6 damage because both the original and Rapid Fire copy can satisfy the Gunspark condition.
- Player-facing mod name is `Exusiai`. Card IDs currently use the original project namespace generated by the mod (`MYFIRSTMOD-*`). Do not rename IDs to `EXUSIAI-*` without a migration plan.

## Design Judgment For Known Differences

- Keep the current card types for now when they are already playable and tested, even if the old Feishu document labels some of them as powers.
- Prefer stable combat flow over strict parity with the stale document.
- Large mechanic rewrites should be done one card at a time, with build/export/deploy and in-game validation after each batch.
- The remote document can still inspire future balancing, but local code plus this file define the current target.

## Current Design Scope

- Current target: a playable Exusiai rapid-fire deck focused on frequent card play, temporary copies, and generated `Gunspark`.
- Current expansion batches: 2026-05-30 added a small 9-card package focused on low-cost play count, defensive token generation, rapid-fire token bridges, `Gunspark` draw engines, and rare finishers. 2026-06-04 added 13 second-wave mechanism cards, raising the non-Ancient obtainable pool from 32 to 45 cards. 2026-06-05 added 3 defensive bridge cards, 2 marked-defense payoff cards, 2 rare utility/defense cards, and 3 Rapid Fire follow-up cards, raising the non-Ancient obtainable pool to 55 cards.
- Current evaluation rule: before major card changes, mentally simulate several representative fights and check whether a small focused deck can assemble a strong output turn without relying on a single overtuned card.
- Deferred: the old ammo-flow / B-flow ideas remain out of scope unless explicitly reintroduced.
- Current card library status is maintained in `CARD_LIBRARY.md`.

## Implementation Reminder

If the current Feishu card library is updated later, fetch it with:

```powershell
lark-cli.cmd docs +fetch --api-version v2 --doc "https://ycn3zoaa2bmg.feishu.cn/docx/CGTHdycpQoFFDpxS819cAc4Vnib" --doc-format markdown
```

After fetching, compare it against this file and update the local canonical decisions explicitly. Do not silently treat the remote document as authoritative.
