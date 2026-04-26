# RUNBOOK

## 强制执行规则
1. Build / export / deploy 必须优先复用历史已验证命令。
2. 禁止猜测 csproj 名称、路径、导出入口、部署目录。
3. 若命令与上次成功链路不同，必须先说明原因与证据。
4. 若出现失败，先判断是入口层 / 编译层 / 导出层 / 运行层，不得混淆。
5. 只有旧链路已证实失效时，才允许改入口。

## 已验证路径
- Project root: `D:\work_console\MyFirstMod`
- C# project: `D:\work_console\MyFirstMod\MyFirstMod.csproj`
- Log directory: `C:\Users\HunterAndDragon\AppData\Roaming\SlayTheSpire2\logs`
- Game mod deploy directory: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\MyFirstMod`
- MegaDot exporter root: `D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp`

## 执行前检查
- [ ] 先读取本文件与 `CLAUDE_NOTES.md`
- [ ] 确认当前 shell / 工作目录
- [ ] 复用已验证 build 命令，不得猜测项目入口
- [ ] 复用已验证 export 命令，不得切换 Godot/MegaDot 版本
- [ ] 复用已验证 deploy 目录
- [ ] 执行后查看日志，而不是凭猜测汇报

## 已验证命令模板
### Build
```bash
dotnet build D:\work_console\MyFirstMod\MyFirstMod.csproj
```

### Export PCK
```bash
"D:\work_console\workspaceforexusuai\megadot-4.5.1-m.9-windows-x86_64-llvm-editor-csharp\MegaDot_v4.5.1-stable_mono_win64_console.exe" --headless --path "D:\work_console\MyFirstMod" --export-pack BasicExport "D:\work_console\MyFirstMod\MyFirstMod.pck"
```

### Deploy
需要同步以下内容到游戏 mods 目录：
- `bin/Debug/net8.0/MyFirstMod.dll`
- `MyFirstMod.pck`
- `myfirstmod/`
- `myfirstmod.json`

## 故障分层
1. 入口层：路径、shell、工作目录、命令入口是否正确
2. 编译层：C# 语法、分析器、依赖、项目构建错误
3. 导出层：PCK 导出失败、导出器版本不匹配
4. 运行层：游戏黑屏、资源加载失败、日志异常

## 汇报格式要求
每次执行 build / export / deploy 前，先显式列出：
- 本次复用的 build 命令
- 本次复用的 export 命令
- 本次复用的 deploy 目录
- 本次将查看的日志目录

如果其中任何一项与历史链路不同，必须先写明差异与证据。
