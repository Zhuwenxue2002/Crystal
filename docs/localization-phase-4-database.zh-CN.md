# 第四阶段数据库内容汉化记录

更新时间：2026-06-03

## 阶段目标

第四阶段开始处理运行数据库 `Build/Server/Debug/Server.MirDB` 中直接面向玩家显示的文本。

本阶段遵循计划中的风险约束：

- 修改前备份数据库。
- 优先处理不依赖脚本按名称查找的字段。
- 物品、怪物、NPC 名称先导出候选和映射，暂不批量改名，避免掉落文件、任务脚本和配置按英文名查找失败。

## 数据库位置与备份

实际运行数据库位于：

- `Build/Server/Debug/Server.MirDB`

手动备份：

- `Build/Server/Debug/Backups/Server.MirDB.phase4-20260603.bak`

工具保存数据库时，服务端原有 `SaveDB` 逻辑也会在运行目录 `Back Up/Database` 下生成一次 `.bak` 备份。

## 辅助工具

新增 `Tools/LocalizationDbTool`：

- `dump`：使用项目自身 `Envir.LoadDB()` 读取数据库，导出技能、地图、任务以及物品/怪物/NPC 候选名称。
- `apply <mapping.tsv>`：按映射表修改低风险字段，然后使用项目自身 `Envir.SaveDB()` 保存数据库。

导出和映射文件：

- `docs/localization-phase-4-db-dump.tsv`
- `docs/localization-phase-4-db-mapping.tsv`
- `docs/localization-phase-4-db-dump-after.tsv`
- `docs/localization-phase-4-map-titles.unique.txt`
- `docs/localization-phase-4-db-mapping-2.tsv`
- `docs/localization-phase-4-db-dump-after-2.tsv`
- `docs/localization-phase-4-db-dump-after-2-tooltips.tsv`
- `docs/localization-phase-4-item-tooltips.unique.txt`
- `docs/localization-phase-4-db-mapping-3-tooltips.tsv`
- `docs/localization-phase-4-db-dump-after-3.tsv`
- `docs/localization-phase-4-db-mapping-4.tsv`
- `docs/localization-phase-4-db-dump-after-4.tsv`
- `docs/localization-phase-4-db-mapping-5.tsv`
- `docs/localization-phase-4-db-mapping-5b-tooltips.tsv`
- `docs/localization-phase-4-db-dump-after-5.tsv`
- `docs/localization-phase-4-remaining-after-5.tsv`

## 第一批修正

| 范围 | 修正 |
| --- | --- |
| 技能名称 | 全量汉化 `MagicInfo.Name`，共 111 条，包含战士、法师、道士、刺客、弓手和扩展技能 |
| 地图标题 | 汉化 1-120 号早期/经典区域 `MapInfo.Title`，覆盖比奇省、沃玛森林、天然洞穴、兽人古墓、废矿、沃玛寺庙、基隆城等 |
| 任务字段 | 汉化 1-54 号新手与早期任务的 `Name`、`Group`、`GotoMessage` 字段 |
| 候选清单 | 导出 1628 条物品、555 条怪物、375 条 NPC 名称作为后续批次候选，暂不直接改名 |

本批共应用 389 条数据库字段修改。

## 第二批修正

| 范围 | 修正 |
| --- | --- |
| 地图标题 | 继续汉化 121 号以后的地图标题，覆盖赤月峡谷、毒蛇山谷、盟重省、石墓、沙巴克、祖玛寺庙、般若岛、过去比奇、雪域、行会试炼、狗妖矿区等区域 |
| 任务字段 | 汉化 54-111 号任务的 `Name`、`Group`、`GotoMessage`、少量 `KillMessage` 字段 |

本批共应用 511 条数据库字段修改。

## 第三批修正

| 范围 | 修正 |
| --- | --- |
| 辅助工具 | `LocalizationDbTool` 增加 `ItemInfo.ToolTip` 导出与应用能力 |
| 物品提示 | 汉化 91 条低风险 `ItemInfo.ToolTip`，覆盖太阳水、修理油、护身符、回城/行会卷轴、宠物蛋与宠物道具、商城增益、技能书说明等 |
| 物品名称 | 仍不直接修改 `ItemInfo.Name`，避免掉落、任务、商城、配方按英文名称查找失败 |

本批共应用 91 条数据库字段修改。

## 第四批修正

| 范围 | 修正 |
| --- | --- |
| 任务字段 | 汉化 111-154 号任务的 `Name`、`Group`、`GotoMessage`、`ItemMessage`、`FlagMessage` 字段，覆盖盟重后续、般若岛、荒地、公告板、圣剑任务和远古兽人洞穴 |
| 物品提示 | 继续汉化 物品 `ToolTip`，覆盖圣水盒药水、城镇/地牢传送卷、喊话号角、限时药水、狩猎礼包、钓鱼礼包、部分扩展技能书说明等 |
| 保留范围 | 仍保留外观名、短装扮标签和疑似内部占位文本，等待物品名称专项统一处理 |

本批共应用 163 条数据库字段修改。

## 第五批修正

| 范围 | 修正 |
| --- | --- |
| 辅助工具 | 修正 `LocalizationDbTool` 的反转义逻辑，避免 `\r\n`、`\t` 和 `\\` 连续替换时互相污染 |
| 任务字段 | 补齐 29、44、54、59 号任务早期目标提示漏项；数据库任务字段中的英文 `VALUE` 计数已降为 0 |
| 物品提示 | 修正/补译 106 条 `ItemInfo.ToolTip`，覆盖宠物蛋、宠物石/饲料、神物盒、技能书说明、复活/PK/性别/神秘石卷轴、高级火炬与时间单位等 |
| 审计清单 | 生成 `docs/localization-phase-4-remaining-after-5.tsv`，记录仍含拉丁字符的提示项；其中包含 `\r\n` 转义、HP/MP/PK/EXP 缩写、命令名和外观标签等误报 |

本批共应用 110 条数据库字段修改，其中任务字段 4 条、物品提示 106 条。

## 风险处理

- 未修改物品名称：掉落文件通过 `Envir.GetItemInfo(parts[1])` 按名称解析，直接改名会导致掉落读取失败。
- 未修改怪物名称：部分怪物 AI 和配置通过 `Settings.*Name` 或 `Envir.GetMonsterInfo(name)` 查找，直接改名需同步配置与脚本。
- 未修改 NPC 名称：NPC 脚本、任务和地图显示可能依赖英文文件名或带前缀名称，需与第五阶段脚本汉化一起处理。
- 任务正文仍在 `Server/Envir/Quests` 文本文件中，属于第五阶段；本批只处理数据库内任务标题、分组和目标短句。

## 自动化验证

- `LocalizationDbTool dump`：修改前可成功读取 `Server.MirDB` 并导出清单。
- `LocalizationDbTool apply docs/localization-phase-4-db-mapping.tsv`：成功应用 389 条修改。
- 修改后再次执行 `LocalizationDbTool dump`：成功读取数据库。
- 映射命中校验：389 条映射全部在修改后 dump 中命中，0 条缺失。
- `dotnet build Server.MirForms/Server.csproj -c Debug --no-restore`：成功，0 错误，保留既有 1 个 `PlayerInfoForm.Location` 隐藏成员警告。
- 第二批后再次执行 `LocalizationDbTool dump`：成功读取数据库。
- 第二批映射命中校验：511 条映射全部在修改后 dump 中命中，0 条缺失。
- 第三批后再次执行 `LocalizationDbTool dump`：成功读取数据库。
- 第三批映射命中校验：91 条映射全部在修改后 dump 中命中，0 条缺失。
- 第三批后执行 `dotnet build Server.MirForms/Server.csproj -c Debug --no-restore`：成功，0 错误，0 警告。
- 第四批前备份：`Build/Server/Debug/Backups/Server.MirDB.phase4-batch4-20260603.bak`。
- 第四批后再次执行 `LocalizationDbTool dump`：成功读取数据库。
- 第四批映射命中校验：163 条映射全部在修改后 dump 中命中，0 条缺失。
- 第四批后执行 `dotnet build Server.MirForms/Server.csproj -c Debug --no-restore`：成功，0 错误，0 警告。
- 第五批前备份：`Build/Server/Debug/Backups/Server.MirDB.phase4-batch5-20260603.bak`。
- 第五批映射应用：`docs/localization-phase-4-db-mapping-5.tsv` 合计 110 条，最终 dump 中全部命中，0 条缺失。
- 第五批后再次执行 `LocalizationDbTool dump`：成功读取数据库并导出 `docs/localization-phase-4-db-dump-after-5.tsv`。
- 第五批后数据库任务字段英文计数：0 条。
- 第五批后执行 `dotnet build Server.MirForms/Server.csproj -c Debug --no-restore`：成功，0 错误，0 警告。
- 说明：第三批部分物品提示译文已被第五批更完整译文覆盖；累计校验应以每个字段的最新映射为准。

## 待继续处理

- 剩余物品提示审计项主要是装扮外观短标签、缩写、命令名、转义标记、部分韩服/扩展物品名和疑似内部占位文本，建议并入物品名称专项统一处理。
- 任务数据库标题、分组和目标短句已完成一轮主线范围处理；后续重点应转到 `Server/Envir/Quests` 正文，否则玩家打开任务详情时仍会看到英文正文。
- 物品名称需要先扫描 `Server/Envir/MonItems`、任务文件、商城和配方引用，再按“数据库名称 + 引用文件”一起改。
- 怪物名称需要先扫描刷新配置、怪物脚本和 `Server/Settings.cs` 中的按名查找配置，再成批改。
- NPC 名称和任务正文建议并入第五阶段 NPC/脚本汉化，避免只改显示名而脚本仍保留英文造成体验割裂。
