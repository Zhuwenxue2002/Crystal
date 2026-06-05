# 名称引用专项移交记录（第六阶段）

更新时间：2026-06-05

## 目标

本记录原为第五阶段前置审计。第五阶段 NPC 与脚本自然语言汉化已于 2026-06-05 收口，名称引用专项正式移交为第六阶段主线。

第六阶段用于审计并联动汉化物品、怪物、NPC、地图/传送点等名称，避免只修改数据库显示名而脚本、任务、掉落表或配置仍按英文名查找。

结论：物品名、怪物名、NPC 名属于“数据库 + 文件引用联动汉化”，不能在数据库中单独批量改名。

## 已生成清单

- `docs/localization-phase-5-name-candidates.tsv`
  - 数据库候选名总表。
  - 来源：`ItemInfo.Name`、`MonsterInfo.Name`、`NPCInfo.Name`。
- `docs/localization-phase-5-name-references.tsv`
  - 名称引用审计表。
  - 覆盖：`Build/Server/Debug` 下的 `Configs` 与 `Envir` 文本文件。
  - 字段：类型、索引、英文名、引用次数、涉及文件数、涉及区域、风险、样例引用。

## 当前统计

| 类型 | 候选数 | 有文本/配置引用 | 暂未发现文本/配置引用 |
| --- | ---: | ---: | ---: |
| 物品 | 1628 | 1073 | 555 |
| 怪物 | 555 | 222 | 333 |
| NPC | 375 | 97 | 278 |

风险分布：

| 类型/风险 | 数量 |
| --- | ---: |
| Item / High | 193 |
| Item / Medium | 880 |
| Monster / High | 73 |
| Monster / Medium | 149 |
| NPC / High | 12 |
| NPC / Medium | 84 |
| NPC / Low | 1 |

低风险项当前只有：

| 类型 | 索引 | 英文名 | 引用 |
| --- | ---: | --- | --- |
| NPC | 182 | Ashes | `Envir/SET [].txt` 1 处 |

这说明名称直接汉化的风险很高，绝大多数名字必须同步处理引用文件。

## 引用热点

引用最多的物品多为药水、技能书、强化石、常见装备：

- `SunPotion`
- `SunPotion(M)`
- `(MP)DrugLarge`
- `(HP)DrugLarge`
- `(HP)DrugMedium`
- `(MP)DrugMedium`
- `(MP)DrugXL`
- `(HP)DrugXL`
- `AwakeningSoul0`
- `MCStone`
- `DCStone`
- `SCStone`
- `BenedictionOil`
- `Teleport`

这些名称大量出现在 `Envir/Drops`、`Envir/NPCs`、`Envir/Recipe`、`Configs`，不适合作为第一批直接改名。

## 处理规则

1. 不单独修改 `ItemInfo.Name`、`MonsterInfo.Name`、`NPCInfo.Name`。
2. 每个名称必须先确认所有引用位置：
   - 数据库字段
   - 掉落表
   - NPC 脚本
   - 任务正文
   - 配方
   - 系统脚本
   - 服务端配置
3. 对可同步的名称建立映射：
   - `TYPE`
   - `INDEX`
   - `SOURCE_NAME`
   - `TARGET_NAME`
   - `REFERENCE_FILES`
   - `APPLY_SCOPE`
   - `VERIFY_STATUS`
4. 涉及脚本命令参数的名称必须同步改文件引用，否则服务端按名查找会失败。
5. 对经典译名优先沿用术语表；扩展内容先采用稳定直译，后续游戏内验收再修正。

## 建议执行顺序

1. 先翻译 NPC/任务正文中的自然语言，不改名称引用。
2. 建立第一批名称映射，优先选择引用少、引用位置清晰的名字。
3. 对每批名称同时修改：
   - 数据库名称
   - 所有引用文件
   - 相关配置项
4. 每批后运行：
   - `LocalizationDbTool dump`
   - 服务端启动加载检查
   - 高频 NPC/任务游戏内验证

## 工具更新

`Tools/LocalizationDbTool` 新增只读命令：

```powershell
dotnet Tools/LocalizationDbTool/bin/Debug/net8.0/LocalizationDbTool.dll name-refs Build/Server/Debug
```

该命令只扫描并输出引用清单，不修改数据库或脚本文件。

随后补充名称应用能力：

```powershell
dotnet Tools/LocalizationDbTool/bin/Debug/net8.0/LocalizationDbTool.dll apply docs/localization-phase-5-name-mapping-npc-batch1.tsv
dotnet Tools/LocalizationDbTool/bin/Debug/net8.0/LocalizationDbTool.dll apply-name-refs docs/localization-phase-5-name-mapping-npc-batch1.tsv Build/Server/Debug
```

`apply` 可处理 `Item/Monster/NPC` 的 `Name` 字段；`apply-name-refs` 用同一份映射同步文本引用。

## 第一批 NPC 名称

映射文件：

- `docs/localization-phase-5-name-mapping-npc-batch1.tsv`

备份：

- `Build/Server/Debug/Backups/Server.MirDB.phase5-npc-batch1-20260603.bak`

本批选择 20 个低引用 NPC 显示名，引用主要位于任务文件注释行：

| 英文名 | 中文名 |
| --- | --- |
| `CraftsLady_Louise` | 工匠露易丝 |
| `Merchant_Hans` | 商人汉斯 |
| `Merchant_Janey` | 商人珍妮 |
| `Merchant_Jim` | 商人吉姆 |
| `Merchant_Rachel` | 商人瑞秋 |
| `Merchant_Ruben` | 商人鲁本 |
| `MirGuide_James` | 向导詹姆斯 |
| `OldSkull` | 古旧头骨 |
| `Sir_Mogu` | 莫古爵士 |
| `Teleport_Harrison` | 传送员哈里森 |
| `Transport_Edwin` | 运输员埃德温 |
| `BrokenCarriage` | 破损马车 |
| `CraftsLady_Jude` | 工匠朱迪 |
| `CrushedBones` | 碎骨 |
| `Investigator_David` | 调查员大卫 |
| `Master_Perry` | 佩里大师 |
| `MasterMK_Deric` | 武学大师德里克 |
| `Merchant_Daniel` | 商人丹尼尔 |
| `Merchant_Jerald` | 商人杰拉德 |
| `Merchant_Kevin` | 商人凯文 |

应用结果：

- 数据库名称修改：20 条。
- 文本引用同步：29 处。
- 修改后 dump：`docs/localization-phase-5-db-dump-after-npc-batch1.tsv`。
- 修改后引用扫描：`docs/localization-phase-5-name-references-after-npc-batch1.tsv`。
- 校验：20 个旧英文名在数据库 dump 和 `Build/Server/Debug` 文本文件中均未再命中；20 个中文名均在数据库 dump 中命中。
- 服务端加载验证：使用本地 `.tools/dotnet/dotnet.exe` 启动 `Build/Server/Debug/Server.dll`，日志显示“环境已启动”“网络已启动”。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 名称修改引入。

## 第二批 NPC 名称

映射文件：

- `docs/localization-phase-5-name-mapping-npc-batch2.tsv`

备份：

- `Build/Server/Debug/Backups/Server.MirDB.phase5-npc-batch2-20260603.bak`

本批继续处理低引用任务入口 NPC 与场景物件名：

| 英文名 | 中文名 |
| --- | --- |
| `MudWall_Board` | 盟重公告板 |
| `OldSkeleton` | 古旧骷髅 |
| `SkeletonPile` | 骷髅堆 |
| `SubjagationManager_James` | 征讨管理员詹姆斯 |
| `VulnerableSon_Alfie` | 虚弱的阿尔菲 |
| `InnKeeper_Robin` | 旅店老板罗宾 |
| `Merchant_John` | 商人约翰 |
| `Merchant_Sam` | 商人山姆 |
| `Merchant_Sarah` | 商人莎拉 |
| `MongchonScout_Brian` | 盟重斥候布莱恩 |
| `Teleport_Jason` | 传送员杰森 |

应用结果：

- 数据库名称修改：12 条。
- 文本引用同步：28 处。
- 修改后 dump：`docs/localization-phase-5-db-dump-after-npc-batch2.tsv`。
- 修改后引用扫描：`docs/localization-phase-5-name-references-after-npc-batch2.tsv`。
- 校验：本批旧英文名在数据库 dump 和 `Build/Server/Debug` 文本文件中均未再命中；中文名已进入数据库并同步到文本引用。
- 服务端加载验证：使用本地 `.tools/dotnet/dotnet.exe` 启动 `Build/Server/Debug/Server.dll`，日志显示“环境已启动”“网络已启动”。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 名称修改引入。
