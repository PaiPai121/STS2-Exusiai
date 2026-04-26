# MyFirstMod 当前进度

## 开工前必须检查
- [ ] 先读取 `RUNBOOK.md`
- [ ] 先读取 `CLAUDE_NOTES.md`
- [ ] 确认真实 csproj/sln 路径，不允许猜
- [ ] 确认当前 shell 环境（bash / powershell）
- [ ] 复用上一次成功的 build 命令
- [ ] 复用上一次成功的 export 命令
- [ ] 复用上一次成功的 deploy 路径
- [ ] 只有旧链路失效时，才允许新增命令

## 已完成
- 修复速射复制牌的核心逻辑：复制牌现在会正确获得**虚无**与**消耗**。
- 修复速射复制的防套娃逻辑：复制牌不会再次触发速射。
- 修复部分战斗结束卡死问题：当前用户实测，速射相关战斗流程已不再卡死。
- 修复狙击芯片与速射的联动：当前用户实测，遗物触发后的额外出牌已能正确触发速射效果。
- 修正速射关键字描述：已移除“0费复制”的错误描述。
- 调整卡牌描述中的能量图标尺寸：已进一步缩小，目标是与普通文字视觉尺寸接近。
- 已完成构建并自动复制到游戏 mods 目录。
- 已补齐商店/奖励池所需的普通、罕见、稀有卡池覆盖。
- 用户实测：商店现已可正常进入。
- 已创建流程护栏文档：`RUNBOOK.md`、`CLAUDE_NOTES.md`。
- 已验证“直接引用 InesSilent 外部战斗场景”不可行：build 可过，但 export 无法解析 `res://InesSilent/...` 资源链。

## 当前状态
- 核心功能可用。
- 游戏内验证通过：
  - 速射复制生成正常
  - 虚无/消耗词条显示正常
  - 战斗未再出现此前的结束卡死
  - 狙击芯片联动生效
  - 商店已恢复可进入
- 当前牌池为“可玩性优先”状态：已使用部分正式卡 + 部分占位补池卡保证流程稳定。
- 当前战斗角色仍保持“稳定静态图”基线；上一轮伪动态脚本尝试未满足真实需求。

## 最新提交
- `dc6df89` Fix shop pool generation and add workflow guardrails
- `4ae3b3e` Fix rapid fire clone behavior and combat hang

## 当前代办 / 下一步执行顺序
1. 将真实动态素材接入方案写入本地知识，并以 **webm 资产驱动** 为唯一目标继续推进。
2. 围绕以下素材启动处理链路：
   - `D:\work_console\workspaceforexusuai\assests\能天使-午夜邮差-正面-Idle-x1.webm`
   - `D:\work_console\workspaceforexusuai\assests\能天使-午夜邮差-正面-Attack-x1.webm`
   - `D:\work_console\workspaceforexusuai\assests\能天使-午夜邮差-正面-Die-x1.webm`
3. 先读取媒体信息（尺寸 / 时长 / 帧率），确认切帧策略。
4. 优先执行：`Idle-x1.webm -> 序列帧 / sprite sheet -> 战斗待机动画`。
5. 仅在 `myfirstmod/scenes/character/exusiai_default.tscn` 及其直接依赖内实施，不得扩散到商店 / 继续游戏 / 小头像链路。
6. build / export / deploy / 进战斗验证真实动态素材是否显示。
7. 待 Idle 成功后，再考虑 Attack / Die 的接入与状态切换。

## 未完成 / 待继续观察
- 卡牌描述中的能量图标仍需根据游戏内观感继续微调。
- 部分 UI 资源与文案仍建议继续统一检查。
- 占位补池卡仍需逐步替换为正式设计版卡牌。
- **战斗角色动态素材替换当前真实需求**：不是伪动态，而是把 webm 动态资产本体（直接播放或切帧后播放）接入战斗。
- 当前存在未提交的动态尝试文件：
  - `myfirstmod/scenes/character/exusiai_default.tscn`
  - `myfirstmod/scenes/character/exusiai_battle_visuals.gd`
  - `MyFirstMod.pck`
- 仓库当前未配置 git remote，尚未 push。

## 说明
- 本次修复已经通过本地构建，并自动部署到：
  - `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\MyFirstMod`
- 规则文件已固化当前项目执行纪律，后续 build / export / deploy 必须优先复用已验证链路。
- 如需继续 push，需要先配置远程仓库地址。
