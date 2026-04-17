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

### 2. 项目结构

```
MyFirstMod/
├── Code/
│   ├── Entry.cs                    # MOD 入口
│   ├── MyFirstModCardModel.cs      # 卡牌基类（统一管理卡图路径）
│   └── Cards/
│       └── TestCard.cs             # 具体卡牌
├── myfirstmod/                     # 资源目录（必须是 modid）
│   ├── images/cards/               # 卡图目录
│   └── localization/zhs/           # 中文本地化
├── MyFirstMod.csproj               # 项目配置
├── myfirstmod.json                 # MOD 元数据
└── export_presets.cfg              # Godot 导出配置
```

### 3. 卡牌开发流程

1. 继承自定义基类（不是直接继承 CustomCardModel）
2. 添加 `[Pool(typeof(ColorlessCardPool))]` 注册到卡池
3. 卡图命名：`{ClassName}.png`（如 TestCard.png）
4. 本地化 key：`{MODID}-{CARD_ID}.title` 和 `.description`
5. 卡牌 ID 格式：`{命名空间首段大写}-{类名SNAKE_CASE}`

### 4. 常见问题

**问题1：卡牌未注册**
- 检查 BaseLib 版本是否匹配
- 检查 dll_name 是否正确
- 查看日志：`%AppData%\Roaming\SlayTheSpire2\logs\godot.log`

**问题2：卡图不显示**
- 确保 PCK 已导出并复制到 mods 目录
- 检查 has_pck 是否设置为 true
- 卡图路径：`res://{modid}/images/cards/{ClassName}.png`

**问题3：中文不显示**
- 检查 localization/zhs/cards.json 是否在 PCK 中
- 检查 key 格式是否正确

### 5. 测试命令

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

### 6. 编译和部署

```bash
# 编译 DLL
dotnet build

# 导出 PCK（需要 Godot）
godot --headless --export-pack "BasicExport" "MyFirstMod.pck"

# 文件会自动复制到 mods 目录
```

## 经验总结

1. **版本锁定是关键** — BaseLib 版本不匹配是最常见的错误
2. **先验证 DLL 加载** — 确保 MOD 能加载后再添加资源
3. **日志是第一手资料** — 出问题先看日志，不要猜
4. **资源路径要规范** — modid 目录名必须和 json 中的 id 一致
5. **测试要在战斗中** — card 命令只能在战斗中使用
