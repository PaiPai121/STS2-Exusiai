# MyFirstMod - Exusiai

杀戮尖塔2 MOD：明日方舟能天使（Exusiai）作为可选角色。

## 开发环境

- Godot 4.5.1 Mono / MegaDot 4.5.1.m.9
- .NET 9.0 / C#
- BaseLib 3.0.1
- Krafs.Publicizer 2.3.0

## 编译和部署

```bash
# 1. 编译 DLL（自动复制到 mods 目录）
dotnet build

# 2. 导出 PCK（打包资源：图片、本地化JSON等）
megadot --headless --export-debug "BasicExport" MyFirstMod.pck

# 3. 复制 PCK 到 mods 目录
copy MyFirstMod.pck "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\MyFirstMod\"
```

> ⚠️ PCK 导出需要 MegaDot 导出模板已安装，详见 [DEVELOPMENT.md](DEVELOPMENT.md#⚠️-pck-导出模板问题)

## 项目结构

```
MyFirstMod/
├── Code/
│   ├── Entry.cs                       # MOD 入口
│   ├── MyFirstModCardModel.cs         # 卡牌基类
│   ├── MyFirstModRelicModel.cs        # 遗物基类
│   ├── Cards/
│   │   ├── RapidFireCardModel.cs      # 速射关键字卡牌基类
│   │   └── TestCard.cs               # 速射攻击牌示例
│   ├── Keywords/
│   │   └── MyKeywords.cs             # 自定义关键字（速射）
│   ├── Relics/
│   │   └── SniperChipset.cs          # 初始遗物：狙击芯片
│   ├── CardPools/                     # 卡池
│   ├── RelicPools/                    # 遗物池
│   └── PotionPools/                   # 药水池
├── myfirstmod/
│   ├── images/cards/                  # 卡图
│   ├── images/relics/                 # 遗物图标
│   └── localization/zhs/             # 中文本地化
├── MyFirstMod.csproj
├── myfirstmod.json
└── DEVELOPMENT.md                     # ← 开发经验文档
```

## 已实现功能

| 功能 | 文件 | 说明 |
|------|------|------|
| Exusiai 角色 | `Code/CardPools/`, `Code/RelicPools/` | 专属卡池/遗物池/药水池 |
| 速射关键字 | `Code/Keywords/MyKeywords.cs` | 打出时生成0费复制，复制不再触发 |
| 狙击芯片（初始遗物） | `Code/Relics/SniperChipset.cs` | 首张攻击牌额外打出1次 |
| TestCard | `Code/Cards/TestCard.cs` | 1费12伤速射攻击牌 |

## 测试方法

**战斗中获取卡牌**：
1. 开始一场战斗
2. 按 `~` 打开控制台
3. 输入: `card MYFIRSTMOD-TEST_CARD`

### 常用控制台命令

```bash
fight                    # 开始随机战斗
card CARD_ID            # 获得指定卡牌
kill                    # 杀死所有敌人
deck                    # 查看当前牌组
gold 999                # 获得金币
```

## 详细开发文档

👉 **[DEVELOPMENT.md](DEVELOPMENT.md)** — 包含完整的踩坑经验、Namespace地图、遗物Hook用法、PCK导出修复等

## 参考资料

- [官方教程](https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/)
- [模板项目](https://github.com/Alchyr/ModTemplate-StS2)

