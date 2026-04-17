# MyFirstMod

My first Slay the Spire 2 mod with a test card.

## 开发环境

- Godot 4.5.1 Mono (Mobile 渲染器)
- .NET 9.0
- C#

## 编译

```bash
dotnet build
```

编译成功后，dll 和 json 会自动复制到游戏的 mods 目录：
```
C:/Program Files (x86)/Steam/steamapps/common/Slay the Spire 2/mods/MyFirstMod/
```

## 导出 PCK（可选）

如果需要打包资源文件（图片、音频等），使用 Godot 导出：

```bash
dotnet publish
```

**注意**: 需要在 `.csproj` 中配置 `GodotPath` 才能自动导出 PCK。

或者手动使用 Godot 编辑器导出：
1. 用 Godot 4.5.1 打开项目
2. 项目 → 导出 → 添加导出预设 → Windows Desktop
3. 导出为 PCK 文件到 `mods/MyFirstMod/MyFirstMod.pck`

## 项目结构

```
MyFirstMod/
├── Code/
│   ├── Entry.cs                    # MOD 入口
│   ├── MyFirstModCardModel.cs      # 卡牌基类
│   └── Cards/
│       └── TestCard.cs             # 测试卡牌
├── myfirstmod/                     # 资源目录（modid）
│   ├── images/
│   │   └── cards/
│   │       └── TestCard.png        # 卡牌立绘
│   └── localization/
│       └── zhs/
│           └── cards.json          # 中文本地化
├── project.godot                   # Godot 项目配置
├── MyFirstMod.csproj               # C# 项目配置
└── myfirstmod.json                 # MOD 元数据
```

## 测试卡牌

已添加测试卡牌 `TestCard`：
- **卡牌ID**: `MYFIRSTMOD-TEST_CARD`
- **名称**: 测试卡牌
- **类型**: 攻击牌
- **稀有度**: 普通
- **耗能**: 1
- **效果**: 造成 12 点伤害（升级后 16 点）
- **卡池**: 无色卡池

### 测试方法

**方法1：战斗中获取卡牌**
1. 开始一场战斗
2. 按 `~` 打开控制台
3. 输入: `card MYFIRSTMOD-TEST_CARD`

**方法2：在商店/奖励中查看**
- 由于加入了无色卡池，可以在商店或战斗奖励中随机遇到

**方法3：使用 event 命令**
1. 按 `~` 打开控制台
2. 输入: `event Falling` (进入掉落事件)
3. 选择获得无色卡牌
4. 可能会出现你的测试卡牌

### 常用控制台命令

```bash
# 战斗相关
fight                    # 开始随机战斗
kill                     # 杀死所有敌人

# 卡牌相关（需要在战斗中）
card CARD_ID            # 获得指定卡牌
hand 10                 # 抽10张牌

# 资源相关
gold 999                # 获得金币
potion POTION_ID        # 获得药水

# 事件相关
event Falling           # 触发掉落事件（可获得无色卡）
event Shop              # 触发商店事件

# 其他
deck                    # 查看当前牌组
```

## 参考资料

- [官方教程](https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/)
- [模板项目](https://github.com/Alchyr/ModTemplate-StS2)

