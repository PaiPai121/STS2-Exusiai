# CLAUDE_NOTES

## 强制规则
1. 优先复用历史已验证 build / export / deploy 命令。
2. 禁止猜测 csproj 名称、路径、导出入口、部署目录。
3. 若与上次成功链路不同，必须先说明原因与证据。
4. 修复失败时，必须先判断问题属于：入口层 / 编译层 / 导出层 / 运行层。
5. 非用户明确要求前，不得擅自改动已稳定资源链路。
6. 游戏日志目录固定为：
   `C:\Users\HunterAndDragon\AppData\Roaming\SlayTheSpire2\logs`

## 当前项目执行纪律
- 先读 `RUNBOOK.md`，再做 build / export / deploy。
- 先确认 shell 与工作目录，再执行命令。
- 汇报必须带实际命令与实际结果，禁止空口完成。
- 只有需要用户亲自打开游戏验证时，才允许停下来等用户。
