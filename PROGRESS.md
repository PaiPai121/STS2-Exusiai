# MyFirstMod 当前进度

更新时间：2026-05-31 01:42 Asia/Shanghai

## 开工前检查

- 先读 `RUNBOOK.md`
- 先读 `CLAUDE_NOTES.md`
- 先读 `DESIGN_SOURCE.md` 和 `CARD_LIBRARY.md`
- 确认真实工程路径：`D:\work_console\MyFirstMod`
- 确认当前 shell：PowerShell
- 复用已验证 build 命令，不猜测 csproj 路径
- 复用已验证 export 命令，不切换 MegaDot/Godot 版本
- 复用已验证 deploy 路径

## 当前已完成

- 速射核心已修复：速射牌会生成自身复制，复制获得虚无与消耗，并移除速射，避免再次触发。
- 已移除旧的 `TryManualPlay` / `OnPlayWrapper` Harmony bypass，不再绕过游戏原生出牌流程。
- `Gunspark` 生成链已恢复：通过 `Owner.Creature.CombatState.CreateCard<Gunspark>(Owner)` 创建战斗内 token，再加入手牌。
- 用户实测确认：追猎指令生成 `Gunspark` 后不再卡死，商店可正常进入。
- `PiercingRound` 已清理旧的 `StrikeCopy` 生成逻辑，仅保留稳定直伤。
- `StrikeCopy` / `StrikeCopyPlus` 类已从实际卡牌代码中清理。
- 新增 9 张扩展卡并接入牌池：
  - `PointBlankShot` / 贴身点射
  - `CoverReload` / 掩护换弹
  - `InterleavedFire` / 交错射击
  - `SparkCircuit` / 火花回路
  - `BreakthroughVector` / 突破向量
  - `HaloCover` / 光环掩护
  - `FireControl` / 火控校准
  - `AngelicReload` / 天使装填
  - `TerminalVolley` / 终端齐射
- 新增卡牌图片资源已放入 `myfirstmod/images/cards/`，并生成对应 `.import`。
- 卡牌图片资源已从 `1024x1024` 统一裁切为 `1024x768` 横向比例：裁掉上方 160px、下方 96px，使卡面在游戏较宽的美术窗口中减少两侧空窗并略微上移；原图备份在项目外 `D:\work_console\MyFirstMod_asset_backups\asset_backups\cards_20260531_013149`。
- 新增 3 个专属遗物并接入遗物池：
  - `SparkCapacitor` / 火花电容：每场战斗第一个玩家回合开始时，将 1 张 `枪火火花` 加入手牌。
  - `TacticalMagazine` / 战术弹匣：每场战斗中首次打出 `枪火火花` 时，抽 1 张牌。
  - `ReticleCalibrator` / 准星校准器：每打出 3 张攻击牌，获得 3 点格挡。
- 新增遗物图标资源已放入 `myfirstmod/images/relics/`，并生成对应 `.import`。
- 能力系统已扩展：
  - `AngelsBlessingPower`
  - `SweepModePower`
  - `SparkCircuitPower`
  - `FireControlPower`
- 4 个正式 Power 已补齐独立状态图标，资源位于 `myfirstmod/images/powers/`，不再共用角色头像：
  - `AngelsBlessingPower.png`
  - `SweepModePower.png`
  - `SparkCircuitPower.png`
  - `FireControlPower.png`
- 遗物与 Power 图标已统一改为透明底、内容铺满画布的图标规格，减少游戏 UI 中两侧空窗。
- 休息/篝火角色显示已从铁甲战士场景切换为 `myfirstmod/scenes/character/exusiai_rest_site.tscn`。
- `华法琳特调` 已改为 1 费技能：失去 2 点生命，抽 1/2 张牌，并将 1 张 `枪火火花` 加入手牌。
- `快速换弹` 已从 0 费抽 2/3 降为 0 费抽 1/2，并保留生成 1 张 `枪火火花`。
- `扫射模式` 已改为 1 费能力：每当你打出攻击牌，对所有敌人造成 2/3 点伤害。
- `天使装填` 已从抽牌爆发改为防御装填：1 费消耗，获得 6/9 点格挡并生成 2 张 `枪火火花`。
- `火控校准` 已收敛为每回合固定生成 1 张 `枪火火花`，升级改为提高入场格挡。
- `火花回路` 已收敛为每打出 3 张 `枪火火花` 抽 1 张牌，升级改为提高入场格挡。
- `追猎指令` 已去掉抽牌，保留造成伤害并生成 `枪火火花`。
- `终端齐射` 已降为生成 2 张 `枪火火花`，避免单卡直接推满火花回路触发。
- 普通牌过牌强度已收敛：`战术侧闪` 去掉抽牌，`应急护盾` 升级不再增加抽牌，`速射架势` 降为抽 1/2。
- `天使祝福` Power 已补出牌者检查，只统计拥有者本人的出牌。
- `过载模式` 当前设计保留：2 费技能，虚无；抽 2/3 张牌；本回合接下来打出的 2/3 张攻击牌打出前费用变为 0。
- `CARD_LIBRARY.md` 已更新为当前卡池真相。
- `DESIGN_SOURCE.md` 已记录飞书文档状态与本地优先原则。
- 飞书文档已用当前 `CARD_LIBRARY.md` 覆盖同步，回读 revision 为 `46`。

## 当前卡池状态

- 起始卡组：5 张打击、4 张防御、1 张交叉火力。
- 当前实际带速射的卡：
  - `CardTemplate`
  - `GunslingerRush`
  - `BarrageFire`
  - `FullAuto`
  - `InterleavedFire`
  - `BreakthroughVector`
- 当前正式 Power：
  - `AngelsBlessingPower`
  - `SweepModePower`
  - `SparkCircuitPower`
  - `FireControlPower`
- 当前 `Gunspark` 生成来源：
  - 战术侧闪
  - 快速换弹
  - 追猎指令
  - 枪林弹雨
  - 掩护换弹
  - 突破向量
  - 光环掩护
  - 火控校准
  - 天使装填
  - 终端齐射
  - 华法琳特调
- 当前专属遗物：
  - `SniperChipset` / 狙击芯片（起始）
  - `SparkCapacitor` / 火花电容（普通）
  - `TacticalMagazine` / 战术弹匣（罕见）
  - `ReticleCalibrator` / 准星校准器（罕见）

## 最近验证

- 已执行：

```powershell
dotnet build D:\work_console\MyFirstMod\MyFirstMod.csproj
```

- 结果：成功，0 warning，0 error。
- build 后 DLL 和 manifest 已由项目构建流程复制到游戏 mod 目录。
- 飞书文档同步命令已成功执行，并回读确认 revision `46`。
- 已重新导出 `MyFirstMod.pck`，新增遗物图标和本地化资源已进入 PCK；导出仍会打印既有 `sts2` 查找、UID duplicate、`.godot/exported` 缺失资源警告。
- 已重新导出 `MyFirstMod.pck`，正式 Power 的独立状态图标已进入 PCK。
- 已重新导出 `MyFirstMod.pck`，遗物与 Power 图标缩放修正已进入 PCK。
- 已重新导出 `MyFirstMod.pck`，能天使休息场景已进入 PCK。
- 已重新导出 `MyFirstMod.pck`，40 张卡图的横向裁切版本已进入 PCK。

## 当前暂存状态

- 扩展卡、Power、文档、PCK、新卡图片资源已提交。
- `cards.json` 本地化同步已提交。
- 旧卡图 `.import` 噪音已清理出工作区。
- 新增遗物扩展已提交，当前 Power 图标补齐仍待提交。

## 待验证

1. 游戏内测试 `华法琳特调`：
   - 是否正确失去 2 点生命。
   - 是否抽 1/2 张牌。
   - 是否生成 1 张 `枪火火花`。
   - 是否不会因自损或 token 生成卡死。
2. 游戏内测试 `扫射模式`：
   - 打出能力后是否出现 buff 图标。
   - 后续每次打出攻击牌是否对所有敌人造成 2/3 点伤害。
   - 多层叠加是否按数值叠加。
3. 游戏内测试新增 9 张扩展卡：
   - 奖励、商店、战斗中是否可正常出现和打出。
   - 新卡卡面是否显示正常。
   - `SparkCircuit` 是否按每 3 张 `Gunspark` 抽 1 张牌触发。
   - `FireControl` 是否每回合固定生成 1 张 `Gunspark`。
4. 再次观察 `Overclock`、`DeliveryGuaranteed`、`AngelsBlessing` 的实战强度。
5. 游戏内测试新增 3 个遗物：
   - `火花电容` 是否在战斗第一个玩家回合开始时生成 1 张 `枪火火花`。
   - `战术弹匣` 是否每场战斗只在首次打出 `枪火火花` 时抽 1 张牌。
   - `准星校准器` 是否每 3 张攻击牌获得 3 点格挡。
6. 游戏内检查 4 个正式 Power 的状态图标：
   - `天使祝福`、`扫射模式`、`火花回路`、`火控校准` 是否显示各自独立图标。
   - 小图标和详情大图标是否都不再显示能天使头像。
7. 游戏内检查休息/篝火界面：
   - 休息场景是否显示能天使而不是铁甲战士。
   - 能天使位置、缩放是否适合篝火界面。
8. 游戏内检查卡牌边框适配：
   - 卡面美术是否不再显得细长。
   - 两侧空窗是否明显减少。
   - 顶部/底部是否有关键人物、武器或特效被裁掉。

## 下一步建议

1. 先完成本轮实机测试，确认 `华法琳特调` 和 `扫射模式` 没有运行时问题。
2. 测试通过后提交当前 staged 改动。
3. 再进入下一轮平衡：优先看新增 Power 与 `Gunspark` 引擎是否过强。
4. 继续补齐或校准新增卡卡面资源。
5. 后续如要继续做战斗角色动态表现，目标应是接入真实 webm 素材或切帧资源，而不是伪动态。

## 已验证命令

### Build

```powershell
dotnet build D:\work_console\MyFirstMod\MyFirstMod.csproj
```

### Export PCK

```powershell
"D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe" --headless --path "D:\work_console\MyFirstMod" --export-pack BasicExport "D:\work_console\MyFirstMod\MyFirstMod.pck"
```

### Deploy 目录

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\MyFirstMod
```

### 日志目录

```text
C:\Users\HunterAndDragon\AppData\Roaming\SlayTheSpire2\logs
```

## 维护规则

- 改卡牌机制、数值、费用、稀有度、关键字后，同步更新 `CARD_LIBRARY.md`。
- 改当前设计判断或云文档同步状态后，同步更新 `DESIGN_SOURCE.md`。
- 做阶段性总结时，同步更新本文件。
- 改资源、本地化或图片后，需要重新 export PCK。
- 只改 C# 时通常 build 即可，但新增资源或 Godot import 变化需要重新 export。
