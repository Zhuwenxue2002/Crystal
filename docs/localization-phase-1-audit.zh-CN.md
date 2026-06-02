# 第一阶段汉化审计记录

更新时间：2026-06-02

## 当前状态

- 服务端已成功编译，并可通过本地 `.tools/dotnet/dotnet.exe` 启动 `Server.dll`。
- 数据库 release 已应用到服务端调试输出目录。
- 客户端和服务端默认语言已设置为 `Chinese`。
- `Server.MirForms/Server.csproj` 已将 `log4net` 调整为 `3.3.0`，这是当前构建成功所需的依赖修复。
- 已建立第一版术语表：`docs/localization-glossary.zh-CN.md`。
- 已完成第一批术语统一：职业、属性、行会、退出游戏等高频词。

## 语言文件入口

| 模块 | 英文资源 | 中文资源 | 状态 |
| --- | --- | --- | --- |
| 客户端 | `Client/Localization/English.json` | `Client/Localization/Chinese.json` | 需继续统一术语 |
| 服务端/管理端 | `Server.MirForms/Localization/English.json` | `Server.MirForms/Localization/Chinese.json` | JSON 结构正常 |
| 枚举与默认文本 | `Shared/Language.cs` | 同文件 | 后续清理硬编码与默认英文 |

## JSON key 检查

PowerShell 默认哈希表大小写不敏感，曾误报以下 key 为重复。经源码核对，它们是大小写不同的独立枚举项，严格大小写下不是重复键：

| 文件 | key | 说明 |
| --- | --- | --- |
| 客户端中英文 JSON | `LogOut` / `Logout` | 两个不同枚举项 |
| 客户端中英文 JSON | `Online` / `ONLINE` | 两个不同枚举项 |
| 客户端中英文 JSON | `YouDoNotHaveEnoughBaseMac` / `YouDoNotHaveEnoughBaseMAC` | 两个不同枚举项，但命名非常接近 |

这些 key 不应直接删除或合并；后续如要整理，需要同步修改 `Shared/Language.cs` 和所有引用点。

## 已发现术语不一致

| 位置 | 当前问题 | 建议 |
| --- | --- | --- |
| `Client/Localization/Chinese.json` | `Guild` 相关多处译为“公会” | 统一为“行会” |
| `Server.MirForms/Localization/Chinese.json` | `NotInGuild` 等已出现“帮会” | 统一为“行会” |
| `Server.MirForms/Localization/Chinese.json` | `LowSC` 译为“精神值不足” | 统一为“道术不足” |
| `Client/Localization/Chinese.json` | `Archer` 有“弓箭手” | 统一为“弓手” |
| 客户端快捷键文本 | `LogOut`、`Logout` 译法不一 | 根据按钮语境统一为“退出游戏” |

## 已完成修正

| 范围 | 修正 |
| --- | --- |
| `Client/Localization/Chinese.json` | `公会` 统一为 `行会` |
| `Client/Localization/Chinese.json` | `弓箭手` 统一为 `弓手` |
| `Client/Localization/Chinese.json` | `法力` 统一为 `魔法值` |
| `Client/Localization/Chinese.json` | `LowDC/LowMC/LowSC` 修正为 `攻击/魔法/道术` |
| `Client/Localization/Chinese.json` | `登出/注销游戏` 统一为 `退出游戏` |
| `Server.MirForms/Localization/Chinese.json` | `帮会/公会` 统一为 `行会` |
| `Server.MirForms/Localization/Chinese.json` | `精神值` 统一为 `道术` |
| `Server.MirForms/Localization/Chinese.json` | `LowDC/LowMC/LowSC` 修正为 `攻击/魔法/道术` |

## 构建验证

- `dotnet build Server.MirForms/Server.csproj -c Debug --no-restore`：成功，0 警告，0 错误。
- `dotnet build Client/Client.csproj -c Debug --no-restore`：成功，1 个既有 `WindowsBase` 版本冲突警告，0 错误。

## 自动化审计结果

| 检查项 | 客户端 | 服务端/管理端 |
| --- | --- | --- |
| 中英文 key 数量 | 一致 | 一致 |
| 中文缺失 key | 0 | 0 |
| 中文额外 key | 0 | 0 |
| 精确重复 key | 0 | 0 |
| 占位符编号不一致 | 0 | 0 |
| 术语残留扫描 | 0 | 0 |

客户端语言文件存在 3 组大小写相近的 key，它们在 C# 枚举中是独立项，暂不合并：

| key 组 | 说明 |
| --- | --- |
| `LogOut` / `Logout` | 不同客户端文本位置 |
| `Online` / `ONLINE` | 普通文本与大写标签 |
| `YouDoNotHaveEnoughBaseMac` / `YouDoNotHaveEnoughBaseMAC` | 命名接近但当前为独立枚举 |

客户端中文文件中仍保留两个纯英文显示值：

| key | 值 | 处理 |
| --- | --- | --- |
| `ItemTypeDeco` | `Deco` | 类型短标签，暂保留，后续 UI 验收时决定是否改为“装饰” |
| `BtnEsc` | `Esc` | 键盘按键名，保留 |

本轮修复了客户端中文文件中多出的 `{0}` 占位符：

| key | 修正 |
| --- | --- |
| `Skills` | `技能 ({0})` -> `技能` |
| `Groups` | `队伍 ({0})` -> `队伍` |
| `Quests` | `任务 ({0})` -> `任务` |
| `Fishing` | `钓鱼 ({0})` -> `钓鱼` |
| `LogOut` | `退出游戏 ({0})` -> `退出游戏` |
| `Trade` | `交易 ({0})` -> `交易` |
| `Guild` | `行会（{0}）` -> `行会` |
| `Exit` | `退出 ({0})` -> `退出` |

## 第一批建议改动

1. 按 `docs/localization-glossary.zh-CN.md` 统一客户端和服务端 JSON 中的高频术语。
2. 先处理职业、属性、行会、基础系统消息，不碰数据库。
3. 每批 JSON 改动后执行构建，确认语言文件能复制到 `Build` 目录。
4. 启动服务端检查是否加载中文资源。
5. 进入客户端后逐页记录仍显示英文、乱码、溢出的界面。

## 暂不处理

- 暂不批量修改 `Server.MirDB`，因为数据库内容可能被脚本按名称引用。
- 暂不翻译脚本命令、变量、标签和文件名。
- 暂不清理大小写相近的枚举 key，避免引入引用错误。
