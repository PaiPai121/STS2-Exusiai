# MyFirstMod 当前进度

## 开工前必须检查
- [ ] 先读 `RUNBOOK.md`
- [ ] 先读 `CLAUDE_NOTES.md`
- [ ] 确认真实 csproj/sln 路径，不允许猜
- [ ] 确认当前 shell 环境，当前为 PowerShell
- [ ] 复用上一轮成功的 build 命令
- [ ] 复用上一轮成功的 export 命令
- [ ] 复用上一轮成功的 deploy 路径
- [ ] 只有旧链路失效时，才允许新增命令

## 已完成
- 已清理并重构 `PiercingRound`，移除了错误的 `StrikeCopy` 生成逻辑。
- 已清理多余废弃占位类，`StrikeCopy` / `StrikeCopyPlus` 不再作为实际卡牌存在。
- 已同步 `CARD_LIBRARY.md` 与 `cards.json`：`PiercingRound` 仅保留直伤效果，`StrikeCopy*` 本地化残留已移除。
- 已验证 `Gunspark` 的 `CreateClone()` 方案不可行：canonical token 调用 `CreateClone()` 会抛 `CanonicalModelException`。
- 已恢复 `Gunspark` 生成功能：`TacticalSidestep`、`QuickMagazine`、`PursuitOrder`、`BulletHell` 会重新生成枪火火花。
- `Gunspark` 当前使用正确战斗态创建链路：`Owner.Creature.CombatState.CreateCard<Gunspark>(Owner)`，再通过 `CardPileCmd.AddGeneratedCardToCombat(..., PileType.Hand, true)` 加入手牌。
- 已移除旧的 `TryManualPlay` / `OnPlayWrapper` Harmony bypass，不再绕过游戏原生出牌流程。
- 已修复速射复制牌核心逻辑：复制牌会获得虚无与消耗，且不会再次触发速射。
- 用户实测：速射相关战斗流程、遗物联动、商店进入已恢复。
- 已创建流程护栏文档：`RUNBOOK.md`、`CLAUDE_NOTES.md`。

## 当前状态
- 核心功能可用，但 `Gunspark` 新恢复链路仍需要游戏内实测。
- 当前实际带 **速射** 的卡：`CardTemplate`、`GunslingerRush`、`BarrageFire`、`FullAuto`。
- 当前起始卡组：5 张打击、4 张防御、1 张交叉火力。
- 当前牌池仍处于“可玩性优先”状态：部分正式卡 + 部分占位补池卡保证流程稳定。
- 当前源码、`CARD_LIBRARY.md`、`cards.json` 与导出的 `MyFirstMod.pck` 需要在每次改动后保持同步。
- 当前战斗角色仍保持“稳定静态图”基线；动态 webm 接入仍是后续需求。

## 最新提交
- `dc6df89` Fix shop pool generation and add workflow guardrails
- `4ae3b3e` Fix rapid fire clone behavior and combat hang

## 当前待办 / 下一步执行顺序
1. **最高优先级：游戏内验证 `Gunspark` 生成链路**
   - 打出 `PursuitOrder`，确认伤害、抽牌、枪火火花加入手牌都正常。
   - 打出生成出来的 `Gunspark`，确认不会再报 “must be added to a CombatState before playing it”。
   - 测试 `TacticalSidestep`、`QuickMagazine`、`BulletHell` 的生成数量与流程稳定性。
   - 若仍失败，查看 `C:\Users\HunterAndDragon\AppData\Roaming\SlayTheSpire2\logs` 下日志，按运行层问题继续定位。
2. 继续校准 `AngelsBlessing`、`Overclock`、`DeliveryGuaranteed` 的作用范围与回合逻辑。
3. 将已接入的新通用卡从“骨架版效果”升级为设计稿版本。
4. 为新增卡补齐真实 portrait 资源，减少 fallback 日志。
5. 将真实动态素材接入方案写入本地知识，并以 webm 资产驱动为目标继续推进。

## 未完成 / 待继续观察
- 卡牌描述中的能量图标仍需根据游戏内观感继续微调。
- `Gunspark` 的恢复链路已通过本地 build，但尚未完成游戏内实测确认。
- 占位补池卡仍需逐步替换为正式设计版卡牌。
- 战斗角色动态素材替换当前真实需求：不是伪动态，而是把 webm 动态资产本体直接播放或切帧后播放并接入战斗。
- 当前存在未提交的动态尝试文件：
  - `myfirstmod/scenes/character/exusiai_default.tscn`
  - `myfirstmod/scenes/character/exusiai_battle_visuals.gd`
  - `MyFirstMod.pck`
- 仓库当前未配置 git remote，尚未 push。

## 说明
- 构建命令：
  - `dotnet build D:\work_console\MyFirstMod\MyFirstMod.csproj`
- PCK 导出命令：
  - `"D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe" --headless --path "D:\work_console\MyFirstMod" --export-pack BasicExport "D:\work_console\MyFirstMod\MyFirstMod.pck"`
- 部署目录：
  - `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\MyFirstMod`
- 规则文件已固化当前项目执行纪律，后续 build / export / deploy 必须优先复用已验证链路。
