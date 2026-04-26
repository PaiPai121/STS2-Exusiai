### 当前项目增量记录（2026-04-26）

- **已验证 build 入口**：`D:\work_console\MyFirstMod\MyFirstMod.csproj`
- **已验证导出器**：`D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe`
- **已验证日志目录**：`C:\Users\HunterAndDragon\AppData\Roaming\SlayTheSpire2\logs`
- **已验证部署目录**：`C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\MyFirstMod`
- **本次新增规则文件**：`RUNBOOK.md`、`CLAUDE_NOTES.md`
- **商店黑屏根因（本轮）**：卡池稀有度覆盖不足，导致 merchant card generation 无法生成合法选项。
- **本轮修复策略**：新增 Uncommon/Rare 正式最简卡 + 多张 placeholder 补池卡，先恢复商店与奖励流转，再逐步替换为正式设计卡。
- **本轮结果**：用户实测商店恢复可进入。
- **执行纪律更新**：后续 build / export / deploy 必须先复用已验证命令，不得猜测工程入口或导出链路。
- **战斗动态素材需求澄清**：目标不是伪动态呼吸/浮动，而是把现有 webm 动态资产本体接入战斗角色显示链路。
- **关键动态资产路径**：
  - `D:\work_console\workspaceforexusuai\assests\能天使-午夜邮差-正面-Idle-x1.webm`
  - `D:\work_console\workspaceforexusuai\assests\能天使-午夜邮差-正面-Attack-x1.webm`
  - `D:\work_console\workspaceforexusuai\assests\能天使-午夜邮差-正面-Die-x1.webm`
- **已排除方案**：直接引用 `res://InesSilent/scenes/character/ines_default.tscn` 作为战斗场景来源不可行；build 可过，但 export 无法解析外部 mod 资源链。
- **当前推荐实施顺序**：先做 `Idle-x1.webm -> 序列帧 / sprite sheet -> exusiai_default.tscn`，待待机成功后再接 Attack / Die。

# 杀戮尖塔2 MOD 开发经验记录

## 项目搭建要点

### 1. 关键配置

**BaseLib 版本必须匹配**：
- 检查游戏 mods 目录中的 BaseLib.dll 版本
- 在 .csproj 中锁定对应版本（如 `Version="3.0.1"`）
- 版本不匹配会导致 `ReflectionTypeLoadException`

**modid.json 必须包含 dll_name**：
```json
{
  "id": "myfirstmod",
  "dll_name": "MyFirstMod.dll",  // 必须指定，否则游戏会查找 {id}.dll
  "has_pck": true,
  "has_dll": true
}
```

**Publicizer 配置**（.csproj）：
```xml
<!-- 必须同时 publicize sts2 和 BaseLib，否则无法访问内部类型 -->
<Publicize Include="sts2" />
<Publicize Include="BaseLib" />
```
- `sts2`：访问 CardModel、Creature、CombatRoom 及 protected virtual 方法
- `BaseLib`：访问 CustomEnumAttribute、KeywordPropertiesAttribute 等
- ⚠️ Publicize 会把基类的 `protected` 改成 `public`，子类的 `protected override` 必须同步改为 `public override`，否则 CS0507 编译错误

### 2. 项目结构

```
MyFirstMod/
├── Code/
│   ├── Entry.cs                       # MOD 入口
│   ├── MyFirstModCardModel.cs         # 卡牌基类（统一管理卡图路径）
│   ├── MyFirstModRelicModel.cs        # 遗物基类（统一管理图标路径）
│   ├── Cards/
│   │   ├── RapidFireCardModel.cs      # 速射卡牌抽象基类
│   │   └── TestCard.cs               # 具体卡牌
│   ├── Keywords/
│   │   └── MyKeywords.cs             # 自定义关键字定义
│   ├── Relics/
│   │   └── SniperChipset.cs          # 遗物实现
│   ├── CardPools/                     # 卡池
│   ├── RelicPools/                    # 遗物池
│   └── PotionPools/                   # 药水池
├── myfirstmod/                        # 资源目录（必须是 modid）
│   ├── images/cards/                  # 卡图
│   ├── images/relics/                 # 遗物图标
│   └── localization/zhs/             # 中文本地化
│       ├── cards.json
│       ├── card_keywords.json         # 关键字本地化
│       ├── relics.json
│       ├── characters.json
│       └── ancients.json
├── MyFirstMod.csproj
├── myfirstmod.json
└── export_presets.cfg
```

---

## 核心 Namespace 地图

这是踩了无数坑整理出来的，**务必参考此表写 using**：

| 类型 | 命名空间 | 备注 |
|------|---------|------|
| `CardModel` | `MegaCrit.Sts2.Core.Models` | ⚠️ 不在 Entities.Cards！ |
| `CardType`, `CardRarity`, `TargetType` | `MegaCrit.Sts2.Core.Entities.Cards` | |
| `CardKeyword`, `CardPlay`, `PileType` | `MegaCrit.Sts2.Core.Entities.Cards` | |
| `Creature` | `MegaCrit.Sts2.Core.Entities.Creatures` | 需要 Publicize |
| `CombatRoom` | `MegaCrit.Sts2.Core.Rooms` | ⚠️ 不在 Entities.Rooms！ |
| `Player` | `MegaCrit.Sts2.Core.Entities.Players` | |
| `PlayerChoiceContext` | `MegaCrit.Sts2.Core.GameActions.Multiplayer` | |
| `CardPileCmd`, `DamageCmd` | `MegaCrit.Sts2.Core.Commands` | |
| `RelicModel`, `RelicRarity`, `RelicStatus` | `MegaCrit.Sts2.Core.Entities.Relics` | |
| `CustomEnumAttribute` | `BaseLib.Patches.Content` | ⚠️ 不在 BaseLib.Utils！ |
| `KeywordPropertiesAttribute` | `BaseLib.Patches.Content` | |
| `AutoKeywordPosition` | `BaseLib.Patches.Content` | |
| `[Pool]` 属性 | `BaseLib.Utils` | |
| `CustomCardModel`, `CustomRelicModel` | `BaseLib.Abstracts` | |
| `DynamicVar`, `DamageVar`, `ValueProp` | `MegaCrit.Sts2.Core.Localization.DynamicVars` / `MegaCrit.Sts2.Core.ValueProps` | |

---

## 卡牌开发

### 基本流程

1. 继承 `MyFirstModCardModel`（不是直接继承 `CustomCardModel`）
2. 添加 `[Pool(typeof(YourCardPool))]` 注册到卡池
3. 卡图命名：`{ClassName}.png`（如 TestCard.png），放 `myfirstmod/images/cards/`
4. 本地化 key：`{MODID}-{CARD_ID}.title` / `.description`
5. 卡牌 ID 格式：`{命名空间首段大写}-{类名SNAKE_CASE}`（如 `MYFIRSTMOD-TEST_CARD`）

### 自定义关键字（如"速射"）

**定义关键字枚举**（`Code/Keywords/MyKeywords.cs`）：
```csharp
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

public class MyKeywords
{
    [CustomEnum("RAPID_FIRE")]                          // 注册到 CardKeyword 枚举
    [KeywordProperties(AutoKeywordPosition.Before)]     // 关键字显示在卡牌描述前面
    public static CardKeyword RapidFire;
}
```

**本地化**（`myfirstmod/localization/zhs/card_keywords.json`）：
```json
{
    "MYFIRSTMOD-RAPID_FIRE.title": "速射",
    "MYFIRSTMOD-RAPID_FIRE.description": "打出时额外生成 1 张此牌的复制加入手牌（0 费）。复制不会再次触发速射。"
}
```

**关键字基类模式**（如 `RapidFireCardModel`）：
```csharp
public abstract class RapidFireCardModel : MyFirstModCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [MyKeywords.RapidFire];

    // 子类在 OnPlay 末尾调用此方法
    protected async Task TryGenerateRapidFireCopy(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!CanonicalKeywords.Contains(MyKeywords.RapidFire)) return;  // 复制无速射，不再触发
        if (!cardPlay.IsFirstInSeries) return;  // 防止 ModifyCardPlayCount 导致重复生成

        Player owner = Owner;
        CardModel copy = owner.RunState.CloneCard(this);
        copy.RemoveKeyword(MyKeywords.RapidFire);     // 移除速射（防套娃）
        copy.EnergyCost.SetUntilPlayed(0);            // 设为0费
        await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, addedByPlayer: true);
    }
}
```

**子类使用**：
```csharp
public class TestCard : RapidFireCardModel
{
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // ... 卡牌效果 ...
        await TryGenerateRapidFireCopy(choiceContext, cardPlay);  // 末尾调用
    }
}
```

---

## 遗物开发

### 关键 Hook 方法

| 方法 | 时机 | 用途 |
|------|------|------|
| `BeforeCombatStart()` | 战斗开始 | 初始化状态，设 `Status = RelicStatus.Active` |
| `ModifyCardPlayCount(card, target, playCount)` | 计算打出次数 | 返回 `playCount + N` 可让卡牌额外打出 N 次 |
| `AfterModifyingCardPlayCount(card)` | 打出次数修改后 | 一次性效果标记已使用，`Flash()` + `Status = RelicStatus.Normal` |
| `AfterCombatEnd(CombatRoom _)` | 战斗结束 | 重置状态 |

### 遗物示例：狙击芯片（首张攻击牌额外打出1次）

```csharp
[Pool(typeof(ExusiaiRelicPool))]
public class SniperChipset : MyFirstModRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    private bool _usedThisCombat;

    public override Task BeforeCombatStart()
    {
        _usedThisCombat = false;
        Status = RelicStatus.Active;      // 高亮显示
        return Task.CompletedTask;
    }

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (_usedThisCombat) return playCount;
        if (card.Owner != Owner) return playCount;
        if (card.Type != CardType.Attack) return playCount;
        return playCount + 1;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        _usedThisCombat = true;
        Flash();                           // 闪光动画
        Status = RelicStatus.Normal;       // 灰化（已使用）
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        _usedThisCombat = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}
```

---

## 编译和部署

### 一键流程

```bash
# 1. 编译 DLL（自动复制到 mods 目录）
dotnet build

# 2. 导出 PCK（打包资源文件：图片、JSON本地化等）
"D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64.exe" --headless --export-debug "BasicExport" "D:\work_console\MyFirstMod\MyFirstMod.pck"

# 3. 复制 PCK 到 mods 目录
copy MyFirstMod.pck "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\MyFirstMod\"
```

### ⚠️ PCK 导出模板问题

**症状**：导出时报错 `指定路径不存在导出模板：.../windows_debug_x86_64.exe`

**原因**：MegaDot 的导出模板被清空（目录 `AppData\Roaming\Godot\export_templates\4.5.1.m.9.mono\` 为空）

**修复**：将 MegaDot 编辑器自身复制为导出模板：
```powershell
# 创建模板目录
New-Item -ItemType Directory -Force -Path "C:\Users\{用户名}\AppData\Roaming\Godot\export_templates\4.5.1.m.9.mono"

# 复制编辑器为 debug 和 release 模板
Copy-Item "MegaDot编辑器路径\MegaDot_v4.5.1-stable_mono_win64.exe" `
  "C:\Users\{用户名}\AppData\Roaming\Godot\export_templates\4.5.1.m.9.mono\windows_debug_x86_64.exe"
Copy-Item "MegaDot编辑器路径\MegaDot_v4.5.1-stable_mono_win64.exe" `
  "C:\Users\{用户名}\AppData\Roaming\Godot\export_templates\4.5.1.m.9.mono\windows_release_x86_64.exe"
```

**注意**：headless 导出时 `sts2.dll not found` 的报错可以忽略——这是 Godot 加载项目程序集时的警告，不影响 PCK 打包。

---

## 常见踩坑

### ❌ 不要在 CardModel 上覆写 AfterCardPlayed

`AfterCardPlayed` 是 `RelicModel` 的虚方法，Publicize 让它在 CardModel 上可见，但覆写会导致运行时战斗画面混乱。正确做法是在 `OnPlay` 末尾手动调用生成逻辑。

### ❌ CardModel 命名空间不是 Entities.Cards

`CardModel` 在 `MegaCrit.Sts2.Core.Models`，不在 `MegaCrit.Sts2.Core.Entities.Cards`。后者是 `CardType`、`CardKeyword` 等的命名空间。

### ❌ CombatRoom 命名空间不是 Entities.Rooms

`CombatRoom` 在 `MegaCrit.Sts2.Core.Rooms`，不在 `MegaCrit.Sts2.Core.Entities.Rooms`。

### ❌ CustomEnumAttribute 不在 BaseLib.Utils

`CustomEnumAttribute`、`KeywordPropertiesAttribute`、`AutoKeywordPosition` 都在 `BaseLib.Patches.Content`。

### ✅ IsFirstInSeries 防重复触发

当遗物用 `ModifyCardPlayCount` 让卡牌额外打出时，`OnPlay` 会被调用多次。速射类效果必须用 `cardPlay.IsFirstInSeries` 检查，只在首次打出时生成复制，否则会重复生成。

### ✅ Publicize 后 protected→public

Publicize 把 sts2/BaseLib 的 `protected` 改为 `public`，导致子类 `protected override` 与基类 `public` 冲突（CS0507）。所有覆写方法改为 `public override`。

---

## 测试命令

```bash
# 进入战斗
fight

# 获取卡牌（必须在战斗中）
card MYFIRSTMOD-TEST_CARD

# 杀死所有敌人
kill

# 查看牌组
deck
```

## 查看日志

```
C:\Users\HunterAndDragon\AppData\Roaming\SlayTheSpire2\logs\godot.log
```

## 参考资料

- [官方教程](https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/)
- [模板项目](https://github.com/Alchyr/ModTemplate-StS2)

---

## 经验总结

1. **版本锁定是关键** — BaseLib 版本不匹配是最常见的错误
2. **先验证 DLL 加载** — 确保 MOD 能加载后再添加资源
3. **日志是第一手资料** — 出问题先看日志，不要猜
4. **资源路径要规范** — modid 目录名必须和 json 中的 id 一致
5. **测试要在战斗中** — card 命令只能在战斗中使用
6. **Namespace 要查实** — 不要凭直觉猜命名空间，反编译 sts2.dll 确认
7. **PCK 每次改资源要重新导出** — 改了本地化 JSON 或图片后必须重新导出 PCK 并部署
8. **导出模板会丢失** — 清理 AppData 或重装系统后需要重新安装 MegaDot 导出模板
