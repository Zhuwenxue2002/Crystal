# 第二阶段客户端界面汉化记录

更新时间：2026-06-02

## 阶段目标

优先处理客户端中玩家最常见、最容易暴露问题的界面文本：

- 启动器与登录流程
- 角色创建与职业说明
- 主界面、背包、人物、技能、任务、行会、邮件、交易
- 技能说明、物品提示、快捷键提示
- 游戏内明显英文、乱码、占位符错误和中文语病

## 第一批扫描结果

客户端中文语言文件中仍含英文的条目主要分为两类：

| 类型 | 示例 | 处理 |
| --- | --- | --- |
| 应翻译文本 | 技能说明中的 `Instant Casting`、`Mana Cost`、`Current Skill Level` | 已修复 |
| 可保留技术/按键文本 | `NPC`、`Ctrl`、`HOST`、`BROWSER`、`Error.txt`、邮箱示例 | 暂保留，进入 UI 验收时再决定 |

## 已完成修正

| 文件 | key | 修正 |
| --- | --- | --- |
| `Client/Localization/Chinese.json` | `FireballSkillDescription` | 移除误混入的 `ThunderboltSkillDescription` 文本 |
| `Client/Localization/Chinese.json` | `ThunderboltSkillDescription` | 将英文技能说明翻译为中文 |
| `Client/Localization/Chinese.json` | `TrapHexagonSkillDescription` | 修正“魔魔法值量”为“魔法力量” |
| `Client/Localization/Chinese.json` | `MassHealingSkillDescription` | 修正“用魔法值笼罩”为“用治疗之力笼罩” |
| `Client/Localization/Chinese.json` | `MeteorStrikeSkillDescription` | 统一范围写法为 `5×5` |

## 自动化验证

- 客户端占位符编号检查：0 个不一致。
- 明显英文技能说明扫描：已清理本批发现项。
- `dotnet build Client/Client.csproj -c Debug --no-restore`：成功，0 错误，保留既有 `WindowsBase/WebView2` 警告。

## 待继续处理

- 逐页进入客户端 UI，记录仍显示英文、中文过长、按钮遮挡的位置。
- 继续人工审查技能说明，重点处理“下一级/下一等级”不一致、“魔法消耗/魔法值消耗”不一致。
- 检查角色创建界面职业说明是否过长或溢出。
- 检查行会、任务、邮件、交易面板中的固定坐标中文显示。
