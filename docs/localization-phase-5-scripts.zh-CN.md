# 第五阶段 NPC 与脚本汉化启动记录

更新时间：2026-06-03

## 阶段目标

第五阶段开始处理运行目录 `Build/Server/Debug/Envir` 中面向玩家显示的 NPC、任务、公告、活动与系统脚本文本。

本阶段遵循总计划中的风险约束：

- 只翻译玩家可见文本，不翻译脚本命令、变量名、跳转标签、文件引用、物品/怪物/NPC 内部名称。
- 优先处理低风险文本，再处理高频 NPC 对话。
- 每批修改后运行脚本加载或服务端构建验证，避免破坏 NPC 脚本语法。
- 涉及物品名、怪物名、地图文件名、名单文件名的内容先保留英文引用，等名称专项联动处理。

## 实际工作目录

源码目录 `Server/Envir` 当前只有 `Drops` 和空的 `Quests` 目录；完整运行环境位于：

- `Build/Server/Debug/Envir`

第五阶段应以运行目录为准。后续如果需要把运行环境同步回源码或发布包，应单独建立同步记录。

## 当前范围盘点

| 范围 | 文件数 | 处理建议 |
| --- | ---: | --- |
| `NPCs` | 635 | 主体工作。优先公告板、向导、传送、仓库、商人、任务入口等高频 NPC。 |
| `Quests` | 161 | 任务正文。第四阶段已处理数据库任务标题/分组/目标短句，本阶段补齐正文。 |
| `NameLists` | 4 | 多为名单/状态文件，通常不翻译文件名和记录结构。 |
| `Events` | 18 | 活动脚本，按 NPC 脚本规则处理。 |
| `SystemScripts` | 27 | 登录、使用物品、回城卷、任务触发等脚本，谨慎处理显示文本。 |
| `Notice.txt` / `LoginNotice.txt` / `LineMessage.txt` | 3 | 低风险公告文本，建议作为第一批。 |

暂不纳入本阶段首批：

- `Drops`：掉落文件按物品名查找，和物品名称专项强绑定。
- `Goods`：商店库存二进制/数据文件，优先保持原状。
- `Recipe`：配方按物品名查找，和物品名称专项强绑定。
- `Routes`、`Values`、`Previews`：多数为坐标、变量或资源文件，不作为翻译主体。

## 脚本语法保护规则

根据 `Server/MirObjects/NPC/NPCScript.cs` 和 `Server/MirObjects/NPC/NPCSegment.cs`：

- 保留段名：`[@MAIN]`、`[@Main-1]`、`[@BUY]`、`[@SELL]`、`[@STORAGE]` 等。
- 保留控制块：`#IF`、`#SAY`、`#ACT`、`#ELSESAY`、`#ELSEACT`。
- 保留脚本命令：如 `CHECKITEM`、`CHECKQUEST`、`GOTO`、`GIVEITEM`、`TAKEITEM`、`GLOBALMESSAGE`、`MOVE`、`MONGEN`、`LOADVALUE`、`SAVEVALUE`。
- 保留脚本引用：`#INSERT [...]`、`#INCLUDE [...] ...` 中的路径和页名。
- 保留跳转目标：`<显示文本/@target>` 中只翻译斜杠前的显示文本，`/@target` 不动。
- 保留特殊变量和值占位：`%ARG(...)`、`%USERNAME`、`%USERLEVEL`、`%USERGOLD`、`%A1`、`[flag]` 等。
- 保留配置段：`[TRADE]`、`[RECIPE]`、`[TYPES]`、`[USEDTYPES]`、`[QUESTS]`、`[SPEECH]`。
- `[TRADE]`、`[RECIPE]` 下的物品名暂不翻译，避免 `Envir.GetItemInfo(...)` 查找失败。
- `[QUESTS]` 下的数字只表示任务 ID，不翻译。
- `[SPEECH]` 下格式为 `权重 文本`，只翻译权重后的文本。

## 建议批次

1. 第一批：`Notice.txt`、`LoginNotice.txt`、`LineMessage.txt`。
2. 第二批：新手/主城高频 NPC，如 `BichonProvince/BichonWall/MirGuide*.txt`。
3. 第三批：任务正文 `Quests/BichonProvince`、`Quests/MongchonProvince`、`Quests/SerpentValley` 等主线区域。
4. 第四批：传送、仓库、商人、行会、宠物、活动 NPC。
5. 第五批：`Events` 与 `SystemScripts` 中的玩家可见文本。

每批建议保留一份映射或变更说明，记录英文原文、中文译文、文件路径和是否包含名称引用。

## 翻译风格

- 沿用 `localization-glossary.zh-CN.md`：战士、法师、道士、刺客、弓手、行会、攻击、魔法、道术、防御、魔御。
- 地名沿用第四阶段数据库译名：比奇省、盟重省、沃玛森林、毒蛇山谷、祖玛寺庙、石墓、沙巴克、般若岛、白色山谷、冰封宫殿等。
- NPC 口吻保持简洁自然，避免现代网页式说明。
- 按钮文本尽量短：返回、关闭、主页、传送、购买、出售、修理、仓库、接取任务、完成任务。
- 保留常见缩写：HP、MP、EXP、PK、GM。

## 验收建议

- 文本级检查：确认修改后没有破坏 `[@...]`、`#...`、`<.../@...>`、`[TRADE]` 等结构。
- 加载级检查：启动服务端或运行可加载 NPC 脚本的构建路径，观察脚本未找到、物品未找到、未知命令等日志。
- 游戏内检查：至少覆盖主城向导、公告、传送、仓库、商人、接取任务、完成任务。
- 问题清单：继续维护“仍为英文/疑似内部引用/需名称联动/需游戏内确认”的记录。

## 已确认的第一批原文

`Notice.txt` 和 `LoginNotice.txt` 当前标题为 `Welcome to Legend of Mir 2`，正文为服务条款提示和 LOMCN/数据库来源链接；`LineMessage.txt` 当前包含 Crystal Mir2 发布说明、GitHub 数据库提示、LOMCN 链接和 `.NET 8` 说明。

链接和项目名可保留，欢迎语和说明文本可翻译。

## 第一批公告文本

更新时间：2026-06-03

已翻译低风险公告文件：

- `Build/Server/Debug/Envir/Notice.txt`
- `Build/Server/Debug/Envir/LoginNotice.txt`
- `Build/Server/Debug/Envir/LineMessage.txt`

处理原则：

- 保留 `TITLE=` 结构。
- 保留 LOMCN、DATABASE、GitHub、Suprcode、Crystal Mir 2、`.NET 8` 等项目名、链接和技术名。
- 翻译欢迎语、必读提示、服务条款提示和数据库发布说明。

验证：

- 复读文件确认 UTF-8 中文显示正常。
- 英文残留仅为链接、项目名、协议字段和技术名。
- 使用本地 `.tools/dotnet/dotnet.exe` 启动 `Build/Server/Debug/Server.dll`，日志显示“环境已启动”“网络已启动”。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批公告文本修改引入。

## 第二批系统脚本文本

更新时间：2026-06-03

已翻译低风险系统脚本中的玩家可见文本：

- `Build/Server/Debug/Envir/SystemScripts/00Default/UseItems.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/CustomCommands.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/GuardsHelp.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/HeroUseItems.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/LevelUp.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/TownScroll.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/DungeonScroll.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/TestServerScroll.txt`
- `Build/Server/Debug/Envir/SystemScripts/SharedNPCS/Tavern.txt`

处理原则：

- 保留 `[@...]`、`#IF`、`#ACT`、`#SAY`、`MOVE`、`GIVEITEM`、`CHECKITEM` 等脚本结构。
- 保留物品名、地图编号、怪物名、Buff 名、命令名和 `@跳转` 标签。
- 只翻译 `LocalMessage`、`GLOBALMESSAGE`、`#SAY` 正文和 `<显示文本/@跳转>` 的显示文本。
- 测试服命令页保留具体 `@命令`，只翻译页标题和返回按钮。

验证：

- 已扫描并清理 `Back/@`、`Close/@`、`Teleport to:`、`Teleport Region:`、`Available Dungeons`、`Welcome`、`Thank`、`Not added` 等本批应处理英文残留。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批系统脚本翻译引入。

## 第三批短 NPC 对话

更新时间：2026-06-03

已翻译一批短 NPC 脚本中的 `#SAY` 正文和按钮显示文本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/ArcherMage.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/ArchMage-0115.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/HighPriest.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/HighPriest-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/ArcherCaptain.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/ArcherCaptain-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/DeadMine/OldSkeleton.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/14Qob-D404.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/SeokchoValley/Signpost.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/InsectCave/Soho.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/BrokenCarriage.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/BrokenCarriage.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/CrushedBonesTP.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/TreePath/CrushedBonesTP.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/SDeadMine/OldSkull.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/OldSkullSDM2.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/SDeadMine/SkeletonPile.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/SkeletonPileSDM.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/LostSoulDV.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/BugCave/LostSoul.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/SquadLeader.txt`

处理原则：

- 保留 `[@...]`、`[Quests]`、任务编号、文件名和跳转标签。
- 只翻译自然语言与 `<关闭/@exit>`、`<离开/@exit>` 这类按钮显示文本。
- 复制/镜像 NPC 文件保持译文一致。

验证：

- 对本批文件扫描 `Close/@`、`Exit/@`、`Welcome`、`Hello`、`Looks like`、`missing`、`Bounty Board` 等旧英文模式，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 启动 `Build/Server/Debug/Server.dll`，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第四十二批 WoomyonWoods 开门脚本与 Castle-GI 基础服务 NPC

更新时间：2026-06-04

本批继续处理 WoomyonWoods，覆盖 TaoistVillage TreePath 开门脚本、根目录镜像开门脚本，以及 Castle-GI 的基础服务 NPC 与编号镜像文件：

- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/TreePath/Pillar.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/14Plv-12.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Weaponsmith.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/2Giwe.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Drapery.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/3Gidr.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Potion.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/4Gidu.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Potion1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/4Gidm.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Peddlar.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/7Gist.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Jewellery.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/8Giac.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/6Giwh.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Transporter.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/CastleGi_Transporter.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Crafting.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/11Gicft.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/2Gibl.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Stones.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/8GiStn.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Butcher.txt`

覆盖内容：

- 石柱/封门脚本的进入确认、Ancient Post Piece 放置提示、死亡与绝望区域说明。
- Castle-GI 武器、服装、药水、首饰、仓库、杂货、屠夫、制作与特殊修理 NPC 的问候、PK 拒绝、交易、修理、回购、毒药、仓库、包裹、兑换和制作提示。
- 杂货商 Candle、Dungeonescape、RandomTeleport、RepairOil、TownTeleport 的说明文本。
- Denzel 传送员、委托商人、Wayne 特殊修理、矿石/石头商人的可见按钮和说明。

处理原则：

- 保留 `@BuySell`、`@BuyBack`、`@Storage`、`@mbind`、`@tele`、`@Market`、`@Craft`、`@SRepair`、`@next2` 等脚本入口。
- 保留 `MOVE`、`CHECKITEM`、`TAKEITEM`、`GIVEGOLD`、`CHECKGOLD`、`TAKEGOLD` 等命令与坐标。
- 保留 `[Trade]`、`[Types]`、`[Quests]`、`[RECIPE]` 数据段及其英文物品/配方名。
- 保留 `Ancient Post Piece`、`GoldBar`、`GoldBarBundle`、`GoldChest`、`TownTeleport`、`RepairOil`、`Dungeonescape`、`RandomTeleport`、`Bichon Province`、`Mongchon Province`、`Tao Village`、`Prajna Island`、`Past Bichon` 等内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `I open`、`closed door`、`cursed land`、`I will not help`、`Close/@`、`Hello`、`Welcome`、`View/@`、`Repair/@`、`Exchange/@`、`Purchase/@`、`Back/@`、`What kind of Poison`、`service fee`、`Commission Merchant`、`Help/@`、`Special repairs`、`Candles are`、`Dungeonescape scroll`、`Randomteleport scroll`、`durabillity`、`TownTeleport scrolls`、`Just pay`、字面 `\r\n` 等旧英文/格式残留，未再命中。

## 第三十二批 DogYoHyun 仓库与 GuildTerritory 基础服务 NPC

更新时间：2026-06-04

本批继续处理模板化基础服务 NPC，优先覆盖高重复、低风险的仓库、杂货与公告板显示文本：

- `Build/Server/Debug/Envir/NPCs/DogYoHyun/Village/HongSul.txt`
- `Build/Server/Debug/Envir/NPCs/GuildTerritory/GA0` 至 `GA9` 的 `GTStore-GA*.txt`
- `Build/Server/Debug/Envir/NPCs/GuildTerritory/GA0` 至 `GA9` 的 `GTPeddler-GA*.txt`
- `Build/Server/Debug/Envir/NPCs/GuildTerritory/GA0` 至 `GA9` 的 `GTBulletinBoard-GA*.txt`

覆盖内容：

- DogYoHyun 仓库 NPC 的 PK 拒绝、问候、仓库入口与存取提示。
- GuildTerritory 各分区仓库 NPC 的仓库、包裹发送/领取和存取提示。
- GuildTerritory 各分区杂货 NPC 的买卖、回购、修理、物品询问入口，以及 Candle、DungeonEscape、RandomTeleport、RepairOil、TownTeleport 说明。
- GuildTerritory 各分区公告板的问候、公告板查看与跳过按钮。

处理原则：

- 保留 `[Types]`、`[Trade]` 数据段及其物品英文内部名。
- 保留 `@Storage`、`@SendParcel`、`@CollectParcel`、`@BuySell`、`@BuyBack`、`@Repair`、`@Ask`、`@TOWNBOARDLIST` 等脚本入口。
- 保留 `Candle`、`DungeonEscape`、`RandomTeleport`、`RepairOil`、`TownTeleport`、`GT` 类物品名等引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 31 个文件扫描 `I will not help`、`Close/@`、`Hello`、`How can`、`how may`、`Back/@`、`View/@`、`Repair/@`、`Access/@`、`What item`、`Buy Back`、`RepairOil makes`、`TownTeleport scrolls`、`Bulletin Board` 等旧英文显示文本，未发现需要继续翻译的残留；命中项仅为 `@BuyBack`、`@SendParcel`、`@CollectParcel` 等保留脚本标签。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第四十一批 WoomyonWoods/TaoistVillage BigTaoist 长剧情 NPC

更新时间：2026-06-04

本批处理 TaoistVillage 中 BigTaoist / 编号镜像的 RedMoonEvil 长剧情对白：

- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/BigTaoist.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/9Wbt.txt`

覆盖内容：

- BigTaoist/Abel 的初始问候、命运请求和任务承接按钮。
- Abel 早年武艺、JadeCrystal、Thunderman 身份和前往 RedMoon Valley 的叙述。
- RedMoonEvil、HolySword、RedMoonSword、RedEvilApe、RedMoonChip 相关完整剧情和任务请求。

处理原则：

- 保留 `JadeCrystal`、`Abel`、`Thunderman`、`RedMoon Valley`、`RedMoonEvil`、`HolySword`、`RedMoonSword`、`RedEvilApe`、`RedMoonChip` 等名称引用。
- 保留 `CHECK [530]`、`CHECKQUEST 146 1`、`CHECK [531]`、`SET [530]`、`SET [531]`、`BREAK`、`CLOSE` 等脚本命令和状态位。
- 保留 `@Next1`、`@CHECK2`、`@Next2`、`@Quest1` 至 `@Quest8`、`@Next3` 等脚本入口。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 2 个文件扫描 `Welcome`、`What can`、`Close/@`、`Come here`、`waiting`、`destiny`、`orders`、`pleasure`、`Well. This`、`leaving`、`nickname`、`striven`、`martial`、`arrogant`、`community`、`Next/@`、`stronger monsters`、`highest floor`、`ugly`、`bad-smelling`、`hatred`、`heartbeasts`、`frightened`、`Your right`、`taoist school`、`bad reputation`、`expelled`、`extorted`、`wrong step`、`enemy heard`、`kill with anger`、`Consequently`、`murdered`、`corpses`、`anger against`、`loathing`、`minuet`、`showered`、`demon energy`、`growning`、`everything was my fault`、`how can I`、`Although`、`passage of time`、`wound Therefore`、`human community`、`Give me an order`、`Since I became`、`Murder. Fighting`、`faught`、`aquired`、`body Died`、`would like`、`If left alone` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第四十批 WoomyonWoods/TaoistVillage Perry 长剧情 NPC

更新时间：2026-06-04

本批处理 TaoistVillage 中 Perry / 编号镜像的 HolySword 与 Abel 长剧情对白：

- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Perry.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/6Wrt.txt`

覆盖内容：

- HolySword 初始询问、危险地点提醒和 RootSpider 击杀后的追问入口。
- 偷剑恶徒、HolySword 来历、Redmoon Valley、JadeCrystal / JadeStick 线索说明。
- Perry 关于 Abel 的长篇回忆、追捕、决斗、秘传技艺失踪和 Tree path 洞穴传闻。

处理原则：

- 保留 `HolySword`、`Holy Sword`、`RootSpider`、`TreePath`、`Tree path`、`Redmoon Valley`、`Redmoon valley`、`JadeCrystal`、`JadeStick`、`Abel`、`Bichon Province`、`Woomyon Woods` 等名称引用。
- 保留 `CHECK [527]`、`CHECK [528]`、`CHECK [529]`、`CHECK [1144]`、`SET [528]`、`SET [529]`、`CLOSE`、`BREAK` 等脚本命令和状态位。
- 保留 `@Quest`、`@quest1`、`@quest2`、`@next_quest*`、`@hear`、`@nohear`、`@pre_hear`、`@next*` 等脚本入口。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 2 个文件扫描 `You seem`、`Ok.`、`Close/@`、`What do`、`don't`、`People say`、`villain`、`Where is`、`dangerous`、`curiosity`、`responsibility`、`Good job`、`goign`、`Please kill`、`No information`、`former`、`pitiful`、`serious crime`、`betrayed`、`chronicle`、`specification`、`cayses`、`hot air`、`According`、`perfroming`、`Possible`、`Keep this`、`Rev.Taoist`、`young hero`、`blood boil`、`Do you want`、`Hear/@`、`Skip/@`、`Get out`、`Ask/@`、`Listen/@`、`curious`、`happend`、`approximately`、`martial`、`secret know`、`underestimated`、`Finaly`、`Fourteen`、`great man`、`persuading`、`dissapeared`、`whereabouts`、`permanent riddle`、`story is over`、`Next/@` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第三十九批 WoomyonWoods/TaoistVillage 短任务与提示 NPC

更新时间：2026-06-04

本批继续处理 `WoomyonWoods/TaoistVillage` 中短任务、导师、收集、毒药和仓库镜像文件：

- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/TrainerTao.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/9Wtt.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/MasterMK.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/LeadTrainerTV.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Dealer.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/9Enc5.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/DeclarationTV.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/InvestigatorTV.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Potion1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/4Wdm.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/6Wwh.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Apprentice*.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/9Wae*.txt`

覆盖内容：

- 道士导师关于 HolyWeapon / Holysword 的短对话和选择按钮。
- 怪物抱怨、教学改进、Mirian 新闻、村庄异常调查等短提示。
- 材料收集商的收购说明、出售入口和材料展示提示。
- 毒药商的购买入口与毒药选择提示。
- 仓库镜像的仓库、包裹与存取提示。
- 学徒关于 HolySword、Tree Path、Rootspider 的短传闻/提示。

处理原则：

- 保留 `HolySword`、`HolyWeapon`、`Holysword`、`Tree Path`、`Rootspider`、`Mirian` 等名称引用。
- 保留 `[RECIPE]`、`[Types]`、`[Quests]` 数据段及其英文配方/物品内部名。
- 保留 `@next`、`@deney`、`@Sell`、`@Buy`、`@Storage`、`@SendParcel`、`@CollectParcel` 等脚本入口。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 26 个文件扫描 `I will not help`、`Close/@`、`Hello`、`Welcome`、`What kind`、`What Posion`、`What item`、`Please`、`Okay/@`、`Thanks/@`、`Ok/@`、`Back/@`、`Access/@`、`Purchase/@`、`Sell/@`、`HolyWeapon... I`、`Holysword?`、`apprentice's words`、`good attitude`、`Defenitly`、`Soo many`、`Monsters..`、`lastest`、`dosent`、`Tree Path are`、`villian stole`、`worst monster amongst`、`rushs`、`larves`、`Material's`、`I can sell`、`Show me the Material` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第三十八批 WoomyonWoods/TaoistVillage 基础服务 NPC

更新时间：2026-06-04

本批开始处理 `WoomyonWoods/TaoistVillage` 中模板化基础服务 NPC：

- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Weaponsmith.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/2Wwe.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Drapery.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/3Wdr.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Potion.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/4Wdu.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Peddlar.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Jewellery.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/8Wac.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/7Wst.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Transporter.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Woomyon_Transporter1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Signpost.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Woomyon_Signpos.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Books.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/5Wbo.txt`

覆盖内容：

- 武器、衣物、药水、杂货、首饰、仓库、包裹、传送员、路牌和书店基础交互。
- 杂货商的 Candle、Dungeonescape、RandomTeleport、RepairOil、TownTeleport 说明。
- 书店的技能书买卖入口、技能说明入口、职业分类和技能等级列表按钮。

处理原则：

- 保留 `[Types]`、`[Trade]`、`[Quests]` 数据段及其英文物品/技能书内部名。
- 保留 `@BuySell`、`@BuyBack`、`@Repair`、`@Storage`、`@SendParcel`、`@CollectParcel`、`@tele`、`@helpbooks`、`@War*`、`@Wiz*`、`@Tao*`、`@Assa*`、`@Arc*` 等脚本入口。
- 保留 `Candle`、`Dungeonescape`、`RandomTeleport`、`RepairOil`、`TownTeleport`、`Sigmund`、技能名、物品名和 `MOVE` 目标。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 17 个文件扫描 `I will not help`、`Close/@`、`Hello`、`Welcome`、`How can`、`How may`、`View/@`、`Repair/@`、`Access/@`、`Buy Back`、`Back/@`、`Exit/@`、`Use.`、`Shop moving`、`Weapon shop`、`Armour shop`、`Which item`、`What Item`、`Would you`、`purchase back`、`Please select`、`What kind of Books`、`Skill List`、`Level`、`Service/@`、`Time/@`、`Which place`、`service fee`、`Candles are`、`Dungeonescape scroll`、`Randomteleport scroll`、`durabillity`、`carn`、`TownTeleport scrolls`、`More/@` 等旧英文显示文本，未发现需要继续翻译的残留；命中项仅剩技能名 `Reincarnation`，按技能/书名引用保留。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第三十七批 MudWall 残留显示文本收口

更新时间：2026-06-04

本批收口前几批扫描后留下的少量 MudWall 可见英文尾巴：

- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Grocery1.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Grocery1-3.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/CollectorMW.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Butcher.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Butcher-3.txt`

覆盖内容：

- 杂货镜像 NPC 的特殊问候、买卖说明和商店入口按钮。
- 收集商的出售询问、出售入口和交易提示。
- 肉铺的出售肉类按钮。

处理原则：

- 保留 `@BuySell`、`@Ask`、`@Sell`、`[TYPES]`、`[Quests]` 等脚本入口和数据段。
- 保留 `Candle`、`Dungeonescape`、`RandomTeleport`、`RepairOil`、`Townteleport` 等名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对整个 `MongchonProvince/MudWall` 目录扫描 `Ah are`、`I think`、`I can sell`、`forgot`、`View /@`、`Do you have`、`What do you have`、`Hello`、`Welcome`、`How can`、`How may`、`What can`、`Please`、`Would you`、`Buy Back`、`Back/@`、`Close/@`、`Exit/@`、`View/@`、`Repair/@`、`Access/@`、`Purchase/@`、`teleport to village`、`I will not help`、`Which item`、`What Item`、`What Helmet`、`repurchase`、`Commission`、`guild territory`、`play a game`、`Dark Swamp` 等旧英文显示文本，未发现需要继续翻译的残留；命中项仅为 `@Consignment` 等保留脚本标签。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第三十六批 MudWall 行会、委托、彩票与沼泽传送 NPC

更新时间：2026-06-04

本批继续处理 `MudWall` 周边剩余的中等长度功能 NPC：

- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/13Mgtm.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/GT.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/TrustMerchant-3.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/9Mlo-3.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Lottery.txt`
- `Build/Server/Debug/Envir/NPCs/MonghconProvince/Swamp/Transporter1.txt`

覆盖内容：

- 行会领地商人的领地说明、租赁、续租费用、进入领地、列表查看和交易说明。
- 委托商人的市场入口、寄售入口、帮助说明、手续费、保证金、期限、数量限制和寄售提示。
- 猜数字小游戏的 PK 拒绝、玩法说明、胜负提示、金币不足提示和重玩按钮。
- 黑暗沼泽传送员的危险提示、确认/取消和准备状态按钮。

处理原则：

- 保留 `@info`、`@rent`、`@agitreg`、`@agitmove`、`@agitbuy`、`@agittrade`、`@Market`、`@Consign`、`@Consignment`、`@Guess()`、`@WIN`、`@LOSE`、`@transport` 等脚本入口。
- 保留 `Bichon Province`、`Mongchon Province`、`Tao Village`、`Prajna Island`、`Past Bichon`、`GT Steward`、`Gold`、`MOVE EBEEBOSS` 等名称/变量/地图引用。
- 保留 `RANDOM`、`MOV`、`CHECKCALC`、`GIVEGOLD`、`TAKEGOLD`、`CHECKPKPOINT` 等脚本命令。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 7 个文件扫描 `How are`、`Guild Territory Merchant`、`guild territory`、`Commission Merchant`、`commission`、`View Market`、`View Consignments`、`Help/@`、`Leave/@`、`Consignment fee`、`Trust money`、`What would`、`PK POINTS`、`didnt`、`play a game`、`guess`、`Play again`、`Congratulations`、`Unfortunately`、`funds to gamble`、`Dark Swamp`、`Please`、`I'm ready`、`berfore` 等旧英文显示文本，未发现需要继续翻译的残留。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第三十五批 MongchonProvince/MudWall 基础服务 NPC

更新时间：2026-06-04

本批开始收口 `MongchonProvince/MudWall` 中模板化基础服务 NPC，优先处理商店、仓库、传送、告示牌和制作入口：

- 武器、服装、头盔、首饰、药水、杂货、肉铺、收集/委托短 NPC。
- 仓库、包裹、GoldBar/GoldBarBundle/GoldChest 兑换。
- 村庄商店传送告示牌与 Edwin 传送员。
- `Crafting.txt` / `11Mcft.txt` 制作女士与配方买卖入口。

覆盖文件共 39 个，包括：

- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Blacksmith*.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Clothes*.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Bracelet*.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Jewelers*.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Necklace.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Ring.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Helmets*.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Potion*.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Grocery*.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Warehouse*.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Board.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Signpost-3.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Transport.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Mongchon_Transport-3.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Crafting.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/11Mcft.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/CollectorMW.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MonDelegate*.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Butcher*.txt`

处理原则：

- 保留 `[Types]`、`[Trade]`、`[RECIPE]`、`[Quests]` 数据段及其英文物品/配方内部名。
- 保留 `@BuySell`、`@BuyBack`、`@Repair`、`@Storage`、`@SendParcel`、`@CollectParcel`、`@mbind`、`@tele`、`@Craft` 等脚本入口。
- 保留 `MOVE` 目标、`GoldBar`、`GoldBarBundle`、`GoldChest`、`Candle`、`Dungeonescape`、`RandomTeleport`、`RepairOil`、`TownTeleport`、`Jessica`、`Edwin` 等内部名或人名引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 39 个文件扫描 `I will not help`、`Close/@`、`Hello`、`Welcome`、`How can`、`View/@`、`Repair/@`、`Access/@`、`Buy Back`、`Back/@`、`Exit/@`、`Use.`、`Which item`、`What Item`、`Would you`、`purchase back`、`Please`、`Recipe`、`Exchange:`、`Commission`、`Candles are`、`Dungeonescape scroll`、`Randomteleport scroll`、`durabillity`、`carn`、`TownTeleport scrolls`、`Service/@`、`Which place`、`service fee`、`Weapon shop`、`Armour shop`、`Purchase/@` 等旧英文显示文本，未发现需要继续翻译的残留；命中项仅为 `@BuyBack`、`@SendParcel`、`@CollectParcel` 等保留脚本标签。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第三十四批 GuildTerritory 宠物 NPC

更新时间：2026-06-04

本批处理 GuildTerritory 中剩余的大型宠物服务模板：

- `Build/Server/Debug/Envir/NPCs/GuildTerritory/GA0` 至 `GA7` 的 `GTPetNPC.txt`

覆盖内容：

- 法师宠物 NPC 的主页问候、购买 5 只普通宠物、购买 5 只 7 级宠物、购买 1 只精英宠物、购买 1 只 7 级精英宠物入口。
- 普通/精英宠物购买列表、等级要求、价格表、等级或金币不足提示。
- 宠物存放说明、普通/精英宠物存放入口、存放条件不满足提示。
- 宠物取回说明、普通/精英宠物取回入口、凭证缺失提示。

处理原则：

- 保留 `CHECKCLASS Wizard`、`PETCOUNT`、`CHECKPET`、`PETLEVEL`、`CLEARPETS`、`GIVEPET`、`GIVEITEM`、`CHECKITEM`、`TAKEITEM` 等脚本命令。
- 保留 `RedViper`、`WickedTong`、`WildBeast`、`BloodRat`、`SeaCreature`、`ClawBeast`、`DemonSlasher`、`DemonFighter`、`FrozenBasher`、`CrystalCrawler`、`DragonWarrior`、`TwinHeadBeast`、`BlackTortoise`、`SnowLord`、`SeedingsClaw`、`SnowBlest` 及其 `Token` 物品内部名。
- 保留 `@buyp`、`@buysev`、`@buysp`、`@buyspsev`、`@storep`、`@retP`、`@Storepet*`、`@Storesolo*`、`@Retpet*`、`@Retsolo*` 等脚本入口。
- 只翻译动作、说明、等级提示、价格单位和按钮动词；宠物名称显示仍使用英文内部名，避免名称引用风险。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 8 个文件扫描 `Hello`、`Buy 5`、`Price List`、`Store Pets`、`Retrieve Pets`、`business here`、`activate pets`、`What pets`、`Level`、`right level`、`to poor`、`store certain`、`token to retrieve`、`same type`、`requirements to store`、`retreive`、`Normal Pets`、`Elite Pets`、`need to bring`、`Main/@Main` 等旧英文显示文本，未再命中。
- 针对整个 `GuildTerritory` 目录扫描 `Hello`、`Welcome`、`How are`、`How can`、`I want`、`Rental days`、`Teleport to`、`Players Online`、`Price List`、`Store Pets`、`Retrieve Pets`、`What pets`、`Close/@`、`Back/@`、`View/@`、`Repair/@`、`Access/@`、`Buy Back`、`Please`、`rental period`、`ownership`、`transaction` 等明显旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第三十三批 GuildTerritory 管家与传送员 NPC

更新时间：2026-06-04

本批继续收口 GuildTerritory 中十个分区的管理和传送模板：

- `Build/Server/Debug/Envir/NPCs/GuildTerritory/GA0` 至 `GA9` 的 `GTAdmin-GA*.txt`
- `Build/Server/Debug/Envir/NPCs/GuildTerritory/GA0` 至 `GA9` 的 `GTTeleporter-GA*.txt`

覆盖内容：

- 行会领地管家的问候、剩余租赁天数、续租、全员/单人召唤、出售/取消出售说明。
- 续租费用、租期到期后的宽限期、所有权移除提示。
- `LocalMessage` / `LOCALMESSAGE` 中的领地权限与续租成功提示。
- 行会领地传送员的在线人数、PK 点、下次沙巴克攻城时间和各目的地按钮。

处理原则：

- 保留 `@extend`、`@summon`、`@appcancel`、`@agitextend`、`@agitrecall`、`@agitonerecall`、`@@summontest`、`@@agitforsale`、`@agitforsalecancel` 等脚本入口。
- 保留 `HASGT`、`EXTENDGT`、`GTALLRECALL`、`GTRECALL`、`MOVE`、`<$GUILDGTRENTALDAYSLEFT>`、`<$GUILDEXTENDFEE>`、`<$CONQUESTSCHEDULE(1)>` 等命令和变量。
- 仅翻译传送按钮的显示地名，不改 `MOVE` 目标地图编号或内部地图名。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 20 个文件扫描 `How are`、`How can`、`Rental days`、`I want`、`Total summon`、`Individual summon`、`rental period`、`ownership`、`Come back`、`Who do`、`transaction`、`Players Online`、`Next Sabuk`、`Teleport to` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第七批比奇/边境基础商店 NPC

更新时间：2026-06-03

本批继续扩大单次处理量，集中翻译比奇城与边境村基础商店 NPC 的可见文本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Blacksmith.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Blacksmith-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Blacksmith.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Blacksmith-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Blacksmith-0103.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Blacksmith1.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Potion-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Potion-0108.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Potion1.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Potion2.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Potion3.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/BPotion-0109.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Grocery.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Grocery-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Grocery.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Grocery-0.txt`

覆盖内容：
- 武器店买卖、回购、普通修理、特殊修理说明。
- 药店与毒药商的购买入口、回购入口和拒绝高 PK 玩家提示。
- 杂货店买卖、回购、蜡烛、随机传送、地牢逃脱、修理油、回城卷轴说明。
- 杂货店中与皇帝传闻相关的任务对白。

处理原则：
- 保留 `[Trade]` 内物品名、`[Types]`、`[Quests]`、任务编号和 `CHECK` / `CHECKQUEST` / `SET` 等脚本命令。
- 保留 `<Candle/candle>`、`<Dungeonescape/dungeonescape>`、`<RandomTeleport/randomteleport>`、`<RepairOil/repairoil>`、`<Townteleport/townteleport>` 的跳转目标和物品名。
- 只翻译 `#SAY`、`#ELSESAY` 正文与按钮显示文本。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Hello traveller`、`Welcome`、`What Item`、`Which item`、`Buy Back`、`Back/@`、`Close/@`、`These are the items`、`Would you like`、`Repair Weapon`、`about Item`、`Candles`、`Have you ever` 等旧英文显示文本。
- 保留命中的 `Candle`、`Dungeonescape`、`RandomTeleport`、`RepairOil`、`TownTeleport` 属于物品名或跳转引用。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十三批比奇城特殊入口与纪念 NPC

更新时间：2026-06-03

本批翻译比奇城剩余的小型特殊入口与纪念 NPC：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/14Wr-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/IcemanStatue.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/OldmanInfinity.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/OldmanInfinity-0101.txt`

覆盖内容：
- Premium 通行证检查、Premium 地下城入口说明和快速入口按钮。
- Iceman 纪念雕像文本。
- 无限斗场介绍、费用说明、注意事项和挑战入口。

处理原则：
- 保留 `CHECKITEM`、`MOVE`、`GIVEITEM`、`@AOC`、`@AWT`、`@AST`、`@AZT`、`@APC`、`@MOVE10101` 等脚本命令和跳转目标。
- 保留 `PremiumPass[...]`、`TownTeleport`、`Brown Chestnuts`、`Crystal`、`Eden Elite` 等物品名或专有名词。
- `IcemanStatue1.txt` 为空文件，本批未改动。
- `BookStore*.txt` 与 `MirGuide*.txt` 仍保留为后续大型专项。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Close/@`、`Premium Pass`、`Thank You`、`Premium Dungeons`、`Dungeon 1`、`Quick Entry`、`Ancient Oma`、`In Memory`、`Rest In Peace`、`Read/@`、`pillar`、`passed away`、`Hey`、`Infinite Bout`、`What is the`、`I Will`、`Maybe next`、`bear in mind`、`hear it again`、`Bring it on`、`Okay/@` 等旧英文显示文本；剩余命中仅为脚本注释、物品名或专有名词。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十四批比奇城书店技能书 NPC

更新时间：2026-06-03

本批翻译比奇城书店技能书 NPC 的可见文本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/BookStore.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/BookStore-0104.txt`

覆盖内容：
- 书店买卖、回购、技能说明入口。
- 职业分类按钮：战士、法师、道士、刺客、弓箭手。
- 技能书列表中的 `Level` 显示改为 `等级`，`More` / `Back` 按钮改为中文显示。
- 与古代语言任务相关的书店信息提示。

处理原则：
- 保留 `[Trade]` 内技能书/技能名。
- 保留技能英文名，如 `Fencing`、`FireBall`、`Healing`、`FatalSword`、`Focus` 等，避免影响技能名、书籍名和数据库引用。
- 保留 `@War1`、`@Wiz1`、`@Tao1`、`@Assa1`、`@Arc1`、`@helpbooks` 等跳转目标。
- `MirGuide*.txt` 仍保留为后续大型指南专项。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Close/@`、`Welcome`、`View/@BuySell`、`Store.`、`Listen to`、`Seeking`、`Please select`、`Buy Back`、`Back/@`、`What kind`、`Warrior:`、`Wizard:`、`Taoist:`、`Assassin:`、`Archer:`、`Skill List`、`More/@`、`Level`、`Think ive`、`asking about`、`Travel to`、`Thank You` 等旧英文显示文本，未再命中；剩余英文为技能名、脚本标签或 `[Trade]` 内容。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十五批 MirGuide 狩猎与基础采集指南

更新时间：2026-06-03

本批翻译各地 MirGuide 的主菜单、狩猎等级指南与基础采集说明：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide-0.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide-3.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuidePI.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide-2.txt`

覆盖内容：
- MirGuide 主入口问候、PK 拒绝提示和菜单按钮。
- 狩猎等级段：1~11、11~21、21~30、31~40、41+。
- 新手割肉、采矿、药材和怪物尸体采集说明。
- `Back`、`Go to Main`、`Go to Next` 等导航按钮显示文本。

处理原则：
- 保留 `@hunting`、`@level1-11`、`@lvl1-3`、`@slicemeat`、`@combat` 等跳转目标。
- 保留地图、怪物、物品和技能英文名，如 `Bichon-Province`、`HookingCat`、`Candle`、`Portal Scroll`、`Thunderbolt` 等，等待名称引用专项统一处理。
- 本批仅处理狩猎与基础采集相关文本；`Combat & Skill` 技能说明正文仍作为后续大型专项处理。
- 统一将本批 MirGuide 文件写为 UTF-8 with BOM，避免 Windows 默认编码读取中文时出现乱码。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Welcome to the Mir world`、`It is wise`、`Start hunting`、`If you wish for a safe hunt`、`Go to dungeon from Level 11`、`If you wish for higher exp`、`You can learn a special skill`、`Slice meat:`、`Medicinal stuff:`、`Once you achieved`、`Back/@`、`Go to Main/@`、`Go to Next/@` 等旧英文显示文本，未再命中；剩余英文主要为技能说明专项正文、地图/怪物/物品/技能名和脚本标签。
- 本批 8 个 `MirGuide*.txt` 文件均已确认文件头为 `EF BB BF`。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十六批 MirGuide 战斗技能目录与战法技能说明

更新时间：2026-06-03

本批继续翻译 8 个 `MirGuide*.txt` 的战斗与技能内容：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide-0.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide-3.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuidePI.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide-2.txt`

覆盖内容：
- `Combat & Skill` 入口说明、技能学习方法、下一页说明。
- 战士、法师、道士、刺客、弓箭手技能目录标题和职业说明。
- 战士技能说明：`Fencing`、`Slaying`、`Thrusting`、`HalfMoon`、`ShoulderDash`、`TwinDrakeBlade`、`Entrapment`、`FlamingSword`、`LionsRoar`、`CrossHalfMoon`、`BladeAvalanche`、`Rage`、`ProtectionField`。
- 法师技能说明：`Fireball`、`Repulsion`、`ElecShock`、`GreatFireBall`、`Hellfire`、`Thunderbolt`、`Teleport`、`FireBang`、`FireWall`、`Lightning`、`FrostCrunch`、`Thunderstorm`、`MagicShield`、`TurnUndead`、`Vampirism`、`IceStorm`、`FlameDisruptor`、`Mirroring`、`FlameField`、`Blizzard`、`MeteorStrike`。
- 技能标题中的 `(Level n)` 统一改为 `(等级 n)`。

处理原则：
- 保留 `@skilllearning`、`@warrskill`、`@wizskill`、`@taoskill`、`@sinskill`、`@arcskill` 以及所有技能跳转目标。
- 保留技能名英文显示，避免提前影响技能书、技能名与数据库引用。
- 道士、刺客、弓箭手各技能的详细效果说明仍留作后续批次。

验证：
- 已扫描本批目标文件中的 `Skill learning`、`Warrior skill type`、`Wizard skill page`、`Taoist skill page`、`Assassin skill page`、`Archer skill page`、`Combat Technic`、`Under Development`、`(Level n)`、战士技能旧英文说明、法师技能旧英文说明等，未再命中；剩余英文主要为技能名、后续职业技能详细说明和脚本标签。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十七批 MirGuide 道士与弓箭手技能说明

更新时间：2026-06-03

本批继续翻译 `MirGuide*.txt` 的剩余技能详细说明：

- 8 个 `MirGuide*.txt` 中的道士技能说明。
- 比奇城两个长版 `MirGuide*.txt` 中的弓箭手技能说明。

覆盖内容：
- 道士技能说明：`Healing`、`SpiritSword`、`Poisoning`、`SoulFireBall`、`SummonSkeleton`、`Hiding`、`MassHiding`、`SoulShield`、`Revelation`、`BlessedArmour`、`EnergyRepulsor`、`TrapHexagon`、`Purification`、`MassHealing`、`Hallucination`、`UltimateEnhancer`、`SummonShinsu`、`SummonHolyDeva`、`Curse`、`Reincarnation`、`PoisonCloud`。
- 弓箭手技能说明：`Focus`、`StraightShot`、`MentalState`、`DoubleShot`、`Meditation`、`ElementalShot`、`ExplosiveTrap`、`Concentration`、`VamnpireShot`、`SummonVampire`、`BackStep`、`DelayedExplosion`、`ElementalBarrier`、`BindingShot`、`SummonToad`、`PoisonShot`、`CrippleShot`、`SummonSnakes`、`NapalmShot`、`OneWithNature`、`StoneTrap`。

处理原则：
- 保留技能名英文显示，以及 `@healing`、`@poison`、`@arcskill` 等脚本跳转目标。
- 保留物品/增益/属性缩写英文，如 `Poison`、`amulet`、`Amulet of Revival`、`Grey Poison`、`Vampire`、`PoisonShot`、`CrippleShot`、`OneWithNature`、`HP`、`MP`、`DC`、`MC`、`SC` 等，等待名称引用专项统一处理。
- 刺客菜单项目前仅有技能列表，未发现对应 `@fatalsword`、`@doubleslash` 等详细页，本批无可翻译刺客详情。
- `Basic` 基础信息区仍保留为下一批处理。

验证：
- 已扫描本批目标文件中的道士技能旧英文说明、弓箭手技能旧英文说明、`TO BE ADDED`、`待补充..` 等，未再命中；剩余英文主要为基础信息区、技能名、物品/增益名和脚本标签。
- 本批 8 个 `MirGuide*.txt` 文件均已确认文件头为 `EF BB BF`。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十八批 MirGuide 基础信息区

更新时间：2026-06-03

本批收尾翻译 8 个 `MirGuide*.txt` 的 `Basic` 基础信息区：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide-0.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide-3.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuidePI.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide-2.txt`

覆盖内容：
- 基础信息入口菜单：基础操作、商店与 NPC、采集、生产、查看玩家信息、命令。
- 基础操作说明：拾取、负重/背包限制、拾取优先权、丢弃物品。
- 商店/NPC 分类：物品商店、特殊商店、特殊建筑、特殊 NPC。
- 生产/制作占位说明、查看其他玩家信息说明。
- 聊天、队伍、行会、喊话、屏蔽聊天、退出行会等命令显示文字。

处理原则：
- 保留 `@basiccontrol`、`@shop`、`@slicemeat`、`@craft`、`@checkother`、`@command`、`@34678` 等脚本跳转目标。
- 保留 `/玩家名`、`!!`、`!~`、`!` 等命令符号示例。
- 短版中既有 `<Special shop/36487>` 未补加 `@`，仅翻译显示文字，避免改变脚本行为。
- `Deer`、`Meat`、`PickAxe` 等名称仍保留为后续名称引用专项处理。

验证：
- 已扫描本批目标文件中的 `These are the basic`、`Basic control`、`Item root`、`Item dispose`、`Item shop`、`Special shop`、`Special structure`、`Special NPC`、`Production (Craft`、`provided later`、`Check other user`、`Private chat`、`Group shout`、`Guild shout`、`Block guild`、`Ban private`、`Withdraw guild`、`Ban shout` 等旧英文显示文本，未再命中；剩余英文主要为技能名、物品/怪物名、脚本标签和 `[Quests]`。
- 本批 8 个 `MirGuide*.txt` 文件均已确认文件头为 `EF BB BF`。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十九批般若岛基础商店与仓库 NPC

更新时间：2026-06-03

本批翻译般若岛基础服务 NPC：

- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/WeaponSmith.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Clothes.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Jewellery.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Potion.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Potion1.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/CollectorPI.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/CraftLady.txt`

覆盖内容：
- 武器、防具、首饰商店的问候、当前装备提示、买卖/回购、修理说明。
- 仓库入口、包裹数量、发送/领取包裹、存取物品说明。
- 药水与毒药商店买卖入口。
- 收集商出售入口。
- 工匠女士介绍、制作入口、配方买卖入口。

处理原则：
- 保留 `@BuySell`、`@BuyBack`、`@Repair`、`@Storage`、`@SendParcel`、`@CollectParcel`、`@Craft` 等脚本跳转目标。
- 保留 `[Trade]`、`[RECIPE]`、`[TYPES]`、`[Quests]` 段内容中的物品名、配方名、类型编号和任务编号。
- `Peddlar.txt` 仍保留为后续卷轴说明专项处理。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Close/@`、`Hello`、`Welcome`、`What can I`、`How can`、`I see you're`、`View/@BuySell`、`Store.`、`Repair/@Repair`、`Which item`、`What Item`、`Buy Back`、`purchase back`、`repair a`、`Jewellery`、`Access/@Storage`、`parcels`、`Parcel`、`store or withdraw`、`Poison would`、`Purchase/@Buy`、`Posion`、`Traveler`、`anything for sale`、`Sell/@Sell`、`What do you have`、`craft lady`、`Please select the Recipe` 等旧英文显示文本；剩余命中仅为脚本标签如 `@BuyBack`、`@Storage`。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第二十批般若岛说明型 NPC

更新时间：2026-06-03

本批翻译般若岛说明型 NPC：

- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Peddlar.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/GTMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Inspector.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Inspector1.txt`

覆盖内容：
- 杂货商 `Peddlar` 的商店入口、回购入口、Candle / Dungeonescape / RandomTeleport / RepairOil / TownTeleport 说明。
- 行会领地商人的租赁、传送、列表、交易和行会领地概念说明。
- 检查员关于 `Bottomless Pit`、怪物调查、封印守护的对白。

处理原则：
- 保留 `@BuySell`、`@Ask`、`@candle`、`@dungeonescape`、`@randomteleport`、`@repairoil`、`@townteleport`、`@agitreg`、`@agitmove`、`@agitbuy`、`@agittrade`、`@ask1`、`@ask2`、`@next1`、`@next2` 等脚本跳转目标。
- 保留 `[Trade]` 物品名，以及 `Candle`、`Dungeonescape`、`RandomTeleport`、`RepairOil`、`TownTeleport`、`Bottomless Pit`、`Armed Transporter`、`GT Steward` 等名称引用，等待名称专项统一处理。
- `VillageChief*.txt` 的长篇传说对白仍留作下一批单独处理。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Close/@`、`Hello`、`How may`、`holding`、`View/@BuySell`、`Store.`、`Ask/@Ask`、`What Item`、`purchase back`、`I deal with`、`Candles are`、`Dungeonescape scroll`、`RepairOil makes`、`TownTeleport scrolls`、`How are you`、`Guild Territory Merchant`、`Back/@`、`Rent/@`、`Move/@`、`Trade/@`、`Hello I'm`、`cooperate`、`investigating`、`Next/@`、`dispatched from`、`protect the seal`、`I understand`、`nonsense` 等旧英文显示文本；剩余命中仅为保留名称或脚本标签。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第二十一批般若岛村长传说对白

更新时间：2026-06-03

本批翻译般若岛村长长篇传说对白：

- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/VillageChief.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/VillageChiefPI.txt`

覆盖内容：
- 村长开场介绍与询问入口。
- 当地传说：千年前战争、邪恶势力、英雄封印、迷宫神庙。
- 怪物回归相关对白。
- `Bottomless Pit` 的旧事、幸存者、驱邪与村中长者的不安。
- 下一页、关闭、移动到外面等按钮显示文字。

处理原则：
- 保留 `@ask1`、`@ask2`、`@ask3`、`@next1` 至 `@next8`、`@Tmove` 等跳转目标。
- 保留 `MOVE 5 125 324`、`MOVE 5 126 325` 坐标命令和 `[Quests]` 任务编号。
- 保留 `Bottomless Pit` 名称引用，等待名称专项统一处理。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Close/@`、`I'm already`、`local legend`、`about the monsters`、`move outside`、`According to`、`epic war`、`mighty heroes`、`sealed the path`、`built a temple`、`Unbelievable`、`monsters spotted`、`end of the world`、`Long ago`、`came back alive`、`famous shaman`、`70 years`、`close/@`、`Next/@`、`ask/@` 等旧英文显示文本；剩余命中仅为保留名称 `Bottomless Pit`。
- 已确认 `VillageChief.txt` 与 `VillageChiefPI.txt` 内容保持一致。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十二批比奇城公告与功能 NPC

更新时间：2026-06-03

本批翻译比奇城公告、功能入口与中型说明 NPC：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Board.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Signpost-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/CraftsLady.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/CraftsLady-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Hairdresser.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Lottery.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Lottery-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/GTMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/GTMerchant-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Luke.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Luke-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Far.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Far-0122.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/LukeStatue.txt`

覆盖内容：
- 公告牌/路牌商店传送、村庄消息、英雄功能和转生提示。
- 工匠女士配方买卖入口与介绍。
- 理发师服务、发型选择和金币不足提示。
- 抽奖小游戏规则、胜负提示和金币不足提示。
- 行会领地商人租赁、传送、列表和交易说明。
- Commander Luke / Emperor Far 古代语言任务对白。
- Luke Thompson 纪念雕像与募捐说明。

处理原则：
- 保留 `Move`、`ReviveHero`、`SealHero`、`SET`、`ADDNAMELIST`、`DELNAMELIST`、`RANDOM`、`MOV`、`CHECKCALC`、`BUYGT`、`TeleportGT`、`GIVEBUFF`、`LOCALMESSAGE` 等脚本命令。
- 保留 `[RECIPE]`、`[Trade]`、`[Quests]` 内容中的物品/配方名和任务编号。
- 保留 URL 链接地址，只翻译链接显示文字。
- `BookStore*.txt` 与 `MirGuide*.txt` 仍保留为后续大型专项。

验证：
- 已扫描本批目标文件中的 `Use.`、`teleport to`、`News:`、`Create Hero`、`Weapon shop`、`Rebirth starts`、`craft lady`、`Please select the Recipe`、`hairdresser`、`What haircut`、`You Dont`、`I will not speak`、`Your PK POINTS`、`Do you want`、`guess what number`、`Guild Territory Merchant`、`guild territory`、`HALT`、`loyal Commander`、`bribe`、`Mogu sent`、`Theres not much`、`Thank you`、`More to come`、`In Memory`、`Rest In Peace`、`people die by suicide` 等旧英文显示文本；剩余命中仅为脚本标签、命令或保留 URL。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十一批比奇城小型引导与任务对白 NPC

更新时间：2026-06-03

本批翻译比奇城小型引导、任务对白和功能入口 NPC：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Examiner.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Examiner-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Downgrade.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Downgrade-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Solider.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Solider-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MongchonScout.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MongchonScout-0100.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/SubjagationManager.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/SubjagationMngrBW.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Sir.Mogu.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Sir.MoguBW.txt`

覆盖内容：
- 比奇令牌审查官说明、购买入口和地狱入口传闻说明。
- 觉醒物品收购/分解入口。
- 英雄雕像说明、蒙古侦察酒类传闻对白。
- 讨伐管理器简短问候。
- Sir.Mogu 古代语言任务对白、酒馆购买入口和相关分支。

处理原则：
- 保留 `[Trade]` 物品名、`[Quests]`、任务编号、`CHECK` / `CHECKQUEST` / `SET`、`#INCLUDE` 等脚本命令。
- 保留 `@Buy`、`@Ask`、`@Sell`、`@Disassemble`、`@Lang`、`@Fool`、`@Before1`、`@before1` 等跳转目标。
- `Sir.Mogu*.txt` 中既存的 `[@before1>` 标签结构不调整，只翻译可见文本。
- 大型 `MirGuide*.txt` 暂未纳入本批，因为其包含大量技能、怪物和指南文本，后续单独处理。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Close/@`、`Back/@`、`Purchase Awakening`、`Examiner for Bichon`、`Bichon Token`、`Buy Bichon`、`empty lot`、`World Heroes`、`This alcohol`、`Rumours`、`Ancient Language`、`crazy old`、`Emperor Far` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第十批比奇城服务型商店 NPC

更新时间：2026-06-03

本批翻译比奇城低风险服务型商店与入口 NPC 的可见文本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/PetStore.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/PetStore-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/StableGirl.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/StableGirl-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/PearlStore.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/PearlStore-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/WiseFisher.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/WiseFisher-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Premium.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/TrustMerchant-0.txt`

覆盖内容：
- 宠物蛋商店与皇帝传闻任务对白。
- 坐骑用品商店。
- 宠物珍珠商店。
- 钓鱼用品买卖、鱼类出售和回购提示。
- 高级通行证地下城入口说明。
- 寄售商说明、寄售规则、市场/寄售入口与帮助文本。

处理原则：
- 保留 `[Trade]` 内物品名、`[Type]` / `[Types]`、`CHECKITEM`、`MOVE`、`GIVEITEM`、`SET` 等脚本命令。
- 保留 `@Buy`、`@PEARLBUY`、`@Market`、`@Consign`、`@Consignment`、`@AOC` 等跳转目标。
- 保留 `{Gold/Gold}`、`<$USERNAME>`、`<$MOUNT>`、`<$MOUNTLOYALTY>` 等占位符。
- 书店 `BookStore*.txt` 暂未纳入本批，因为技能书清单和技能/物品名引用更多，后续按专项处理。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Close/@`、`Close/@Exit`、`Close/exit`、`Back/@`、`Greetings`、`Creature Eggs`、`available Eggs`、`Please select`、`Ah your`、`Have you ever`、`Welcome traveller`、`View/@BuySell`、`Commission Merchant`、`commission sales`、`Bichon Province`、`View Market`、`cautions`、`Trust money`、`Premium Pass`、`Quick Entry`、`Fishing Stuff`、`Lay down`、`Item's you`、`Creature Pearls` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第九批比奇城装备与材料商店 NPC

更新时间：2026-06-03

本批翻译比奇城装备、材料和肉铺类 NPC 的可见文本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bracelet.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bracelet-0105.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Butcher-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Butcher-0102.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Butcher1.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Butcher2.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Clothing.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Clothing-0106.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Materials.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Materials-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Necklace.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Necklace-0105.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Ring.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Ring-0105.txt`

覆盖内容：
- 服装店买卖、回购、修理说明，以及 CastleBichon / 皇帝相关任务对白。
- 肉铺收购、回购、割肉帮助和 ALT 操作提示。
- 材料商收购说明。
- 戒指、手镯/手套、项链商店买卖、回购、修理说明。

处理原则：
- 保留 `[Trade]` 物品名、`[Types]`、`[Quests]`、任务编号、`CHECK` / `CHECKQUEST` / `SET` 等脚本命令。
- 保留 `@talk`、`@CHECK2`、`@MAIN1-*`、`@Emperor` 等任务跳转目标。
- 原脚本中的 `<Close@exit>` 结构保持不改，仅翻译显示文本为 `<关闭@exit>`。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Close/@`、`Close/@Exit`、`Close@exit`、`Back/@`、`Welcome`、`Hello Traveller`、`Hello traveler`、`View/@BuySell`、`Repair/@Repair`、`Talk/@talk`、`Buy Back`、`These are the items`、`Item's you`、`Which item`、`Which ring`、`Which necklace`、`Would you like`、`Meat.`、`about how`、`high quality`、`To get meat`、`Using magic`、`Show me the Material`、`Thankyou traveler`、`good deeds` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第八批边境村装备与材料商店 NPC

更新时间：2026-06-03

本批翻译边境村基础装备、材料和肉铺类 NPC 的可见文本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Butcher.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Butcher-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Drapery.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Drapery-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Materials.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Ring.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Ring-0141.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Bracelet.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Bracelet-0141.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Necklace.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Necklace-0141.txt`

覆盖内容：
- 肉铺收购说明、割肉帮助说明和 ALT 操作提示。
- 服装店买卖、回购、修理说明。
- 材料商收购说明。
- 戒指、手镯/手套、项链商店买卖、回购、修理说明。

处理原则：
- 保留 `[Trade]` 内物品名、`[Types]`、`[Quests]` 和任务编号。
- 保留按钮跳转目标如 `@Sell`、`@Meathelp`、`@BuySell`、`@BuyBack`、`@Repair`。
- 带颜色标记的 `{Meat/LightSteelBlue}`、`{Hens/Crimson}` 等改为中文显示名并保留颜色标记。

验证：
- 已扫描本批目标文件中的 `I will not help`、`Close/@`、`Back/@`、`Welcome`、`Hello Traveller`、`View/@BuySell`、`Repair/@Repair`、`Buy Back`、`These are the items`、`Which item`、`Which ring`、`Which necklace`、`Would you like`、`Meat.`、`about how`、`high quality`、`To get meat`、`Using magic`、`Show me the Material` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动；`Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。既存 `HiGreatGhoul` / `RedDagger Q` 掉落加载问题仍存在，非本批引入。

## 第六批比奇/边境传送与仓库 NPC

更新时间：2026-06-03

本批按“多一点但仍低风险”的原则，集中翻译比奇城与边境村的传送、仓库、包裹和新手公告类 NPC 可见文本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Transport.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Border_Transport-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport1.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport1-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport2.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport2-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport3.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport3-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Warehouse-0140.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Warehouse-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Warehouse1.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Warehouse2.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Warehouse-0125.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Jane.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/BountyBoard.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/BountyBoard-0.txt`

覆盖内容：
- 传送员拒绝 PK 过高玩家、传送服务说明、目的地按钮、金币不足提示。
- 仓库/包裹/金币物品兑换说明、返回/关闭按钮、包裹数量提示。
- 仓库信息入口中与古老语言相关的任务提示。
- 新手引导 Jane 与新手行会招募公告牌。

处理原则：
- 保留 `MOVE`、`CHECKGOLD`、`TAKEGOLD`、`CHECKITEM`、`TAKEITEM`、`GIVEGOLD`、`ADDTOGUILD`、`ADDNAMELIST` 等命令。
- 保留 `@Border`、`@BichonWall`、`@Storage`、`@GBar` 等跳转标签和功能入口。
- 保留 `GoldBar`、`GoldBarBundle`、`GoldChest`、`NewbieGuild` 等作为脚本参数使用的英文名，只翻译玩家可见说明。

验证：
- 本批目标文件已扫描 `I will not help`、`Hello traveller`、`Which place would`、`Current Bounty List`、`Newbie Guild Recruitment`、`What item do you want`、`Congratulations, You have joined` 等旧英文显示文本，未再命中目标文件。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示环境与网络已启动。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第四批入口与传送 NPC

更新时间：2026-06-03

已翻译一批入口、传送与镜像楼梯 NPC 的显示文本：

- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/CaveGuideTV.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/CaveGuideTV.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/Swamp/Transporter.txt`
- `Build/Server/Debug/Envir/NPCs/MonghconProvince/Swamp/Transporter.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/SubjugationLeadW-1.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Sailor-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Cloud-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/0Helper-0.txt`
- `Build/Server/Debug/Envir/NPCs/GuildTerritory/GA*/GTStairs-*.txt`

处理原则：

- 保留 `MOVE` 目标、金币检查、任务编号和跳转标签。
- 只翻译船夫对话、返回/确认按钮、楼梯显示文本、传送询问和新手提示。
- `CraftingPortal` 系列只有 `MOVE` 命令，无可见文本，本批跳过。

验证：

- 针对本批文件扫描 `Move/@`、`Close/@`、`to the 2nd/3rd floor`、`Cave researcher`、`Dont Have`、`Assassin-Level` 等旧英文模式。
- 行会楼梯 `GTStairs-*.txt` 已确认无英文按钮残留。
- 使用本地 `.tools/dotnet/dotnet.exe` 启动 `Build/Server/Debug/Server.dll`，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第五批功能说明 NPC

更新时间：2026-06-03

已翻译功能入口与说明类 NPC：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Awakening.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Awakening-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Administrator.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Administrator-0122.txt`

覆盖内容：

- 觉醒、分解、降级、重置功能说明与按钮显示。
- 比奇城管理员的行会创建、行会战、沙巴克攻城战说明。

处理原则：

- 保留 `@CREATEGUILD`、`@REQUESTWAR`、`@requestcastlewar`、`@requestcastlewarnow` 等功能入口。
- 保留 `{Gold/Gold}`、`<$GUILDWARFEE>`、`<$GUILDWARTIME>` 等占位和链接格式。
- 保留 `[~@request_ok]` 等特殊段名。
- 只翻译自然语言、说明文字和按钮显示文本。

验证：

- 针对本批文件扫描 `Request Guild`、`Guildchief`、`Wall conquest`、`Back/@`、`Exit/@`、`Close/@`、`Administrator`、`special characters`、`Sabuk conquest` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第二十二批般若岛编号镜像与时间石 NPC

更新时间：2026-06-03

本批收口 Prajna Island 已翻译 NPC 的编号镜像文件，并补齐委托商人、行会领地商人、奇怪老人与时间石传送相关文本：

- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/2Piwe.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/3Pidr.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/4Pidu.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/4Pidm.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/6Piwh.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/7Pist.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/8Piac.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/11Picft.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/12Pitm.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/13Pigtm.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/14CBi.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/OddOldMan.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Timestone.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Prajna_Timestone1.txt`

覆盖内容：

- 武器、服装、药水、仓库、杂货、首饰、制作女士等编号镜像 NPC 的商店、修理、回购、仓库和包裹按钮文本。
- 委托商人的市场入口、帮助说明、手续费/保证金/期限/数量提示。
- 行会领地商人的领地说明、租赁、传送、列表查看和交易说明。
- 奇怪老人的 TimeStonePiece 购买/出售提示，以及时间石传送 NPC 的穿越说明。
- 派驻 Bottomless Pit 入口的封印守卫说明。

处理原则：

- 保留 `@BuySell`、`@BuyBack`、`@Storage`、`@SendParcel`、`@Market`、`@agitreg`、`@agitmove`、`@agittrade`、`@tele` 等脚本入口。
- 保留 `TimeStonePiece`、`Gold`、`Bichon Province`、`Mongchon Province`、`Tao Village`、`Prajna Island`、`Past Bichon`、`Bottomless Pit` 等名称引用，等待名称专项联动处理。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `I will not help`、`Close/@`、`Hello`、`Buy Back`、`Guild territory`、`You need`、`Worship me`、`TimeStone.`、`timetravel`、`View Market`、`Help/@`、`Buy/@`、`Sell/@` 等旧英文显示文本，未再命中；剩余命中均为脚本标签或保留物品/功能名。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第四十三批 WoomyonWoods Fortress 基础服务与短任务 NPC

更新时间：2026-06-04

本批完成 WoomyonWoods/Fortress 目录的基础服务 NPC 与短任务入口：

- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/Blacksmith.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/2Wwe-1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/Drapery.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/WWDr-1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/Potion.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/4Wdu-1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/Butcher.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/1Wme-1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/WCollect-1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/Master.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/9Wqu-1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/SubjugationLead.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Fortress/SubjugationLeadW-1.txt`

覆盖内容：

- 铁匠、裁缝、药水商的 PK 拒绝、问候、装备提示、买卖、回购和修理提示。
- 屠夫/编号镜像的采肉说明、出售入口、品质说明和返回按钮。
- 收购商的出售入口与交易提示。
- Master/Shok 避难短句、城镇首领防御说明和相关短任务入口。

处理原则：

- 保留 `@BuySell`、`@BuyBack`、`@Repair`、`@Sell`、`@Meathelp`、`@Main-1` 等脚本入口。
- 保留 `[Trade]`、`[Types]`、`[TYPES]`、`[Quests]` 数据段和英文物品名。
- 保留 `Hens`、`Deer`、`Sheep`、`Wolves` 等任务/颜色引用结构。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `WoomyonWoods/Fortress` 扫描 `I will not help`、`Close/@`、`Hello`、`Welcome`、`How can`、`View/@`、`Repair/@`、`Buy Back`、`Back/@`、`Which item`、`Would you`、`purchase back`、`Meat can`、`I will buy`、`ALT button`、`seek refuge`、`Leader of this Town`、字面 `\r\n` 等旧英文/格式残留，未再命中。

## 第四十四批 WoomyonWoods SeokchoValley 与 Castle-GI 残留服务 NPC

更新时间：2026-06-04

本批处理 SeokchoValley 小目录，并收口 Castle-GI 中遗留的制作、特殊铁匠和石头商短文件：

- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/SeokchoValley/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/SeokchoValley/Merchant.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/SeokchoValley/Grocery.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/SeokchoValley/Collector.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/SeokchoValley/Signpost.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Crafting.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/11Gicft.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Blacksmith.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Stones.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/8GiStn.txt`

覆盖内容：

- SeokchoValley 仓库、包裹、商人、杂货、收购商的 PK 拒绝、问候、交易、修理、回购和仓库提示。
- Castle-GI 制作女士的制作说明与入口按钮。
- Wayne 特殊铁匠的问候、武器提示、特殊修理入口与材料说明。
- 石头商的收购说明、出售入口与出售提示。

处理原则：

- 保留 `@Storage`、`@SendParcel`、`@CollectParcel`、`@BuySell`、`@BuyBack`、`@Repair`、`@Sell`、`@Craft`、`@SRepair` 等脚本入口。
- 保留 `[Trade]`、`[Types]`、`[TYPES]`、`[RECIPE]`、`[QUESTS]` 数据段和英文物品/配方名。
- 保留 `Wayne`、`SunPotion`、`SewingGoods`、`TownTeleport`、`RandomTeleport`、`DungeonEscape` 等内部名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `I will not help`、`<Close/@`、`Hello traveller`、`Welcome, What`、`<View/@`、`<Repair/@`、`<Buy Back/@`、`<Back/@`、`Which item would`、`What Item would`、`These are the items`、`You can repair`、`Do you have anything`、`I am craft lady`、`Master Artisan`、`Special repairs`、`Traveller, I will purchase`、字面 `\r\n` 等旧英文/格式残留，未再命中。

## 第四十五批 WoomyonWoods 残留短脚本与特殊传送

更新时间：2026-06-04

本批收口 WoomyonWoods 中分散的短脚本残留：

- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/14Wso-D2042.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/WoomaTemple/Stone.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/SeokchoValley/TrollMine/Transporter.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Apprentice5.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/9Wae5.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Apprentice6.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/9Wae6.txt`

覆盖内容：

- 中毒者短句与关闭按钮。
- WoomaTemple 神秘石的无反应提示、等级/物品需求说明、放逐生物入口。
- TrollMine 4 层野兽传闻、确认/拒绝按钮、准备传送提示。
- TaoistVillage 学徒旧书涂写与腿痛休息短句。

处理原则：

- 保留 `CHECKQUEST`、`LEVEL`、`CHECKITEM`、`TAKEITEM`、`MOVE`、`BREAK`、`CLOSE` 等命令。
- 保留 `WoomaHeart`、`D022A`、`SG004`、任务号和 `@Woomaa`、`@transport` 等跳转入口。
- 保留颜色标记结构 `{.../LightSteelBlue}`、`{.../KHAI}`，只翻译可见说明。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `He seems`、`pisoned`、`<Close/@`、`Nothing happens`、`Mysterious Stone`、`Banished Creatures`、`Have you heard`、`rumours`、`beast`、`tails`、`Please let`、`ready`、`cursed mountain`、`vicious woods`、`pain in my legs`、`take a rest`、字面 `\r\n` 等旧英文/格式残留，未再命中。
- 针对整个 `WoomyonWoods` 目录复扫本阶段明确旧英文短句模式，未再命中。

## 第四十六批 AncientCaves 古代洞穴石碑脚本

更新时间：2026-06-04

本批处理 AncientCaves 目录的古代洞穴石碑脚本：

- `Build/Server/Debug/Envir/NPCs/AncientCaves/AncientWooma-D023.txt`
- `Build/Server/Debug/Envir/NPCs/AncientCaves/AncientZuma-D504.txt`
- `Build/Server/Debug/Envir/NPCs/AncientCaves/AncientStone-D715.txt`
- `Build/Server/Debug/Envir/NPCs/AncientCaves/AncientPrajna-D2074.txt`
- `Build/Server/Debug/Envir/NPCs/AncientCaves/AncientNatural-D003.txt`

覆盖内容：

- 古代石碑的无反应提示、古老/未知符号说明、等级需求与心脏物品需求。
- Wooma、Zuma、Stone、Prajna、Natural 古代洞穴入口文本。
- `AncientStone-D715.txt` 中 StoneTemple 发现提示与返回 MongchonDelegate 的 `LocalMessage`。

处理原则：

- 保留 `CHECKQUEST`、`LEVEL`、`CHECKITEM`、`TAKEITEM`、`MOVE`、`SET`、`GOTO`、`LocalMessage` 命令结构。
- 保留 `WoomaHeart`、`ZumaHeart`、`StoneHeart`、`PrajnaHeart`、`StoneTemple`、`MongchonDelegate`、地图编号和坐标。
- 保留 `@Woomaa`、`@zumatemplea`、`@stonetomba`、`@prajnacavea`、`@omacavea` 跳转入口。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `AncientCaves` 扫描 `Nothing happens`、`Mysterious Stone`、`Ancient symbols`、`Unknown symbols`、`Banished Creatures`、`Required {Level`、`The still`、`The noise`、`The Bones`、`Dungeon of the Ancient`、`You've found`、`Return to`、`<Close/@`、字面 `\r\n` 等旧英文/格式残留，未再命中。

## 第四十七批 MongchonProvince SabukWall 基础服务 NPC

更新时间：2026-06-04

本批处理 `MongchonProvince/SabukWall` 中除 `Conquest.txt` 外的基础服务 NPC 与编号镜像文件，共 27 个文件：

- 武器/防具/首饰/药水/杂货/仓库/屠夫等常规商人文件。
- `Blacksmith.txt`、`Blacksmith1.txt`、`2Are-0151.txt`、`9Aup-0151.txt` 特殊修理与武器精炼文件。
- `SwampNPC.txt` 沼泽入口守卫文件。

覆盖内容：

- PK 拒绝、问候、装备提示、商店、修理、回购、仓库、包裹、购买、毒药购买提示。
- 杂货商 Candle、Dungeonescape、RandomTeleport、RepairOil、TownTeleport 说明。
- 屠夫采肉与品质说明。
- Dean 特殊修理、武器精炼、精炼检查提示。
- 沼泽入口的贿赂、遗物出示、金币不足和进入提示。

处理原则：

- 保留 `Conquest.txt` 到后续专项处理。
- 保留 `[Trade]`、`[Types]`、`[Quests]` 数据段和英文物品名。
- 保留 `@BuySell`、`@BuyBack`、`@Repair`、`@Storage`、`@Buy`、`@Ask`、`@Refine`、`@RefineCollect`、`@RefineCheck`、`@next*` 等脚本入口。
- 保留 `BlackIronOre`、`SwampRelic`、`Gold`、`Candle`、`TownTeleport`、`RepairOil`、`EBEE01` 等内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 27 个目标文件扫描 `I will not help`、`<Close/@`、`Hello`、`Welcome`、`Which item`、`What Item`、`Buy Back`、`Back/@`、`These are the items`、`Would you like`、`Please Choose`、`I deal with`、`Candles are`、`Dungeonescape scroll`、`Randomteleport scroll`、`durabillity`、`TownTeleport scrolls`、`Meat can`、`ALT button`、`Master Artisan`、`Weapon Refine`、`small fee`、`guarrantee`、`Hello Mirian`、`Bribe/@`、`Show Relic`、`What kind of Poison`、`I see you're wearing`、字面 `\r\n` 等旧英文/格式残留，未再命中。

## 第四十八批 MongchonProvince SabukWall Conquest 面板

更新时间：2026-06-04

本批专项处理 `Build/Server/Debug/Envir/NPCs/MongchonProvince/SabukWall/Conquest.txt`。

覆盖内容：

- SabukWall 税收收入、NPC 税率、下次攻城日期、进攻行会、提取资金、税率设置入口。
- 城门开关、城墙修理、主城门/左侧城墙/右侧城墙/中央城墙修理成功与资金不足提示。
- 弓箭手一至十二的页面显示、重新雇佣按钮、重新雇佣成功与资金不足提示。
- `LocalMessage` 与 `GLOBALMESSAGE` 中玩家可见公告文本。

处理原则：

- 保留 `CONQUESTOWNER`、`CONQUESTGOLD`、`CONQUESTRATE`、`CONQUESTSCHEDULE`、`CONQUESTWALL`、`CONQUESTGUARD` 等变量。
- 保留 `CHECKPERMISSION`、`SETCONQUESTRATE`、`TAKECONQUESTGOLD`、`OPENGATE`、`CLOSEGATE`、`CONQUESTWALL`、`CONQUESTGUARD`、`AFFORDWALL`、`AFFORDGUARD` 等命令。
- 保留 `@withdrawal`、`@tax*`、`@repaircastle`、`@reliveachers`、`@ach*` 等脚本入口。
- 保留 `SabukWall`、`Sabuk`、`Gold` 等内部名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `Conquest.txt` 扫描 `Welcome`、`Tax Income`、`NPC Tax Rate`、`Withdraw fund`、`Sabuk Taxes`、`Next Conquest Date`、`Attacking Guild`、`Repair Castle`、`Rehire Archers`、`Exit/@`、`Come back`、`Move your Cash`、`Guild Bank`、`withdrawn`、`guild leader`、`Tax to`、`By the order`、`open and close`、`Main Gate`、`Opened`、`Closed`、`transfered`、`Repairs and Archers`、`Left Wall`、`Right Wall`、`Center Wall`、`Repaired`、`Dont have`、`Page Two`、`Page One`、`Bring your achers`、`Archer One` 至 `Archer Twelve`、`Rehire/@`、`rehired`、`dont have`、字面 `\r\n` 等旧英文/格式残留，未再命中。

## 第四十九批 BichonWall Civilians 居民对话

更新时间：2026-06-04

本批处理 `BichonProvince/BichonWall/Civilians` 的 10 个居民对话脚本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/1-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/2-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/3-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/4-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/5-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/6-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/7-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/8-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/9-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/10-0.txt`

覆盖内容：

- 居民关于 Emperor、流言、城市士气、饥荒、信任、帮忙传播善举等任务相关短对话。
- `@listen`、`@emperor`、`@Emperor` 相关按钮文本。
- 关闭按钮统一，并修正 `<Close/exit>`、`<Close@exit>` 等既有异常关闭按钮为 `<关闭/@exit>`。

处理原则：

- 保留 `CHECK`、`CHECKQUEST`、`SET`、`GOTO` 等任务逻辑。
- 保留 `[Quests]`、任务号、`@listen`、`@emperor`、`@Emperor` 跳转入口。
- 保留 `<$USERNAME>` 玩家名变量。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批居民脚本扫描 `Hello`、`Close/@`、`Close/`、`Close@`、`I am`、`I'm`、`Have you heard`、`rumors`、`Thankful`、`starving`、`How can`、`trust`、`Farewell`、`Sorry`、`startled`、`good word`、`traveler`、`Traveler`、`fine day`、`FEAST`、`Throne`、`Information`、`Thankyou`、`rubbish`、`Very well`、`morale`、`lazy`、`govern`、`Greeting`、`please visit`、字面 `` `r`n `` 和 `\r\n` 等旧英文/格式残留，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十批 BichonProvince 边境村、洞穴与短服务 NPC 大批次

更新时间：2026-06-04

本批扩大单次处理范围，合并处理 BichonProvince 中 18 个服务/短脚本文件：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/BookStore.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/BookStore-0132.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/CraftLady.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/CraftLady-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Pedlar.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Prison.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Prison-0127.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/OmaCave/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Warehouse-D002.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/OmaCave/Stone.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/OmaCave/Grim.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/NaturalCave/WickedTrader.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/WickedTrader-DM001.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/WickedTrader-DM011.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Wa-Master.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Wa-Master-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/SinCloud.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Sailor.txt`

覆盖内容：

- BorderVillage 书店问候、技能说明入口、职业分类、技能列表等级标签、买卖/回购按钮。
- CraftLady 制作说明、配方买卖提示。
- Pedlar 杂货商问候、买卖、回购、Candle、Dungeonescape、RandomTeleport、RepairOil、TownTeleport 说明。
- Prison 释放/禁止离开提示。
- OmaCave 仓库、神秘石、Grim 服装商与 WickedTrader 商店/修理模板。
- Wa-Master、SinCloud、Sailor 的短对话与基础按钮。

处理原则：

- 保留 `[Trade]`、`[RECIPE]`、`[Types]`、`[Quests]` 数据段和英文物品/技能/配方名。
- 保留 `@BuySell`、`@BuyBack`、`@helpbooks`、`@War*`、`@Wiz*`、`@Tao*`、`@Assa*`、`@Arc*`、`@Craft`、`@Storage`、`@omacavea` 等脚本入口。
- 保留 `Candle`、`TownTeleport`、`RepairOil`、`CastleBichon`、`D001A`、`PRISON` 等内部名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批 18 个目标文件扫描 `I will not help`、`<Close/@`、`Hello`、`Welcome`、`Please select`、`Book you`、`Buy Back`、`Back/@`、`What kind of Books`、`Warrior:`、`Skill List`、`Level [0-9]`、`I am craft lady`、`Recipe you`、`What Item`、`Which item`、`These are the items`、`Show me the items`、`Would you like`、`I deal with`、`Candles are`、`Dungeonescape scroll`、`Randomteleport scroll`、`durabillity`、`TownTeleport scrolls`、`Nothing happens`、`Prison Guard.*free`、`beloved house`、`Assassin-Level`、`Passengers`、字面 `` `r`n `` 和 `\r\n` 等旧英文/格式残留，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十一批 BichonProvince Event 通用活动脚本

更新时间：2026-06-04

本批继续扩大单批粒度，处理 `BichonProvince/Event` 下 6 个活动脚本的通用重复文本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event/MissDo-EM000.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event/MissMi-EM000.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event/MissRe-EM000.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event/Proceeder30-EM001.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event/Proceeder40-EM002.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event/Proceeder41-EM003.txt`

覆盖内容：

- MissDo/MissMi/MissRe 的竞技场介绍、召唤限制、入场费、时间限制、已有玩家战斗、等级过高、金币不足、返回与思考按钮。
- Proceeder 30/40/41 的已完成提示、欢迎挑战说明、准备开始按钮、未击败怪物提示、已击败并继续挑战提示、继续/离开按钮、最终胜利与奖励返回说明。

处理原则：

- 保留 `CHECKHUM`、`CHECKGOLD`、`TAKEGOLD`、`MONCLEAR`、`TIMERECALL`、`MOVE`、`MonGen`、`CHECKMON`、`SET`、`breaktimerecall` 等事件命令。
- 保留 `EM001`、`EM002`、`EM003` 地图/事件编号、怪物英文名和 `@start*`、`@check*`、`@say*`、`@finish` 等跳转入口。
- 只翻译重复可见文本，不改阶段逻辑和怪物生成表。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `BichonProvince/Event` 扫描 `You have already completed`、`Welcome.`、`I'm ready`、`You defeated them all already`、`You haven't beat`、`Before deafeating`、`Wow, you defeated`、`I'm so honored`、`true hero`、`I'll send`、`Hello, I am Miss`、`There is already someone fighting`、`try again later`、`Your level is`、`You dont have`、`Close/@`、`Okay.`、`Thank you.`、`Return to/@`、`Let me think`、`Pay 3000`、`Proceed.`、`I've had enough`、`prohibitied`、`Time Limit`、字面 `` `r`n `` 和 `\r\n` 等旧英文/格式残留，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十二批 MongchonProvince 玩家侧残留整区收口

更新时间：2026-06-04

本批进一步扩大处理范围，直接收口 `MongchonProvince` 玩家侧剩余残留文件，共 9 个：

- `Build/Server/Debug/Envir/NPCs/MongchonProvince/BugCave/Clothes.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/Clothes-D608.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/FoxCave/Rock.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/Rock-FOX02.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/StoneTemple/Stone.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/ZumaTemple/Stone.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/Swamp/Transporter1.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/Swamp/Bones.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/SwampBones.txt`

覆盖内容：

- BugCave 衣物修理问候、修理入口和返回按钮。
- FoxCave/Rock 组队队长、队伍范围、AdmissionOrb、允许/拒绝通过提示。
- StoneTemple/ZumaTemple 石碑、无反应提示、等级/心脏需求和 `LocalMessage`。
- Swamp 传送员的 Dark Swamp 警告、确认/拒绝、准备传送提示。
- Swamp 骨堆搜索入口和隐藏洞穴提示。

处理原则：

- 保留 `AdmissionOrb`、`StoneHeart`、`ZumaHeart`、`Dark Swamp`、`StoneTemple`、`MongchonDelegate` 等内部引用名称。
- 保留 `GROUPLEADER`、`GROUPCOUNT`、`CHECKMAP`、`CHECKRANGE`、`CHECKITEM`、`TAKEITEM`、`GROUPTELEPORT`、`MOVE`、`CHECKQUEST`、`LEVEL` 等命令。
- 保留 `@pass`、`@checkorb`、`@stonetomba`、`@zumatemplea`、`@transport`、`@next1` 等脚本入口。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `MongchonProvince` 扫描 `I will not help an evil person`、`<Close/@`、`Hello`、`Welcome`、`Which item`、`What Item`、`Buy Back`、`Back/@`、`These are the items`、`Would you like`、`Please`、`I'm`、`I am`、`Nothing happens`、`Have you heard`、`You have`、`You dont`、`ready`、`Proceed`、`Thank`、`Mysterious Stone`、`Ancient symbols`、`Required {Level`、`Only party`、`slight whisper`、`weilding`、`You may pass`、`monsters are strong`、`Search/@`、`hidden hole`、字面 `` `r`n `` 和 `\r\n` 等旧英文/格式残留，除保留的 `Dark Swamp` 名称引用外未再命中。
- 联合复扫 `BichonProvince`、`MongchonProvince`、`WoomyonWoods`、`PrajnaIsland` 的玩家侧旧英文模式，剩余命中为 `Reincarnation` 技能名引用。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十三批 顶层 NPC 与 GM 工具 NPC 通用文本

更新时间：2026-06-04

本批继续扩大处理范围，合并处理顶层短脚本与 `GM` 工具目录通用模板，共 27 个文件：

- `Build/Server/Debug/Envir/NPCs/TravellingMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/Test.txt`
- `Build/Server/Debug/Envir/NPCs/GM/*.txt`

覆盖内容：

- TravellingMerchant 的 PK 拒绝、问候、出售入口与交易提示。
- Test NPC 的强化询问按钮。
- GM 物品商店类文件的问候、购买/出售入口、购买/出售页、返回按钮。
- GM 修理类文件的普通修理、特殊修理提示。
- GM-Manager 的宠物清理、仓库、任务标记重置、活动管理、Sabuk 面板、行会领地传送入口、统计提示等通用可见文本。
- GM-Teleporter 中通用的主菜单/离开按钮。

处理原则：

- 保留 `ISADMIN`、`GIVEITEM`、`GIVESKILL`、`CHANGELEVEL`、`CLEARPETS`、`SET`、`MOVE`、`STARTCONQUEST` 等管理命令。
- 保留 `[Trade]`、`[Types]`、`[Quests]` 数据段，以及物品名、技能名、地图名、GM 内部入口名。
- 保留 `@Buy`、`@Sell`、`@Repair`、`@SRepair`、`@KillPets`、`@STORAGE`、`@ResetQuestFlags`、`@EventManagement`、`@GT*` 等脚本入口。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `GM`、`TravellingMerchant.txt`、`Test.txt` 扫描 `Hello`、`Back/@`、`Exit/@`、`Main Menu`、`Look at`、`Kill my Pets`、`Open my Storage`、`Reset Quest Flags`、`Your pets`、`Trust me`、`All quest flags`、`Welcome to`、`Please select`、`Conquest Gold`、`Intrest`、`Next Conquest`、`Guild Territory [0-9]`、`Buy.`、`Sell.`、`Repair.`、`Special repair`、`Buy/@Buy>`、`Sell/@Sell>`、`Want a boost`、`Managing the affairs`、`What function`、`global events`、`Repair All Walls`、`Below are`、字面 `` `r`n `` 和 `\r\n` 等旧英文/格式残留，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十四批 全 NPC 目录可见英文残留扩大清理

更新时间：2026-06-04

本批按“全 NPC 目录残留清理”方式扩大处理范围，不再按单一区域小批推进，集中修复跨目录旧模板、GM 工具菜单和装备商店同构残留：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/Sailor.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/StrangeMan-D002.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/OmaCave/StrangeMan.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event/Proceeder30-EM001.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event/Proceeder40-EM002.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event/Proceeder41-EM003.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Warehouse-0125.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Warehouse2.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Warehouse-0140.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/Ashes.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/14Qas-D604.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/SabukWall/Bracelet.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/SabukWall/8Abr-0154.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Ring.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Bracelet.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Bracelet1.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Clothes.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Teleporter.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Armour.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Belt.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Boot.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Bracelet.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Grocery.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Helmet.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Mount.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Necklace.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Potion.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Ring.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Stone.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Manager.txt`

覆盖内容：

- BichonProvince 船夫航线说明、边境介绍、金币不足提示。
- StrangeMan 的 BoneElite 警告文本。
- Event Proceeder 完成态离开按钮。
- Ashes / 14Qas-D604 的遗骸提示和返回 LostSoul 的 `LOCALMESSAGE`。
- GM-Manager 的城墙/城门修复广播文本。
- GM-Teleporter 的主菜单提示、城镇/地下城分类、店铺/安全区/洞穴通用按钮和待补提示。
- GM 装备商店与 MudWall 装备商店的“当前佩戴/持有”提示、HP/MP 提示和修理按钮。
- SabukWall 手镯/手套修理按钮与修理说明。
- BichonWall / BorderVillage 仓库购买按钮。

处理原则：

- 保留 `MOVE`、`CHECKGOLD`、`TAKEGOLD`、`ISADMIN`、`LOCALMESSAGE`、`CASTLEGATE`、`CASTLEWALL`、`WALLREPAIR`、`LINEMESSAGE` 等脚本命令。
- 保留 `@brdmove`、`@talk`、`@exit`、`@Tele*`、`@Repair`、`@SRepair`、`@Buy`、`@Storage` 等跳转入口。
- 保留 `BoneElite`、`LostSoul`、`Prajna Island`、`White Valley`、`Bichon Province`、`Gold`、装备变量和物品/地图/怪物内部名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对全 `NPCs` 目录扫描 `It's difficult`、`run away`、`Someones remains`、`The Remains`、`Return to the LostSoul`、`The boat goes`、`Pay ... Gold and Board`、`Which frontier`、`Gate Opened`、`Gate Closed`、`All Walls Repaired`、`You have Repaired`、`Buy.`、`Sell.`、`Repair.`、`Special repair.`、`<Exit/@exit>`、`<Close/@exit>`、`<Back/@` 等旧模板残留，未再命中。
- 针对全 `NPCs` 目录扫描 `Hello`、`Welcome`、`Please`、`I can`、`I will`、`Where shall`、`I see you`、`Teleport to`、`Tip:`、`You can repair` 等明显英文提示句，未再命中。
- 针对全 `NPCs` 目录扫描 `<Repair/@Repair>`、`<Special Repair/@SRepair>`、`Weapon shop`、`Armour shop`、`Accessory shop`、`Reagent store`、`Book store`、`Safezone`、`Caves:`、`Purchase/@Buy` 等可见按钮/菜单残留，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十五批 系统提示收口与 BorderVillage 任务正文整目录翻译

更新时间：2026-06-04

本批继续扩大处理范围，从 NPC 脚本扩展到 `Events`、`SystemScripts` 和任务正文目录，集中处理系统提示残留，并整目录翻译 `BichonProvince/BorderVillage` 任务正文：

- `Build/Server/Debug/Envir/Events/0-Event-40.txt`
- `Build/Server/Debug/Envir/Events/gumi203-Event-41.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/OnAcceptQuests.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/Login/LevelEffect.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/MapCoords/PenalCavern.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/MapCoords/DogYoArena.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Mount.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Manager.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/14Qp1-D701.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/14Qp2-D701.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/14Qp3-D701.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/WierdPillar.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/StrangePillar.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MysteriousPillar.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BorderVillage/*.txt`

覆盖内容：

- Bichon Lord / FlyingStatue 事件完成提示。
- 默认接任务脚本中的占用提示、稍后再来按钮和击杀提示。
- 登录 50/60/70 级特效提示。
- PenalCavern / DogYoArena 地图进入限制提示，并修复文件中既存异常命令拼写为 `#ELSEACT`、`LEVEL`、`LocalMessage`。
- GM-Mount 的坐骑忠诚度与等级限制说明。
- GM-Manager 的城门、守卫和 SabukWall 一键修复/复活提示，以及守卫/城墙/城门修复按钮。
- MongchonProvince 三个石柱发现提示。
- BorderVillage 1-22 号任务的 `[@Description]` 与 `[@TaskDescription]` 中文正文。

处理原则：

- 保留 `GIVEEXP`、`GIVEGOLD`、`ADDATTRIBUTEPOINT`、`#INCLUDE`、`MONCLEAR`、`MONGEN`、`MOVE`、`LEVEL`、`CHECKITEM`、`TAKEITEM`、`SealHero` 等脚本命令。
- 保留任务文件中的 `[@KillTasks]`、`[@ItemTasks]`、`[@CarryItems]`、奖励段、物品键、技能名和坐标注释。
- 保留 `Bichon Lord`、`Mete FlyingStatue`、`SealedHero`、`WierdPillar`、`StrangePillar`、`MysteriousPillar`、`SabukWall`、任务物品和 NPC 内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `Quests/BichonProvince/BorderVillage` 扫描 `Hello`、`Welcome`、`Please`、`Thank`、`Speak`、`Collect`、`Eliminate`、`traveller`、`Coordinates`、`recommended`、`disciple`、`reward`、`rumor`、`Recommendation` 等旧英文任务正文残留，未再命中。
- 针对 `Events`、`SystemScripts`、`GM-Manager` 和 MongchonProvince 石柱脚本扫描英文 `LOCALMESSAGE`、`LocalMessage`、`LINEMESSAGE`、`SENDMSG` 提示，剩余命中仅为中文句子中保留的 `SabukWall` 内部地名。
- 扫描 `oocalMessage`、`oevel`、`#EoSEACT`、`Mysterious evil force`、`Doors are locked` 等异常拼写/旧英文残留，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十六批 BichonWall 任务正文整目录翻译

更新时间：2026-06-04

本批继续按整目录方式推进任务正文翻译，处理 `BichonProvince/BichonWall` 主线、支线和 Board 悬赏任务：

- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/1.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/2.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/3.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/4.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/5.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/6.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/7.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/8.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/9.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/10.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/11.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/12.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/13.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/14.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/15.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/16.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/17.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/18.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/19.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/20.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/21.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/22.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/23.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/24.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/25.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/26.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/Board/1.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/Board/2.txt`

覆盖内容：

- ForestYeti、Oma、SpittingSpider、RedViper / TigerViper、Skeleton、Zombie、SpiderFrog、BoneElite、RoninGhoul 等击杀/收集任务说明。
- WoomyonWoods、SerpentValley、MongchonProvince、TaoVillage、PrajnaIsland 等跨区域引导任务说明。
- CanniTea、RepairOil、RareCopperOre、OldNecklace、BeefRib、CorpsFlower 等递送/收集任务说明。
- BichonWall Board 的 GoldChestnut 与 Skeleton Bone 悬赏说明。
- `26.txt` 中 Emperor 士气任务正文和 `[@FlagTasks]` 显示文本。

处理原则：

- 仅翻译任务文件中的 `[@Description]`、`[@TaskDescription]`、必要的 `[@Completion]` 和可见 `[@FlagTasks]` 文本。
- 保留 `[@KillTasks]`、`[@ItemTasks]`、`[@CarryItems]`、奖励段、物品键、怪物键、技能奖励、坐标注释和 NPC 内部名。
- 保留 `BichonWall`、`BichonProvince`、`OmaCave`、`DeadMineEntrance`、`PrajnaIsland`、`Emperor` 等内部引用名称。
- 修正本批处理中出现的字面 `` `r`n `` 换行残留，以及 `12.txt` 的段落标记异常，恢复为 `[@Description]` / `[@TaskDescription]`。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `Quests/BichonProvince/BichonWall` 扫描字面 `` `r`n ``、`[@o`、`Taskoescription` 等格式异常残留，未再命中。
- 针对同目录扫描 `Hello`、`Welcome`、`Please`、`Thank`、`Do you`、`Have you`、`Speak`、`Collect`、`Eliminate`、`Find`、`Travel`、`Talk`、`Deliver`、`Return`、`traveller`、`Traveler`、`Coordinates`、`recommended`、`disciple`、`reward`、`rumor`、`Bounty`、`Spread good morale`、`worthey`、`desire` 等旧英文正文残留，未再命中。
- 抽查 `1.txt`、`12.txt`、`22.txt`、`25.txt`、`26.txt`，任务段落结构、空行、任务键和奖励段保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十七批 BichonProvince 剩余任务正文收口

更新时间：2026-06-04

本批收口 `BichonProvince` 下 BorderVillage / BichonWall 之外的剩余任务正文，并补修上一批复扫发现的 BichonWall 描述残留：

- `Build/Server/Debug/Envir/Quests/BichonProvince/1.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/DeadMine/1.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/DeadMine/2.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/OmaCavern/1.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/OmaCavern/2.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/2.txt`

覆盖内容：

- DeadMine 中寻找 Vincent 朋友遗骸和复仇击杀 Shaman Zombies 的任务正文。
- OmaCavern 中 MysteriousStone、AncientScyther 和古代符号解读任务正文。
- BichonProvince 顶层模板任务说明。
- 修复 `BichonWall/2.txt` 中遗留的 Oma 袭击商人英文描述。
- `OmaCavern/2.txt` 中 `[@FlagTasks]` 的可见说明文本。

处理原则：

- 保留 `[@KillTasks]`、`[@ItemTasks]`、`[@CarryItems]`、奖励段、物品键、怪物键和坐标注释。
- 保留 `Vincent`、`BichonDeadMine`、`CastleBichon`、`MysteriousStone`、`AncientScyther`、`Scyther`、`Oma` 等内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对全 `Quests/BichonProvince` 扫描字面 `` `r`n ``、`[@o`、`Taskoescription`、`Template`、`Hello`、`Welcome`、`Please`、`Thank`、`Do you`、`Have you`、`Speak`、`Collect`、`Eliminate`、`Find`、`Travel`、`Talk`、`Deliver`、`Return`、`traveller`、`Traveler`、`Coordinates`、`recommended`、`disciple`、`reward`、`rumor`、`Bounty`、`help`、`scyther`、`symbols`、`Enquire`、`Finally Understand`、`danager` 等旧英文/格式残留，剩余命中仅为 `[@KillTasks]` 段名。
- 抽查 `DeadMine/1.txt`、`OmaCavern/2.txt`、`BichonWall/2.txt`，任务段落、任务键、奖励段和坐标注释保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十八批 SerpentValley 任务正文整区翻译

更新时间：2026-06-04

本批按区域整块处理 `SerpentValley` 任务正文，覆盖村庄任务和 SerpentDeadMine 矿洞任务：

- `Build/Server/Debug/Envir/Quests/SerpentValley/Village/*.txt`
- `Build/Server/Debug/Envir/Quests/SerpentValley/Serpent Mines/*.txt`

覆盖内容：

- Village 1-16 号任务：JadeRing 找回与递送、SnakeWine 制作与递送、蛇类击杀、SecretRecipe 递送、MasterShok 拜访、Sandford 解药、MobBlood / OldRobe / TraineeSuit 递送等正文。
- Serpent Mines 1-8 号任务：GhostZombie / DarkPriestZombie、ChainGhoul、HiGreatGhoul、RotNdZombie、RotShamanZombie、WhiteSerpent 等矿洞击杀/调查任务正文。
- HiddenScroll、StolenGold、HeartOfDead、BloodySword 等任务物品相关说明。

处理原则：

- 仅翻译 `[@Description]` 与 `[@TaskDescription]` 的可见正文。
- 保留 `[@KillTasks]`、`[@ItemTasks]`、`[@CarryItems]`、奖励段、怪物键、物品键和坐标注释。
- 保留 `SerpentValley`、`SerpentDeadMine`、`SerpentDeadMines`、`PrajnaIsland`、`BichonWall`、`WoomyonWoods`、NPC 名和任务物品内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `Quests/SerpentValley` 扫描字面 `` `r`n ``、`[@o`、`Taskoescription`、`Hello`、`Welcome`、`Please`、`Thank`、`Do you`、`Have you`、`Speak`、`Collect`、`Eliminate`、`Find`、`Travel`、`Talk`、`Deliver`、`Return`、`traveller`、`Traveler`、`Coordinates`、`reward`、`rumor`、`Rumours`、`Kill`、`Hunt`、`help`、`purchase`、`dire need`、`new's`、`promised`、`enjoy`、`sold out`、`sorry`、`aura`、`Investigate`、`retrieve` 等旧英文/格式残留，剩余命中仅为 `[@KillTasks]` 段名。
- 抽查 `Village/5.txt`、`Village/12.txt`、`Serpent Mines/7.txt`，任务段落、任务键、奖励段和坐标注释保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第五十九批 HolySword 任务链正文翻译

更新时间：2026-06-04

本批单独处理 `HolySword` 独立任务链：

- `Build/Server/Debug/Envir/Quests/HolySword/1.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/2.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/3.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/4.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/5.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/6.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/7.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/8.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/9.txt`

覆盖内容：

- HolySword 被盗背景故事。
- TreePath / RedValley / RedValley_3F / RedValley_5F / KingsRoom(RedMoonEvil) 的任务引导。
- RootSpider、BigApes、RedEvilApe、GreyEvilApes、RedMoonEvil 等击杀/收集任务说明。
- RedMoonChip、JadeCrystal、EvilApeOil、EvilHeart、RedMoonSword 相关说明。
- BigTaoist 试炼完成后的 RedMoonEvil 污血、伪装、组队提示等 `[@Completion]` 长文本。
- `[@FlagTasks]` 与带引号 `[@KillTasks]` 的可见任务说明文本。

处理原则：

- 保留 `[@KillTasks]`、`[@ItemTasks]`、`[@CarryItems]`、奖励段、怪物键、物品键和坐标注释。
- 保留 `HolySword`、`TrainerTaoist`、`School's Owner`、`Perry`、`Abel`、`BigTaoist`、`RedMoonEvil`、`RedMoonSword` 等内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `Quests/HolySword` 扫描 `ooot`、`oeward`、`[@Fixedo`、`[@Selecto`、`[@Expo`、`[@Goldo`、字面 `` `r`n ``、`[@o`、`Taskoescription` 等异常格式残留，未再命中。
- 针对同目录扫描 `long time`、`villain`、`stole`、`Travel`、`Eliminate`、`Retrieve`、`Listen`、`Story`、`Kill`、`Bring`、`ominous`、`hidden door`、`ordeal`、`prepared`、`foul blood`、`disguised`、`smearing`、`depends`、`Band together`、`Slay`、`RootSpider in TreePath`、`Take the JadeCrystal` 等旧英文正文残留，剩余命中为 `[@KillTasks]` 段名和保留的 `School's Owner` NPC 引用。
- 抽查 `2.txt`、`4.txt`、`7.txt`、`8.txt`、`9.txt`，任务段落、任务键、奖励段和坐标注释保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十批 MongchonProvince Bug Cave 与 StoneTomb 任务正文

更新时间：2026-06-04

本批继续按目录块处理 `MongchonProvince` 任务正文，覆盖 Bug Cave 与 StoneTomb：

- `Build/Server/Debug/Envir/Quests/MongchonProvince/Bug Cave/*.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/StoneTomb/*.txt`

覆盖内容：

- Bug Cave 1-5 号任务：Death Valley 虫类调查、WhimperingBee 标本、CharleySword、LostSoul 遗体调查、LifeStone / OldNecklace / Rachel 递送链。
- StoneTomb 1-6 号任务：BlackMaggot / WedgeMoth、BlackBoar / RedBoar、WhiteBoar、Stone Temple 石头、Ancient Layer、RelicRock、BloodPill 等任务正文。
- `Bug Cave/3.txt` 和 `StoneTomb/4.txt` 的 `[@FlagTasks]` 可见说明文本。

处理原则：

- 仅翻译 `[@Description]`、`[@TaskDescription]` 和可见 `[@FlagTasks]` 文本。
- 保留 `[@KillTasks]`、`[@ItemTasks]`、`[@CarryItems]`、奖励段、怪物键、物品键和坐标注释。
- 保留 `Death Valley`、`DeathValley`、`LostSoul`、`LifeStone`、`Stone Temple`、`Ancient Layer`、`StoneTemple` 等内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `Quests/MongchonProvince/Bug Cave` 与 `Quests/MongchonProvince/StoneTomb` 扫描字面 `` `r`n ``、`[@o`、`Taskoescription`、`Did anyone`、`cliffs`、`abnormally`、`researching`、`Travel`、`retrieve`、`Help`、`trapped`、`Investigate`、`returned`、`Please`、`Thankyou`、`Hunt`、`return my`、`Daughter`、`assistance`、`Find the LostSouls`、`Hello`、`Welcome`、`Insects`、`sleep`、`attacked`、`Boar looking`、`Eliminate`、`Deep`、`Darker Evil`、`Find the Stone`、`rumours`、`Collect`、`Kill`、`Youve` 等旧英文/格式残留，剩余命中仅为 `[@KillTasks]` 段名。
- 抽查 `Bug Cave/3.txt`、`StoneTomb/4.txt`、`StoneTomb/6.txt`，任务段落、任务键、奖励段和坐标注释保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十一批 MongchonProvince SabukWall 与 MudWall 任务正文

更新时间：2026-06-04

本批继续收口 `MongchonProvince` 剩余任务块，处理 SabukWall、MudWall 与 MudWall Board：

- `Build/Server/Debug/Envir/Quests/MongchonProvince/SabukWall/*.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/MudWall/1.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/MudWall/Board/1.txt`

覆盖内容：

- SabukWall 1-5 号任务：BichonTales 书卷找回、Volume 1/2、Mr.Wang 引导、Hidden Entrances / Pillar 搜索、CrawlerZombies 击杀说明。
- MudWall 任务：向 BichonProvince 征讨管理员报告。
- MudWall Board 悬赏：StoneTemple 中 Boar 击杀与 Boar Tooth 收集。
- `[@FlagTasks]` 与带引号任务说明中的可见文字。

处理原则：

- 仅翻译 `[@Description]`、`[@TaskDescription]`、可见 `[@FlagTasks]` 和带引号任务说明文本。
- 保留 `BichonTales(1/2/3)`、`WierdPillar`、`StrangePillar`、`MysteriousPillar`、`CrawlerZombie`、`BoarTooth`、`Boar` 等内部键。
- 保留 `Hidden Entrance`、`Hidden Entrances`、`SabukWall`、`MudWall`、`StoneTemple` 等内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `Quests/MongchonProvince` 扫描 `rrawler`、`ooar`、`rrimson`、`[@rom`、`[@rar`、`LightSteelolue`、字面 `` `r`n ``、`[@o`、`Taskoescription`、`oeward` 等异常残留，未再命中。
- 针对同目录扫描 `I'm a fool`、`Oldest Book`、`Hello`、`Please Visit`、`Rumours`、`Eliminate some`、`Collect the Boar`、`Commanding Officer`、`Report`、`Bounty, Has`、`Travel to`、`Find the Hidden`、`retrieve` 等旧英文正文残留，未再命中。
- 抽查 `SabukWall/4.txt`、`SabukWall/5.txt`、`MudWall/Board/1.txt`，任务段落、任务键、奖励段和坐标注释保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十二批 PrajnaIsland 任务正文整区翻译

更新时间：2026-06-04

本批按整区方式处理 `PrajnaIsland` 任务正文：

- `Build/Server/Debug/Envir/Quests/PrajnaIsland/PIVillage/*.txt`
- `Build/Server/Debug/Envir/Quests/PrajnaIsland/PranjaTemple/*.txt`
- `Build/Server/Debug/Envir/Quests/PrajnaIsland/PranjaStoneCave/*.txt`

覆盖内容：

- PIVillage 1-5 号任务：RedSnakeTeeth 解药、ToxicGhoul / RoninGhoul 击杀、VillageChief 引导、TaoVillage / LeadTrainer 返回、Algea 递送。
- PranjaTemple 1-3 号任务：Minotaur、Wind/Elec/Fire/Ice Minotaur、LeftGuard / RightGuard 击杀说明。
- PranjaStoneCave 1-3 号任务：BoneArcher、BoneSpearman、BoneBlademan、CleanSkull 收集任务正文。

处理原则：

- 仅翻译 `[@Description]` 与 `[@TaskDescription]` 可见正文。
- 保留 `[@KillTasks]`、`[@ItemTasks]`、`[@CarryItems]`、奖励段、怪物键、物品键和坐标注释。
- 保留 `PrajnaIsland`、`Prajna Island`、`PrajnaTemple`、`PrajnaStoneCave`、`TaoVillage`、`MongchonProvince`、`CleanSkull` 等内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `Quests/PrajnaIsland` 扫描字面 `` `r`n ``、`[@o`、`Taskoescription`、`oeward`、`rrimson`、`ooar`、`RedSnake ... bite`、`Please`、`Do you`、`Travel`、`Eliminate`、`Collect`、`Speak`、`Deliver`、`stonger`、`faster`、`Order`、`strange`、`Warriors died`、`brought back`、`Please help` 等旧英文/格式残留，剩余命中为保留的 `CleanSkull` 内部物品名。
- 抽查 `PIVillage/1.txt`、`PranjaTemple/2.txt`、`PranjaStoneCave/3.txt`，任务段落、任务键、奖励段和坐标注释保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十三批 WoomyonWoods Castle-GI 与 Wilderness 任务正文

更新时间：2026-06-04

本批开始处理 `WoomyonWoods` 任务正文，先覆盖 Castle-GI 城镇线、Wilderness 短任务和 InsectCave 任务链：

- `Build/Server/Debug/Envir/Quests/WoomyonWoods/Castle-GI/Town/*.txt`
- `Build/Server/Debug/Envir/Quests/WoomyonWoods/Wilderness/*.txt`
- `Build/Server/Debug/Envir/Quests/WoomyonWoods/Wilderness/InsectCave/*.txt`

覆盖内容：

- Castle-GI Town 1-8 号任务：CookBook 递送、MasterShok 求援、InsectCave 昆虫、SubjugationLead 警告、WoomaTemple 威胁、WoomaGuardian / OmaWarrior 等任务正文。
- Wilderness 1-3 号任务：CannibalPlant 材料收集、OmaFighter、Sarah 递话与完成回复。
- Wilderness/InsectCave 1-6 号任务：BugEye、BugBlood、BluJadeNecklace、Soho 搜索、Antidote、GatheringGlove / GatheringTool / GreenHerb 等任务正文。
- `Castle-GI/Town/5.txt` 和 `Wilderness/3.txt` 的 `[@Completion]` 可见完成文本。

处理原则：

- 仅翻译 `[@Description]`、`[@TaskDescription]` 和必要的 `[@Completion]` 可见文本。
- 保留 `[@KillTasks]`、`[@ItemTasks]`、`[@CarryItems]`、奖励段、怪物键、物品键和坐标注释。
- 保留 `CastleGi-Ryoong`、`WoomyonWoods`、`InsectCave`、`WoomaTemple`、`Temple`、`Traveller_Soho` 等内部引用名称。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批目录扫描字面 `` `r`n ``、`[@o`、`Taskoescription`、`oeward`、`rrimson`、`ooar`、`Hello`、`Thankyou`、`Please`、`Do you`、`Can you`、`Travel`、`Eliminate`、`Kill`、`Collect`、`Talk`、`Warn`、`warm them`、`threat`、`Howling`、`Source`、`cant`、`sight`、`poisoned`、`took my bag`、`get my stuff`、`another task`、`reward`、`Traveller`、`Traveler` 等旧英文/格式残留，剩余命中为 `[@KillTasks]` 段名和注释中的 `Traveller_Soho` 标识。
- 抽查 `Castle-GI/Town/5.txt`、`Wilderness/3.txt`、`Wilderness/InsectCave/1.txt`，任务段落、任务键、奖励段和坐标注释保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十四批 WoomyonWoods TaoistVillage 任务正文

更新时间：2026-06-04

本批继续处理 `WoomyonWoods/TaoistVillage` 任务正文，覆盖 Tao Village 主任务、TreePath 任务和 WW Mines 任务：

- `Build/Server/Debug/Envir/Quests/WoomyonWoods/TaoistVillage/WW Mines/1.txt`
- `Build/Server/Debug/Envir/Quests/WoomyonWoods/TaoistVillage/TreePath/1.txt`
- `Build/Server/Debug/Envir/Quests/WoomyonWoods/TaoistVillage/TreePath/2.txt`
- `Build/Server/Debug/Envir/Quests/WoomyonWoods/TaoistVillage/Tao Village/1.txt`
- `Build/Server/Debug/Envir/Quests/WoomyonWoods/TaoistVillage/Tao Village/1 .txt`
- `Build/Server/Debug/Envir/Quests/WoomyonWoods/TaoistVillage/Tao Village/2.txt` 至 `16.txt`

覆盖内容：

- WW Mines 僵尸威胁任务：CursedZombie、ShiZombie、HungryZombie 讨伐正文和 `[@KillTasks]` 可见标签。
- TreePath 材料收集与 EvilBigApe 悬赏任务正文。
- Tao Village 1-16 号任务：Helen 递话、TreePath 资源、Zombie Heart 仪式、Ann 订单、失踪马车、MineralMines 僵尸、AncientTree、WornAxe、怪物讨伐令、失踪学生、TraineeRing、SkysLetter、BichonExaminer、PrajnaHistory 追回与归还等任务正文。

处理原则：

- 仅翻译 `[@Description]`、`[@TaskDescription]` 和 `[@KillTasks]` 中带引号的玩家可见说明。
- 保留 `[@KillTasks]` 数值、`[@ItemTasks]`、`[@CarryItems]`、奖励段、怪物键、物品键、NPC/地图内部引用和坐标注释。
- `TaoistVillage/1 .txt` 与 `TaoistVillage/Tao .txt` 为当前空白结构任务文件，未注入新文本。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `WoomyonWoods/TaoistVillage` 扫描字面 `` `r`n ``、`[@o`、`Taskoescription`、`oeward`、`rrimson`、`ooar`、`rrawler`、`LightSteelolue`、`[@rom`、`[@rar` 以及 `Hello`、`Please`、`Thank`、`Speak`、`Collect`、`Eliminate`、`Find`、`Travel`、`Talk`、`Deliver`、`Return`、`Order`、`Delivery`、`Search`、`Threat` 等旧英文/格式残留，剩余命中为 `[@KillTasks]` 段名、内部击杀/物品键和注释中的 `Traveller_Soho` 标识。
- 抽查 `Tao Village/10.txt`，任务说明、`[@KillTasks]` 数值、奖励段和段落标签保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十五批 AncientCaves 与 WasteLand 任务正文补漏

更新时间：2026-06-04

本批根据全局任务正文残留扫描，处理仍带英文任务句的高命中点：

- `Build/Server/Debug/Envir/Quests/AncientCaves/OmaCavern/1.txt`
- `Build/Server/Debug/Envir/Quests/AncientCaves/OmaCavern/2.txt`
- `Build/Server/Debug/Envir/Quests/WasteLand/Red Cave/1.txt`
- `Build/Server/Debug/Envir/Quests/WasteLand/Red Cave/2.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/15.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/Bug Cave/2.txt`

覆盖内容：

- AncientCaves/OmaCavern 神秘石头、古代符号、AncientScyther 搜索任务正文。
- OmaCavern `[@FlagTasks]` 中带引号的玩家可见步骤说明。
- WasteLand/Red Cave 前往 AncientRuins 前置怪物清理任务、DreamDevourer / DarkSoulDevouer 威胁任务正文。
- BichonWall 15 号武器材料与 DeadMineEntrance 僵尸说明残留。
- MongchonProvince/Bug Cave 2 号任务中残留的 `Boss Monster` 显示文本。

处理原则：

- 仅翻译 `[@Description]`、`[@TaskDescription]` 和 `[@FlagTasks]` 中带引号的玩家可见说明。
- 保留 `[@KillTasks]`、`[@ItemTasks]`、`[@CarryItems]`、奖励段、怪物键、物品键和地图/NPC 内部引用名称。
- `DarkDevouer` 等疑似配置键保持原样，不在任务正文补漏阶段修正。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描字面 `` `r`n ``、`[@o`、`Taskoescription`、`oeward`、`rrimson`、`ooar`、`rrawler`、`LightSteelolue`、`[@rom`、`[@rar` 以及 `Hello`、`Please`、`Thank`、`Collect`、`Eliminate`、`Find`、`Travel`、`Return`、`Search`、`Monster`、`Book`、`Carriage`、`materials`、`Thanks`、`However`、`Recently`、`There must`、`Enquire` 等旧英文/格式残留，剩余命中仅为 `[@KillTasks]` 段名。
- 抽查 `AncientCaves/OmaCavern/2.txt` 与 `WasteLand/Red Cave/2.txt`，任务正文、可见步骤、奖励段和内部键保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十六批 Mongchon 石柱脚本与事件提示补漏

更新时间：2026-06-04

本批根据全 `Envir` 残留扫描，修复 MongchonProvince 石柱短脚本与事件奖励提示：

- `Build/Server/Debug/Envir/NPCs/MongchonProvince/WierdPillar.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/14Qp1-D701.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/StrangePillar.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/14Qp2-D701.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MysteriousPillar.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/14Qp3-D701.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MonDelegate.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MonDelegateMW.txt`
- `Build/Server/Debug/Envir/Events/0-Event-40.txt`
- `Build/Server/Debug/Envir/Events/gumi203-Event-41.txt`

覆盖内容：

- 石柱点击后的 `#SAY` / `#ELSESAY` 可见文本。
- 石柱发现提示、Bichon Oord Event 完成提示、Mete FlyingStatue 事件提示。
- MudWall 代表 NPC 中残留的 Traveler 问候和断句英文。
- 修复同批扫描发现的命令拼写异常：`#EOSESAY` -> `#ELSESAY`、`#EOSEACT` -> `#ELSEACT`、`COOSE` -> `CLOSE`、`#INCOUDE` -> `#INCLUDE`、`OOCAOMESSAGE` -> `LOCALMESSAGE`。

处理原则：

- 保留 `CHECKQUEST`、`SET [521-523]`、`#INCLUDE [Events\Event\Event41.txt] @Main`、`BREAK` 等脚本逻辑。
- 保留 `WierdPillar`、`StrangePillar`、`MysteriousPillar`、`Bichon Oord Event`、`Mete FlyingStatue` 等内部标识或事件名。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `#EOSE`、`COOSE`、`#INCOUDE`、`OOCAOMESSAGE`、`wierd`、`eerie`、`What a`、`You get` 等旧英文/坏命令残留，剩余命中仅为已翻译 `LOCALMESSAGE` 中保留的内部事件名。
- 抽查 `MongchonProvince/WierdPillar.txt`、`MongchonProvince/MudWall/MonDelegate.txt` 与 `Events/gumi203-Event-41.txt`，脚本段名、条件、动作、提示命令、任务列表和关闭命令保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十七批 低频英文提示与测试 NPC 补漏

更新时间：2026-06-04

本批进入低频残留精扫，处理高频词表未覆盖的少量可见英文和脚本格式问题：

- `Build/Server/Debug/Envir/NPCs/Test.txt`
- `Build/Server/Debug/Envir/SystemScripts/00Default/MapCoords/BichonPalace.txt`

覆盖内容：

- `Test.txt` 中测试强化 NPC 的显示按钮：`Spells` -> `技能`，`Level` -> `提升等级`。
- `Test.txt` 中 3 处裸 `ELSESAY` 修复为标准 `#ELSESAY`。
- `BichonPalace.txt` 中 `[Commander Luke] Halt! You may not enter come talk to me.` 地图坐标阻挡提示。

处理原则：

- 仅翻译 `<显示文本/@目标>` 中斜杠前的显示文本，保留 `@SpellsWarrior`、`@Level` 等跳转目标。
- 保留 `GIVEITEM`、`GIVESKILL`、`CHANGELEVEL`、`CHECKQUEST`、`ENTERMAP` 等脚本命令和技能/物品键。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 复扫 `Test.txt` 中 `<Spells/@...>`、`<Level/@Level>` 和裸 `ELSESAY`，未再命中。
- 复扫 `BichonPalace.txt` 中 `Halt`、`enter`、`talk`、`Commander` 等英文提示残留，未再命中玩家侧英文。
- 全 `Envir` 引号内纯英文消息扫描无命中；坏命令/坏标签扫描剩余命中为 `[@oldman]`、`[@omacavea]`、`[@openmaindoor]`、`[@onewithnature]` 等小写跳转标签，非异常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十八批 船夫与 StrangeMan 低频补漏

更新时间：2026-06-04

本批继续处理低频残留精扫中发现的玩家可见短文本：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/Sailor.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Sailor.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Sailor.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/StrangeMan-D002.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/OmaCave/StrangeMan.txt`

覆盖内容：

- 三个船夫脚本中残留的 `Prajna Island`、`White Valley`、`WhiteValley`、`Bichon Province` 等显示地名。
- 船夫脚本中少量 `Gold` 显示文本统一为 `{Gold/Gold}`。
- WhiteValley 船夫对白中残留的 `monsters` 显示文本。
- StrangeMan 两个镜像文件中破损的 `OOk/@exit>` 修复为 `<好的/@exit>`。

处理原则：

- 仅翻译 `<显示文本/@目标>` 中斜杠前的显示文本，保留 `@brdmove`、`@talkPI`、`@talkWV`、`@wvmove` 等跳转目标。
- 保留 `MOVE` / `Move` 地图目标、坐标、`CHECKGOLD`、`TAKEGOLD` 和服务价格不动。
- 保留 `OmaCave`、`BoneElite` 等内部名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `OOk/@exit>`、`Prajna Island`、`White Valley`、`WhiteValley`、`monsters` 和字面 `Gold` 残留，剩余 `Gold` 命中均为 `{Gold/Gold}` 颜色/物品显示标记。
- 抽查 `BichonProvince/Sailor.txt` 与 `BichonProvince/StrangeMan-D002.txt`，显示文本、跳转标签和脚本命令保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第六十九批 GM 工具低频显示文本补漏

更新时间：2026-06-04

本批继续处理低频残留精扫中发现的 GM 工具可见文本：

- `Build/Server/Debug/Envir/NPCs/GM/GM-Book.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-CraftingMaterial.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Script.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Manager.txt`

覆盖内容：

- GM 技能书工具中 `Learn all ... spells.` 显示说明。
- GM 制作材料工具中 `Craft` 按钮和 `What would you like to craft?` 制作提示。
- GM 脚本商店中游戏币余额提示。
- GM-Manager 英雄管理按钮、转生面板标题、转生应用/删除/检查按钮和转生状态反馈。
- GM-Manager 城墙与城门面板中的 `Walls and Gates` 标题。

处理原则：

- 仅翻译 `#SAY` 可见文本、`<显示文本/@目标>` 显示侧和状态反馈句。
- 保留 `GIVESKILL`、`GIVEITEM`、`[Trade]`、`[Recipe]`、`SET [993-1000]`、`ReviveHero`、`SealHero`、`CONQUEST*` 等脚本命令、交易键、配方键和标记编号。
- 保留 `@Warrior`、`@Craft`、`@RB1`、`@RBMAIN`、`@RepairAllWalls` 等跳转目标。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描字面 `` `n `` / `` `r ``、`Learn all`、`What would you like`、`gold coins`、`Hero Management`、`Here you can`、`Apply Rebirth`、`Delete Rebirth`、`Check Rebirth`、`Rebirth 1 granted`、`currently have`、`do not have`、`Main Rebirth`、`Create Hero`、`Walls and Gates`、`<Craft/@Craft>` 等旧英文/格式残留，未再命中。
- 抽查 `GM-Book.txt`、`GM-Script.txt` 和 `GM-Manager.txt` 城墙面板，显示文本、跳转标签、命令段和交易/配方段保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十批 GM-Teleporter 前半段与基础洞穴传送按钮

更新时间：2026-06-04

本批开始处理 `GM-Teleporter.txt` 的管理员传送面板显示文本：

- `Build/Server/Debug/Envir/NPCs/GM/GM-Teleporter.txt`

覆盖内容：

- 主菜单中的城镇、远古地下城、普通地下城和地下城首领传送按钮显示名。
- Bichon、Woomyon、SerpentValley、Mongchon、Castle-GI、PrajnaIsland、TaoVillage、WhiteVillage 等城镇摘要区的店铺/洞穴显示名。
- OmaCave、NaturalCave、InsectCave、WoomaTemple、DeadMine 基础洞穴楼层按钮显示名。
- 修复 PrajnaIsland 安全区按钮与段名格式：`<安全区@PISZ>` -> `<安全区/@PISZ>`，`[PISZ]` -> `[@PISZ]`。

处理原则：

- 仅翻译 `<显示文本/@目标>` 中斜杠前的显示名，保留 `@Tele...`、`@OC1`、`@DM12` 等跳转目标。
- 保留所有 `MOVE`/`Move` 地图名、坐标、GM 命令、段名和注释。
- 楼层显示中的 `1F`、`B1`、`B2` 等层级标识保留原格式。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批范围扫描 `Bichon Province`、`Border Village`、`Woomyon Woods`、`Serpent Valley`、`Mongcon Province`、`Prajna Island`、`White Village`、`Oma Cave`、`Natural Cave`、`Wooma Temple`、`Dead Mine`、`Insect Cave`、`Bug Cave`、`Stone Temple`、`White Dragon Passage` 等旧英文显示名，剩余命中为注释标题或下一批尚未处理的深层洞穴楼层列表。
- 抽查 PrajnaIsland 安全区段、NaturalCave 段和前 240 行主菜单，显示文本、跳转目标和脚本命令保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十一批 GM 目录整体收口

更新时间：2026-06-04

本批按用户要求集中收口 `GM` 目录剩余显示文本：

- `Build/Server/Debug/Envir/NPCs/GM/GM-Teleporter.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Manager.txt`
- `Build/Server/Debug/Envir/NPCs/GM/new 4.txt`

覆盖内容：

- `GM-Teleporter.txt` 后半段深层洞穴楼层按钮显示名：FoxCave、ZumaTemple、BugCave、StoneTemple、IceHell、MineralMine、HellCavern、RedCavern、PrajnaTemple、PrajnaStoneCave、BlackDragonDungeon、HellFire、RedValley、Swamp、Lunar、WhiteDragonPassage、SnowCavern、AncientNaturalCave、AncientTemple、AncientStoneTomb、AncientZumaTemple、AncientPrajnaCave。
- `GM-Teleporter.txt` 的 ViperPath Trello 提示文本。
- 修复 `GM-Teleporter.txt` 中 FoxCave 三个按钮对应段名重复为 `[@FC1]` 的问题，改为 `[@FC1]`、`[@FC2]`、`[@FC3]`。
- `GM-Manager.txt` 中残留的 `SabukWall` 按钮显示名。
- `new 4.txt` 中 GM 相关开发备注。

处理原则：

- 仅翻译 `<显示文本/@目标>` 中斜杠前的显示名，保留所有 `@...` 跳转目标、`MOVE` 地图名、坐标、段名、GM 命令和脚本动作。
- `GM-Weapon.txt` 等交易/物品清单中的物品键不翻译，例如 `Dragon'sRoyalBlades` 保持原样。
- 楼层显示中的 `1F`、`B1`、`B2`、`KR` 等层级/王房标识保留原格式。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对 `GM-Teleporter.txt` 扫描“斜杠前仍为纯英文且整行无中文”的显示按钮，未再命中。
- 针对整个 `NPCs/GM` 目录扫描纯英文可见句子，剩余命中仅为 `GM-Weapon.txt` 中的物品键 `Dragon'sRoyalBlades`。
- 针对 `NPCs/GM` 扫描字面 `` `n `` / `` `r ``、`#EOSE`、`COOSE`、`#INCOUDE`、`OOCAOMESSAGE` 和明显破损按钮格式，未再命中。
- 抽查 `GM-Teleporter.txt` 的 FoxCave、IceHell、BlackDragonDungeon、HellFire 等段，显示文本、跳转目标和脚本命令保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十二批 BichonWall 残留备注、Clothing 乱码与按钮格式修复

更新时间：2026-06-04

本批开始从 GM 目录转入非 GM 残留扫描，优先处理 BichonWall 中明确属于玩家可见文本或开发备注的低风险内容：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Civilians/Civilian List.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Clothing.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Clothing-0106.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide-0.txt`

覆盖内容：

- 翻译 `Civilian List.txt` 中居民任务链备注，保留 `Commander Luke`、SET 标记和任务状态编号。
- 修复 `Clothing.txt` 与 `Clothing-0106.txt` 中残留乱码和坏关闭按钮格式，恢复服装商人问候、买卖、回购、修理、谈话和任务对话文本。
- 修复服装 NPC 的 `<关闭/@exit>`、`<返回/@main>`、`<回购/@BuyBack>` 等显示按钮格式。
- 将 `MirGuide.txt` 与 `MirGuide-0.txt` 中 `Dung, etc` 调整为 `Dung 等`，其余怪物/地图名称引用保留给名称专项。

处理原则：

- 保留 `[Types]`、`[Trade]`、`[Quests]` 段结构与其下物品键、类型号、任务号不动。
- 保留 `CHECKPKPOINT`、`GOTO`、`CHECKQUEST`、`SET`、`CHECK [flag]` 等脚本命令与跳转段名。
- MirGuide 中疑似怪物、地图、技能和名称引用不强行翻译，继续归入“数据库 + 文件引用联动汉化”专项。

验证：

- 针对本批目标文件扫描常见乱码片段、破损标签、坏关闭按钮、字面 `` `n `` / `` `r ``、`Dung, etc`、旧英文备注等，未再命中。
- 抽查 `Clothing.txt` 与 `Clothing-0106.txt` 前 100 行，`#IF`、`#SAY`、`#ACT`、`#ELSESAY`、`[Trade]`、`[Quests]` 结构保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十三批 BichonWall 高级通行证与攻城备注收口

更新时间：2026-06-04

本批继续处理 BichonWall 非 GM 残留扫描中确认安全的英文备注：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/14Wr-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Premium.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Administrator.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Administrator-0122.txt`

覆盖内容：

- 将高级通行证检查备注翻译为“检查玩家是否持有高级通行证”。
- 将高级洞穴菜单备注翻译为“高级洞穴菜单”。
- 将攻城时间备注中的 `PM 8` 调整为“晚上 8 点”。

处理原则：

- 保留 `PremiumPass[1d]`、`PremiumPass[3d]`、`PremiumPass[7d]`、`PremiumPass[15d]` 等物品键不动。
- 保留 `@PremCaveMenu`、`@CHECKPASS...`、`<$CASTLEWARDATE>`、`MOVE` 地图编号和 `GIVEITEM TownTeleport` 等内部引用不动。

验证：

- 针对本批目标文件扫描旧英文备注、`PM 8`、坏按钮格式、字面 `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 抽查 `14Wr-0.txt` 与 `Administrator.txt` 相关片段，脚本段名、条件判断和变量占位符保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十四批 旅行商人与古代洞穴备注收口

更新时间：2026-06-04

本批扩大到整个 `NPCs` 目录中的安全备注项，处理纯备注表头和任务标记：

- `Build/Server/Debug/Envir/NPCs/TravellingMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/AncientCaves/AncientNatural-D003.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/OmaCave/Stone.txt`

覆盖内容：

- 将旅行商人刷新备注表头 `Location / Start - Finish` 调整为“地点 / 开始 - 结束”。
- 将古代洞穴与奥玛洞穴石碑脚本中的 `Quest 153` 备注调整为“任务 153”。

处理原则：

- 保留旅行商人的地图内部名、坐标和时间段不动。
- 保留石碑脚本中的楼层标记 `3F`、任务号 `153`、`CHECKQUEST`、`CHECK [536]` 和跳转目标不动。

验证：

- 扫描整个 `NPCs` 目录中的 `;;Quest`、`;;Location`、`Start - Finish`、高级通行证旧备注和 `PM 8.`，未再命中。
- 针对本批目标文件扫描坏按钮格式、字面 `` `t `` / `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十五批 全局脚本校验与 MudWall 屠夫漏翻修复

更新时间：2026-06-04

本批按阶段进度要求执行全局校验，并修复校验发现的可见英文漏翻：

- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Butcher.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Butcher-3.txt`

覆盖内容：

- 将 `<Ask/@Meathelp> about how to gain meat.` 翻译为“询问如何获得肉”。
- 翻译肉类品质说明中泥土污染、火烧焦导致低价收购的句子。
- 翻译获得肉的方法说明，包括动物来源、按住 ALT 左键点击尸体、肉进入背包和魔法击杀会降低品质。

处理原则：

- 保留 `@Meathelp`、`@Sell`、`[Types]` 和脚本结构不动。
- 保留颜色标记 `/Crimson`，仅将 `{Hens/Crimson}`、`{Deer/Crimson}`、`{Sheep/Crimson}`、`{Wolves/Crimson}` 的显示文字翻译为鸡、鹿、羊、狼。
- 名称专项范围内的 MirGuide 技能名、宠物/怪物名、地图内部名和传送内部名仍暂不处理。

验证：

- 全局扫描 `NPCs`、`Quests`、`Events`、`SystemScripts` 中常见乱码、坏脚本标签、坏按钮格式、字面 `` `t `` / `` `n `` / `` `r ``，未命中。
- 全局扫描旧英文备注和待补提示，如 `;;Location`、`Start - Finish`、`;;Quest`、`Checking whether`、`Premium Cave Menu`、`PM 8.`、`Does nothing`、`Future Quest`、`TO BE ADDED`，未命中。
- 英文按钮残留扫描仍命中 MirGuide 技能名、GuildTerritory 宠物/怪物名、MudWall 传送地图名等名称引用，归入名称专项；其中确认属于普通可见句子的 MudWall 屠夫肉类说明已在本批修复。
- 针对本批屠夫文件复扫旧英文肉类说明、坏按钮格式、字面 `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十六批 校验发现项：任务正文、委托商人与坏入口段名

更新时间：2026-06-04

本批继续执行校验驱动的收口，修复筛选出的低风险漏翻和一个脚本结构异常：

- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/21.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/SabukWall/4Adu-0153.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/SabukWall/Potion.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/12Pitm.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/12Pbtm.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/TrustMerchant-3.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/TrustMerchant.txt`

覆盖内容：

- 翻译任务 21 的尸体与项链描述正文，保留 `{Vincent/LimeGreen}` 与 `{OldNecklace/LightSteelBlue}`。
- 修复 SabukWall 两个脚本首行 `1[@MAIN]` 为 `[@MAIN]`，避免入口段名异常。
- 统一委托商人地点说明中的英文地名列表为“比奇省、盟重省、道馆村、般若岛和旧比奇”。

处理原则：

- 仅翻译任务正文与 NPC 可见说明，保留任务物品、奖励物品、颜色标记和脚本命令不动。
- `1[@MAIN]` 属于校验发现的脚本结构异常，本批只移除多余前缀 `1`，不改其他业务逻辑。
- 委托商人的 `@Market`、`@help`、手续费、保证金、寄售期限等既有逻辑保持不动。

验证：

- 针对本批目标文件复扫 `That body seems`、`necklace matches`、`Bichon Province、Mongchon Province`、`Island 和 Past Bichon`、`^\\d+\\[@`，未再命中。
- 抽查任务 21、MudWall 委托商人和 SabukWall 药剂脚本开头，显示文本与入口段名正常。
- 针对本批目标文件扫描坏按钮格式、常见坏脚本标签、字面 `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十七批 城镇传送按钮显示名收口

更新时间：2026-06-04

本批处理校验中残留的城镇传送按钮英文显示名，覆盖 8 个传送 NPC 镜像文件：

- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Transport.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/SValley_Transport-2.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Woomyon_Transporter1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Transporter.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/CastleGi_Transporter.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Transporter.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Mongchon_Transport-3.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/Transport.txt`

覆盖内容：

- 将 `BorderVillage`、`BichonWall`、`SerpentValley`、`MudWall`、`TaoistSchool`、`CastleGi-Ryoong`、`WoomyonCamp` 的按钮显示名分别调整为边境村、比奇城、毒蛇山谷、盟重土城、道馆村、基隆城、沃玛营地。
- 将一处服务费用中的 `2000 gold` 调整为 `2000 金币`。

处理原则：

- 仅翻译 `<显示文本/@目标>` 中斜杠前的显示文本，保留 `@Border`、`@BichonWall`、`@Serpent`、`@MudWall`、`@TaoVillage`、`@CastleGi`、`@Camp` 跳转目标不动。
- 保留 `MOVE` 地图编号、坐标、`CHECKGOLD`、`TAKEGOLD` 和传送价格不动。
- `CastleGi-Ryoong` 显示名沿用第四阶段地图译名“基隆城”。

验证：

- 针对本批目标文件复扫原英文传送按钮、`2000 gold`、坏按钮格式、常见坏脚本标签、字面 `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 抽查 `SerpentValley/Village/Transport.txt` 与 `WoomyonWoods/Castle-GI/CastleGi_Transporter.txt` 的传送菜单，显示名为中文，跳转目标与脚本命令保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十八批 金币显示词与旧比奇精炼说明收口

更新时间：2026-06-04

本批继续处理校验中发现的半英文可见文本：

- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Transport.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/2Pbbl.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/Unused.txt`

覆盖内容：

- 将毒蛇山谷传送员金币不足提示中的 `gold` 调整为“金币”。
- 将旧比奇武器精炼费用说明中的 `gold` 调整为“金币”。
- 翻译旧比奇精炼说明中的 `accessories`、`money`、`Weapon` 按钮显示词，以及首饰说明中的 `Accessories`、`Necklaces`、`Bracelets`、`Rings`。

处理原则：

- 保留 `@gold` 段名和 `GOTO @gold` 跳转不动，仅修改玩家可见句子里的 `gold`。
- 保留 `BlackIronOre` 物品名、`<$UPGRADEWEAPONFEE>`、`<$USERWEAPON>` 和 `@Biron`、`@Etc`、`@Gold`、`@Weapon`、`@upgradenow` 等脚本入口。
- 仅翻译 `<显示文本/@目标>` 中斜杠前的显示文本。

验证：

- 针对本批目标文件复扫 `accessories`、`Accessories`、`money/@Gold`、`Weapon/@Weapon`、`Necklaces`、`Bracelets`、`Rings`、`UPGRADEWEAPONFEE> gold`、`你的 gold`，未再命中。
- 抽查旧比奇精炼说明和毒蛇山谷金币不足提示，显示文本已中文化，脚本目标保持正常。
- 针对本批目标文件扫描坏按钮格式、常见坏脚本标签、字面 `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第七十九批 MirGuide 地区显示与首饰左右标记收口

更新时间：2026-06-04

本批处理校验剩余的玩家可见英文显示片段：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide-0.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide-3.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide-2.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuidePI.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Ring.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Bracelet.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Ring.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bracelet.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Jewellery.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Jewellery.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI/Jewellery.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/Jewellery.txt`

覆盖内容：

- 将 MirGuide 低等级狩猎区域介绍中的 `Bichon-Province`、`WoomyonWoods`、`Mongchon-Province`、`TaoVillage`、`CastleGi-Ryoong` 显示为比奇省、沃玛森林、盟重省、道馆村、基隆城。
- 将 MirGuide 中 `MineralMine` 显示为矿山。
- 将首饰 NPC 装备栏中的 `L:`、`R:` 标记调整为“左:”和“右:”。

处理原则：

- 仅处理玩家可见说明文本，保留 MirGuide 的技能名、怪物名、物品名和跳转目标给名称专项。
- 保留 `$RING_L`、`$RING_R`、`$BRACELET_L`、`$BRACELET_R`、`/CORAL` 等变量与颜色标记不动。

验证：

- 针对本批目标文件复扫旧地区显示行、`MineralMine。`、首饰 `L/R` 原格式、坏按钮格式、常见坏脚本标签、字面 `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 抽查 `BichonWall/MirGuide.txt` 和 `WoomyonWoods/Castle-GI/Jewellery.txt`，地区显示和首饰左右栏正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第八十批 MirGuide 狩猎区域说明地名收口

更新时间：2026-06-04

本批继续处理 MirGuide 镜像文件中玩家可见的狩猎区域说明：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide-0.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide-3.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide-2.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuidePI.txt`

覆盖内容：

- 翻译狩猎路线说明中的毒蛇山谷、比奇省、奥玛洞穴、死亡矿山、沃玛森林、昆虫洞、死亡山谷、沃玛寺庙、角形石墓、树径、般若岛、般若原野、般若石窟、般若寺庙、红谷、祖玛寺庙、时间石、旧比奇等显示地名。
- 将职业描述中的 `Wizard` 调整为“法师”，保留技能名 `Thunderbolt`。
- 清理翻译后中文短语前后的多余空格。

处理原则：

- 仅翻译 MirGuide 中玩家可见说明地名、职业通用词和自然语言，不改 `@...` 跳转目标。
- 怪物名、技能名、物品名如 `RedSnake`、`TigerSnake`、`OmaFighter`、`Hellfire`、`CaveBat`、`CaveMaggot`、`Skeletons`、`Candles`、`Town Teleports`、`Thunderbolt`、`Zombie` 等仍保留给名称专项。

验证：

- 针对本批目标文件复扫旧英文地名组合、坏按钮格式、常见坏脚本标签、字面 `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 抽查 `BichonWall/MirGuide.txt` 与 `PrajnaIsland/MirGuidePI.txt` 狩猎区域段，中文句子和脚本跳转保持正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第八十一批 PrajnaIsland 无底深渊文本与乱码镜像修复

更新时间：2026-06-04

本批处理 PrajnaIsland 中校验发现的可见地名残留与编码显示异常：

- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/14CBi.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Inspector1.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Inspector.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/VillageChief.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/VillageChiefPI.txt`

覆盖内容：

- 将 `Bichon Wall` / `BichonWall` 的可见描述调整为“比奇城”。
- 将 `Bottomless Pit` 的可见描述统一为“无底深渊”。
- 修复 `Inspector1.txt` 乱码镜像，按 `14CBi.txt` 正常内容重建，并显式写回 UTF-8。
- 将 `Inspector.txt`、`VillageChief.txt`、`VillageChiefPI.txt` 显式写回 UTF-8，避免 PowerShell/编辑器默认编码显示乱码。

处理原则：

- 保留 `@ask1`、`@ask2`、`@ask3`、`@next...`、`@Tmove`、`MOVE`、`random`、`break`、`[Quests]` 等脚本结构不动。
- 仅翻译玩家可见地名和自然语言，不改任务编号与跳转目标。

验证：

- 针对本批目标文件复扫 `Bottomless Pit`、`Bichon Wall`、`BichonWall 的怪物`、`从 Bichon`、常见乱码片段、坏按钮格式、字面 `` `n `` / `` `r `` 和常见坏脚本标签，未再命中。
- 抽查 `Inspector.txt` 和 `VillageChief.txt`，中文显示、无底深渊说明和脚本菜单正常。
- 服务端重启验证待补：本轮本地运行时重启命令仍受额度限制影响，未执行。

## 第八十二批 比奇与边境传送按钮译名一致性

更新时间：2026-06-05

本批处理比奇城与边境村传送 NPC 的地点按钮译名一致性：

- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Border_Transport-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage/Transport.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport1.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport1-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport2.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport2-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport3.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Bichon_Teleport3-0.txt`

覆盖内容：

- 将传送按钮显示名 `道观村` 统一为“道馆村”。
- 将传送按钮显示名 `蛇谷` 统一为“毒蛇山谷”。
- 将传送按钮显示名 `奇龙城` 统一为“基隆城”。

处理原则：

- 仅修改 `<显示文本/@目标>` 中的显示文本，保留 `@TaoVillage`、`@Serpent`、`@CastleGi` 跳转目标不动。
- 保留 `MOVE` 地图编号、坐标、`CHECKGOLD`、`TAKEGOLD`、`@gold` 段名和传送价格不动。

验证：

- 针对本批目标文件复扫 `道观村`、`<<蛇谷/@Serpent>>`、`<<奇龙城/@CastleGi>>`、坏按钮格式、常见坏脚本标签、字面 `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 抽查 `BichonProvince/BorderVillage/Transport.txt` 传送菜单，显示名已统一，跳转目标保持正常。
- 服务端重启验证待补：本轮尚未执行服务端重启验证。

## 第八十三批 旅行商人刷新备注与 GM 待备注释收口

更新时间：2026-06-05

本批继续处理校验中残留的注释/备注显示文本：

- `Build/Server/Debug/Envir/NPCs/TravellingMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Teleporter.txt`

覆盖内容：

- 将旅行商人刷新备注表中的地点名改为中文：边境村、比奇城、沃玛森林、基隆城、道馆村、石草山谷、毒蛇山谷、盟重土城、沙巴克城墙、般若岛、白色山谷。
- 将 GM 传送器中 `Serpent Valley Dead Mine *待补*` 注释调整为“毒蛇山谷废矿 *待补*”。

处理原则：

- 仅翻译备注和注释，保留旅行商人坐标、时间段、脚本段名、`MOVE` 目标和 `待补` 状态不动。
- `GM-Teleporter.txt` 中 `MysteryCave`、`OC` 等内部段落/简称保留原样。

验证：

- 针对本批目标文件复扫旅行商人旧英文备注地点、`Serpent Valley Dead Mine`、坏按钮格式、常见坏脚本标签、字面 `` `n `` / `` `r `` 和常见乱码片段，未再命中。
- 抽查 `TravellingMerchant.txt` 表头与 `GM-Teleporter.txt` 待补段，备注显示正常，脚本结构未变。
- 服务端重启验证待补：本轮尚未执行服务端重启验证。

## 第八十四批 任务正文彩色地名显示收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/Quests` 下任务正文中的彩色地名显示 token。
- 本批只处理 `{地名/Yellow}` 这类玩家可见地名文本，保留 `/Yellow` 颜色标记。

完成内容：
- `{BichonWall/Yellow}` -> `{比奇城/Yellow}`
- `{BorderVillage/Yellow}` -> `{边境村/Yellow}`
- `{WoomyonWoods/Yellow}` -> `{沃玛森林/Yellow}`
- `{TaoVillage/Yellow}` -> `{道馆村/Yellow}`
- `{CastleGi-Ryoong/Yellow}` -> `{基隆城/Yellow}`
- `{DeathValley/Yellow}` -> `{死亡山谷/Yellow}`
- `{WoomaTemple/Yellow}` -> `{沃玛寺庙/Yellow}`
- `{SerpentValley/Yellow}` -> `{毒蛇山谷/Yellow}`
- `{MudWall/Yellow}` -> `{盟重土城/Yellow}`
- `{PrajnaIsland/Yellow}` -> `{般若岛/Yellow}`
- `{PastBichon/Yellow}` -> `{旧比奇/Yellow}`

保护边界：
- 未修改任务条件段、物品名、怪物名、NPC 名称、脚本跳转目标或文件名。
- 未处理 `{MasterShok/LimeGreen}` 等 NPC/角色名彩色引用，继续留给名称引用专项。

验证：
- 已复扫上述旧英文彩色地名 token：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤：无命中。
- 已复扫常见英文可见残留关键词：无命中。
- 服务端重启验证：待补。
## 第八十五批 任务正文彩色地点别名一致化

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/Quests` 下任务正文中的剩余 `{英文地点/Yellow}` 显示 token。
- 本批继续只处理玩家可见地点显示名，保留 `/Yellow` 颜色标记。

完成内容：
- 收口 `Bichon`、`CastleBichon`、`BichonProvince`、`BichonDeadMine(s)` 等比奇相关地点显示。
- 收口 `MongchonProvince`、`Stone Temple/StoneTemple/Stone Tomb/StoneTomb`、`SabukWall` 等盟重与石墓相关地点显示。
- 收口 `OmaCave/OmaCavern`、`InsectCave`、`TreePath`、`MineralMines` 等洞穴和野外地点显示。
- 收口 `Prajna Island/PrajnaTemple/PrajnaStoneCave/Prajna Cave` 等般若岛地点显示。
- 收口 `SerpentDeadMine(s)`、`SerpentDeadMine(s) 2F`、`Death Valley`、`Hidden Entrance(s)` 等毒蛇山谷与死亡山谷相关地点显示。
- 补齐 `Border Village/Yellow` 大小写别名。

保护边界：
- 未修改 `[KillTasks]`、`[ItemTasks]`、`[CarryItems]`、奖励物品、脚本命令或引用键。
- 未修改 `{怪物/Crimson}`、`{物品/LightSteelBlue}`、`{NPC/LimeGreen}` 等名称引用，继续留给名称引用专项。

验证：
- 已精确复扫 `{英文地点/Yellow}`：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤：无命中。
- 服务端重启验证：待补。
## 第八十六批 NPC/脚本可见英文残留扩展收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs`、`Events`、`SystemScripts` 中的低风险可见文本。
- 杂货/药店/行会领地商人脚本中的按钮显示名与说明文字。
- GM 传送器维护注释标题。
- 局部 `LocalMessage` 中的英文地点、事件、石柱与提示名。

完成内容：
- 将 `<Candle/...>`、`<Dungeonescape/...>`、`<RandomTeleport/...>`、`<RepairOil/...>`、`<Townteleport/...>` 等按钮显示侧统一为中文，保留 `/...` 跳转或命令目标。
- 将说明文字中的 `Candles`、`Dungeonescape`、`TownTeleport`、`RandomTeleport`、`RepairOil`、`Portal Scroll` 等显示词补译。
- 将仓库兑换按钮显示侧 `<GoldBar/@GBar>`、`<GoldBarBundle/@GBBundle>`、`<GoldChest/@GChest>` 改为中文显示，保留 `@GBar/@GBBundle/@GChest`。
- 收口 `Bichon Oord Event`、`Mete FlyingStatue`、`StoneTemple`、`MongchonDelegate`、`WierdPillar/StrangePillar/MysteriousPillar`、`LostSoul`、`Sabuk Gold` 等局部可见提示。
- 将 DogYoHyun 村庄占位文本 `TO COME` 改为 `内容待补`。
- 将 Seokcho Valley TrollMine 传送员文本中的 `beast` 改为中文描述。
- 翻译 `GM-Teleporter.txt` 中剩余英文区域注释标题，保留脚本段落和传送命令。

保护边界：
- 未修改商店清单中单独一行的物品键，如 `Candle`、`RepairOil` 等。
- 未修改 `CHECKITEM`、`TAKEITEM`、`GIVEITEM`、`MOVE`、`GOTO` 等命令参数。
- 未修改怪物、物品、NPC 数据名称引用，继续留给名称引用专项。

验证：
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤：无命中。
- 已复扫本批重点英文词：无残留。
- 已复扫 `GM-Teleporter.txt` 英文注释标题：无残留。
- 服务端重启验证：待补。
## 第八十七批 MirGuide 技能按钮与零散显示词收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs` 与 `SystemScripts` 中的低风险可见文本。
- MirGuide/书店技能列表中的 `<技能名 等级/@跳转>` 按钮显示侧。
- 少量制作、仓库兑换、金额单位、GM 管理和本地提示显示文本。

完成内容：
- 将战士、法师、道士、刺客、弓手技能列表按钮显示侧批量改为中文，保留 `@fencing`、`@fireball`、`@summonskel` 等跳转目标。
- 将 `Teleport (等级 19)`、`等级 19： Teleport` 等书店/说明显示文本改为 `瞬息移动`。
- 将 `<Crafting/@Craft>`、`<BlackIronOre/@Biron>`、`<Emperor/@...>` 等少量按钮显示侧改为中文，保留跳转目标。
- 收口仓库兑换说明中的 `GoldBar、GoldBarBundle、GoldChest` 显示词与手续费中的 `Gold` 单位。
- 收口 `Sabuk Wall` 管理标题、城墙修复提示、`[指挥官 Luke]`、`[Sinseok Miner]` 等可见提示文本。

保护边界：
- 未修改 `GIVESKILL`、`GIVEITEM`、`CHECKITEM` 等命令参数。
- 未修改商店清单中单独一行的物品名或技能内部键。
- 未处理宠物/怪物按钮显示名，继续留给名称引用专项。

验证：
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤：无命中。
- 已复扫本批技能按钮英文残留：无命中。
- 已复扫金额单位、仓库说明、GM 管理标题等重点残留：无命中。
- 服务端重启验证：待补。
## 第八十八批 路径误翻修正与任务注释前缀收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs`、`SystemScripts`、`Quests` 中的低风险可见文本和维护注释。
- `NewbieGuild` 相关脚本路径/名单路径一致性检查。
- 任务文件 `// Pickup -`、`// Hand In -` 注释前缀。

完成内容：
- 修正 `LevelUp.txt` 中被误翻的 include 路径，恢复为 `SystemScripts\00Default\LevelUp\NewbieGuild.txt`。
- 修正 `BountyBoard.txt` 中被误翻的名单文件路径，恢复为 `../NameLists/NewbieGuild.txt`。
- 保留玩家可见的“新手行会”文本与行会显示名。
- 将 `VulnerableSon` 系列脚本中夹杂的 `Father/Mines/Loot` 改为自然中文。
- 整理石墓、失落灵魂、石柱发现、新手行会加入等提示语中的多余空格。
- 将 GM 维护说明中的 `GM_Manager`、`GM_Teleporter` 等显示文字整理为中文描述，保留 `R001`、`@GM` 等技术标识。
- 将任务文件注释前缀 `// Pickup -`、`// Hand In -` 批量改为 `// 接取 -`、`// 交付 -`，保留后续 NPC/坐标引用。

保护边界：
- 未修改任务条件段、物品名、怪物名、NPC 内部名或脚本命令参数。
- 未翻译 `@BichonMines`、`@SerpentMines`、`@TaoMineralMines` 等跳转标签。
- 未处理宠物/怪物显示名，继续留给名称引用专项。

验证：
- 已复扫 `// Pickup -`、`// Hand In -`：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤：无命中。
- 已验证 `NewbieGuild.txt` include 与 NameLists 路径恢复到实际文件名。
- 服务端重启验证：待补。
## 第八十九批 书店等级技能说明收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs` 中书店、MirGuide、技能说明页的纯显示行。
- 仅处理 `等级 N: SkillName`、`SkillName (等级 N)` 这类玩家可见说明文本。

完成内容：
- 将战士、法师、道士、刺客、弓手技能等级说明中的英文技能名批量改为中文。
- 补齐 `Hemorrhage`、`CresentSlash`、`VamnpireShot`、`StoneTrap` 等拼写变体/漏网显示名。
- 覆盖比奇书店、边境村书店、MirGuide、般若岛/毒蛇山谷/盟重等多处复用说明页。

保护边界：
- 未修改 `GIVESKILL`、`CHECKSKILL` 等命令参数。
- 未修改物品、怪物、宠物或 NPC 名称引用。
- 未处理宠物购买列表和价格表中的英文名，继续留给名称引用专项。

验证：
- 已复扫 `等级 N: 英文技能名` 与 `英文技能名 (等级 N)`：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 已复扫中文技能名进入 `GIVESKILL` 命令的误改风险：无命中。
- 服务端重启验证：待补。
## 第九十批 普通对话夹英文残留收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/BookStore.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/BookStore-0104.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Far.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/Far-0122.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/StrangeMan-D002.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/OmaCave/StrangeMan.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/CaveGuard.txt`

完成内容：
- 将书店与 Far 任务对话中的 `Sir Mogu` 显示文本统一为“莫古爵士”。
- 将 StrangeMan 对话中的 `OmaCave`、`BoneElite` 叙述句改为“半兽洞穴”“骸骨精英”。
- 将 CaveGuard 对话中的 `Cave` 改为“洞穴”。

保护边界：
- 仅处理 `#SAY` 中的玩家可见自然语言句子。
- 未修改 `@THX`、`@exit`、`CHECK`、`SET`、`GOTO` 等脚本标签和命令。
- `BoneElite` 只在普通叙述文本中显示为中文，未批量修改怪物内部名称或刷新/任务引用。

验证：
- 已复扫 `Sir Mogu`、`OmaCave 里`、`BoneElite 的强大怪物`、`从 Cave 深处`：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第九十一批 MirGuide 技能说明术语收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide-0.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide-3.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuidePI.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide-2.txt`

完成内容：
- 将技能说明正文中的 `MP`、`HP`、`Poison`、`amulet` 统一为“魔法值”“生命值”“毒药”“符咒”。
- 将 `Fireball`、`Blizzard`、`Hiding`、`TrapHexagon`、`Shinsu`、`HolyDeva` 等可见技能/召唤物说明统一为中文。
- 将 `Amulet of Revival`、`Grey Poison` 等需求说明改为“复活护符”“灰色毒药”。
- 收口弓手技能说明中的 `Vampire`、`VampBuff`、`Cripple Shot`、`CrippleShot`、`OneWithNature` 等效果名显示。

保护边界：
- 仅处理 `MirGuide*.txt` 帮助页中的玩家可见说明文本。
- 未修改 `@fireball`、`@traphex`、`@summonshin`、`@onewithnature` 等跳转标签。
- 未修改 GM 脚本、测试脚本中的 `GIVESKILL` 命令和技能内部键。
- 保留 `DC`、`MC`、`SC` 等属性缩写。

验证：
- 已复扫本批目标英文术语与替换后多余空格特征：无命中。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第九十二批 MirGuide 狩猎与采集说明显示名收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide-0.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide-3.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuidePI.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide-2.txt`

完成内容：
- 将狩猎指南中的低等级怪物/动物显示名改为中文，如 `Hen`、`Deer`、`Sheep`、`HookingCat`、`RakingCat`、`OmaFighter`、`CaveMaggot`、`Zombie` 等。
- 将采集说明中的 `Meat`、`PickAxe` 等显示词改为“肉”“鹤嘴锄”。
- 将 `Hellfire`、`Wizard`、`Thunderbolt`、`Marian Province` 等帮助文本中的普通说明词改为中文。
- 收紧批量替换后的中文排版空格。

保护边界：
- 仅处理 `MirGuide*.txt` 中帮助页正文的玩家可见说明。
- 未修改怪物、物品、技能、地图的数据库名称。
- 未修改任务击杀目标、掉落表、刷新配置或脚本命令参数。
- 保留跳转标签，如 `@slicemeat`、`@level1-11`、`@hellfire`、`@tbolt`。

验证：
- 已复扫本批目标英文显示名：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第九十三批 MirGuide 中文排版残留收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall/MirGuide-0.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall/MirGuide-3.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/MirGuidePI.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/MirGuide-2.txt`

完成内容：
- 收口前几批批量替换后的中文空格残留，如“死去的 僵尸”“基础 火球术 的”“到 玛丽安省 中”。
- 收紧弓手技能效果说明中的中文技能名空格，如“使 致残射击产生”“使 致残射击命中”。
- 抽样确认狩猎说明、法师说明、弓手说明段落显示自然。

保护边界：
- 仅处理 `MirGuide*.txt` 帮助页正文中的中文排版。
- 未修改跳转标签、脚本命令、技能内部键或数据库名称。
- 保留 `Mir`、`Alt`、`Ctrl`、`B`、`SC` 等品牌/快捷键/属性缩写。

验证：
- 已复扫本批中文空格残留特征：无命中。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第九十四批 任务进度与悬赏提示显示名收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/Board/1.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/Board/2.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/MudWall/Board/1.txt`
- `Build/Server/Debug/Envir/Quests/AncientCaves/OmaCavern/2.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/26.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/StoneTomb/4.txt`

完成内容：
- 将悬赏提交提示中的 `BichonWall`、`MudWall` 改为“比奇城”“盟重土城”。
- 将任务旗标提示中的 `OmaCavern`、`MysteriousStone`、`Stone Temple`、`Emperor` 改为中文显示。
- 将 `BoarTooth` 行中引号内的显示说明 `收集 Boar Tooth` 改为“收集野猪牙”，保留行首物品键 `BoarTooth 75`。

保护边界：
- 仅处理任务描述、旗标提示和引号内玩家可见说明。
- 未修改 `ItemTasks` 行首物品键、任务文件名、NPC 注释引用或命令字段。
- 保留 `{Boar Tooth/LightSteelBlue}` 等颜色引用中的物品名，继续留给名称引用专项。

验证：
- 已复扫本批目标旧英文提示：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第九十五批 任务正文人名与旗标说明收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/Quests/BichonProvince/BorderVillage/17.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/1.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/2.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/SabukWall/4.txt`

完成内容：
- 将任务正文中的 `Assistant Jane` 显示为“助手简”。
- 将 `HolySword` 在任务正文和任务目标中显示为“神圣之剑”。
- 将 `TrainerTaoist`、`School's Owner` 的任务目标显示为“道士导师”“道场主人”。
- 将 `TreePath`、`RootSpider` 在任务描述和引号内击杀说明中显示为“林间小径”“根蛛”，保留行首怪物键 `RootSpider 1`。
- 将 `WierdPillar`、`StrangePillar`、`MysteriousPillar` 的旗标提示显示为“奇异石柱”“奇怪石柱”“神秘石柱”。

保护边界：
- 仅处理任务正文、任务目标和引号内玩家可见说明。
- 未修改怪物/物品/NPC 内部键、文件名、坐标注释或颜色引用中的名称。
- `RootSpider 1` 等行首任务匹配键保持英文。

验证：
- 已复扫本批目标旧英文显示词：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第九十六批 任务正文与击杀说明显示名收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/8.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BorderVillage/9.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BorderVillage/12.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BorderVillage/15.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/BorderVillage/18.txt`
- `Build/Server/Debug/Envir/Quests/BichonProvince/OmaCavern/2.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/3.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/4.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/5.txt`
- `Build/Server/Debug/Envir/Quests/HolySword/7.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/Bug Cave/3.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/SabukWall/2.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/SabukWall/5.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/StoneTomb/4.txt`
- `Build/Server/Debug/Envir/Quests/PrajnaIsland/PranjaTemple/1.txt`
- `Build/Server/Debug/Envir/Quests/PrajnaIsland/PranjaTemple/2.txt`
- `Build/Server/Debug/Envir/Quests/PrajnaIsland/PranjaTemple/3.txt`
- `Build/Server/Debug/Envir/Quests/SerpentValley/Village/16.txt`
- `Build/Server/Debug/Envir/Quests/WasteLand/Red Cave/1.txt`
- `Build/Server/Debug/Envir/Quests/WoomyonWoods/TaoistVillage/WW Mines/1.txt`

完成内容：
- 将任务正文中的 `Mirian`、`Peter`、`Perry`、`Abel` 等显示文本改为中文。
- 收口 `HolySword` 支线中的 `EvilApeOil`、`RedMoonSword`、`RedMoonEvil`、`EvilHeart`、`JadeCrystal`、`BigTaoist` 等普通叙述/旗标说明。
- 将 `Minotaur`、`LeftGuard`、`RightGuard`、`LostSoul`、`CrawlerZombies` 等可见说明改为中文。
- 将 `CursedZombie`、`ShiZombie`、`HungryZombie` 的引号内击杀说明改为中文，保留行首任务匹配键。

保护边界：
- 未修改 `KillTasks`、`ItemTasks` 行首怪物/物品键。
- 未修改颜色引用中的名称，如 `{EvilApeOil/LightSteelBlue}`、`{CrawlerZombies/Crimson}`。
- 未修改 NPC 坐标注释、文件名、地图内部名或数据库名称。

验证：
- 已复扫本批目标旧英文显示词；仅剩颜色引用和行首任务键，均为保留项。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第九十七批 NPC 商店与管理提示显示词收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall` 中公告牌、法尔、杂货商、仓库、无限斗场老人和居民列表。
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage` 中小贩与仓库镜像。
- `Build/Server/Debug/Envir/NPCs/PastBichon`、`PrajnaIsland`、`MongchonProvince/SabukWall`、`SerpentValley/Village`、`WasteLand`、`WoomyonWoods/Castle-GI` 中的小贩/杂货商镜像。
- `Build/Server/Debug/Envir/NPCs/GM/GM-Manager.txt`

完成内容：
- 将 `Randomteleport` 卷轴说明统一为“随机传送卷”。
- 将仓库兑换失败提示中的 `GoldBar`、`GoldBarBundle`、`GoldChest` 显示为“金条”“金条包”“金币箱”。
- 将 `Damian`、`Mogu`、`Commander Luke`、`Brown Chestnuts` 等普通显示文本改为中文。
- 将 GM 面板中的 `Game Master`、`SabukWall`、`SealedHero` 显示为“游戏管理员”“沙巴克”“封印英雄”。

保护边界：
- 未修改 `TAKEITEM GoldBar`、`CHECKITEM GoldBarBundle`、`GIVEGOLD` 等命令参数。
- 未修改跳转标签，如 `@randomteleport`、`@GBBundle`、`@GChest`。
- 未处理宠物购买列表、颜色引用中的怪物名和纪念碑/外部链接文本。

验证：
- 已复扫本批目标旧英文显示词：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第九十八批 NPC 通用说明词与探索文本收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/PastBichon` 中制作、铁匠、杂货、商人和剧情 NPC。
- `Build/Server/Debug/Envir/NPCs/SerpentValley` 中神秘余、骷髅堆和村庄制作/杂货 NPC。
- `Build/Server/Debug/Envir/NPCs/WasteLand` 中制作、探索者和杂货/小贩 NPC。
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/OddOldMan.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/SabukWall/SwampNPC.txt`
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/Swamp/Transporter1.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/LeadTrainerTV.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Manager.txt`
- `Build/Server/Debug/Envir/NPCs/GM/GM-Quest.txt`

完成内容：
- 将 `monsters`、`Monsters`、`Recipe`、`BlackIronOre` 等普通说明词改为中文显示。
- 将探索对话中的 `cave`、`caves`、`certificate`、`dungeon`、`unknown dungeon` 改为“洞穴”“通行证”“地下城”“未知地下城”。
- 将骷髅堆文本中的 `skeleton pile`、`note`、`book`、`scroll` 改为中文。
- 将过去比奇剧情中的 `Omas/omas` 改为“半兽人”。
- 将残留问候中的 `Mirian` 改为“冒险者”，`Dark Swamp` 改为“黑暗沼泽”。
- 将 GM 任务页中的 `Shard` 显示为“碎片”，GM 统计说明中的 `Sabuk` 显示为“沙巴克”。

保护边界：
- 未修改颜色引用中的物品/怪物名，如 `{BlackIronOre/LightSteelBlue}`。
- 未修改命令参数、跳转标签、数据列表和外部链接。
- 未处理纪念碑专名、宠物购买列表和行会领地内部功能词。

验证：
- 已复扫本批目标旧英文显示词：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第九十九批 行会领地与沙巴克管理显示词收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/PastBichon` 中行会领地商人脚本。
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland` 中行会领地商人脚本。
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall` 中行会领地、传送和仓库脚本。
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/SabukWall` 中铁匠与沙巴克征服管理脚本。
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Event` 中 MissDo/MissMi/MissRe 引导文本。
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall`、`Build/Server/Debug/Envir/NPCs/GuildTerritory` 中已出现的行会领地管家显示词。

完成内容：
- 将 `GT Steward`、`Armed Transporter` 改为“行会领地管家”“武装传送员”。
- 将行会领地交易说明中的 `note`、`trade`、`kicked out` 改为“便条”“交易”“踢出”。
- 将沙巴克管理页中的 `SabukWall`、`Sabuk 税率` 等显示词改为中文。
- 将普通问候中的 `Edwin`、`Jessica`、`Dean`、`Jude`、`Miss Do`、`MissRe`、`MissMi` 改为中文显示名。
- 收紧替换后的中文句子空格，避免“寻找 武装传送员”等排版残留。

保护边界：
- 未修改 `[@trade]`、`@tax`、`GLOBALMESSAGE` 参数结构和行会领地交易命令。
- 未修改物品、怪物、NPC 数据库名称和颜色引用中的内部键。
- 未处理纪念碑、外部链接和装备/怪物名称专项。

验证：
- 已用文本扫描器复扫本批目标旧英文显示词和替换后多余空格：无残留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百批 任务花括号显示引用第一批

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/Quests/BichonProvince` 中边境村、比奇城墙、废矿和半兽洞窟任务。
- `Build/Server/Debug/Envir/Quests/WoomyonWoods` 中沃玛森林、城堡镇、道士村和昆虫洞穴任务。
- `Build/Server/Debug/Envir/Quests/SerpentValley` 中村庄任务。
- `Build/Server/Debug/Envir/Quests/MongchonProvince`、`Build/Server/Debug/Envir/Quests/PrajnaIsland`、`Build/Server/Debug/Envir/Quests/HolySword`、`Build/Server/Debug/Envir/Quests/AncientCaves` 中命中的同类任务引用。

完成内容：
- 将任务正文和任务描述中的花括号显示引用改为中文，如 `{Oma/Crimson}` -> `{半兽人/Crimson}`、`{WoomaWarrior/Crimson}` -> `{沃玛勇士/Crimson}`。
- 将常见 NPC 显示引用改为中文，如 `{Assistant Jane/LimeGreen}`、`{MongchonScout/LimeGreen}`、`{MasterShok/LimeGreen}`。
- 将常见物品显示引用改为中文，如 `{BugEye/LightSteelBlue}`、`{PrajnaHistory/LightSteelBlue}`、`{JadeCrystal/LightSteelBlue}`。
- 同步处理部分拼写变体显示引用，如 `BluHoroBlaster`、`VoiletKekTal`、`WoomaGaurdian`。

保护边界：
- 只修改 `{显示名/颜色}` 的显示名部分，保留颜色标记。
- 未修改 `[@KillTasks]`、`[@ItemTasks]`、奖励列表、脚本注释中的 NPC 文件名和坐标。
- 未修改数据库物品名、怪物名、NPC 名和任务内部执行键。

验证：
- 已复扫本批目标花括号英文显示引用：无残留。
- 抽查任务样例，确认 `KillTasks`、`ItemTasks` 中英文内部键仍保留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百零一批 任务花括号显示引用第二批

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/Quests/BichonProvince` 中比奇城墙、边境村、公告板等剩余任务。
- `Build/Server/Debug/Envir/Quests/MongchonProvince` 中虫洞、石墓、沙巴克城墙、盟重土城任务。
- `Build/Server/Debug/Envir/Quests/SerpentValley` 中村庄与毒蛇废矿任务。
- `Build/Server/Debug/Envir/Quests/PrajnaIsland` 中般若村、般若石窟、般若神殿任务。
- `Build/Server/Debug/Envir/Quests/WoomyonWoods`、`Build/Server/Debug/Envir/Quests/HolySword`、`Build/Server/Debug/Envir/Quests/WasteLand` 中命中的同类任务引用。

完成内容：
- 继续将任务正文和任务描述中的 `{显示名/颜色}` 改为中文，覆盖赤色洞穴、般若神殿、石墓、毒蛇废矿、死亡山谷、比奇早期任务等区域。
- 将剩余常见怪物显示名改为中文，如赤月恶魔、梦境吞噬者、牛头怪、骷髅、蛇类、蜘蛛类、僵尸类、野猪类和虫洞怪物。
- 将剩余常见 NPC 显示名改为中文，如佩里、工艺师小姐、首席训练师、村长、金医生、布商山姆、屠夫约翰等。
- 将剩余常见物品显示名改为中文，如血丸、旧项链、隐藏卷轴、亡者之心、秘密配方、蝙蝠翅膀、骷髅骨、生命石等。

保护边界：
- 只修改 `{显示名/颜色}` 的显示名部分，保留颜色标记。
- 未修改 `KillTasks`、`ItemTasks`、奖励列表、NPC 文件名、地图坐标和任务执行键。
- 未处理 NPC 宠物购买清单、装备数据库名称和脚本命令参数。

验证：
- 已复扫任务目录中英文 `{显示名/Crimson|LightSteelBlue|LimeGreen}` 引用：无残留。
- 抽查赤色洞穴、石墓、毒蛇废矿任务样例，确认内部英文任务键仍保留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百零二批 NPC 彩色显示引用收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/AncientCaves`、`MongchonProvince/StoneTemple`、`MongchonProvince/ZumaTemple`、`WoomyonWoods/WoomaTemple` 中古代入口石碑需求显示。
- `Build/Server/Debug/Envir/NPCs/BichonProvince`、`PrajnaIsland`、`WhiteValley` 中船夫、理发、行会、仓库、寄售说明里的金币显示。
- `Build/Server/Debug/Envir/NPCs/MongchonProvince`、`PastBichon`、`SerpentValley`、`WoomyonWoods` 中屠夫、仓库、封印入口和剧情提示。
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage` 中圣剑与根须蜘蛛提示。

完成内容：
- 将古代入口需求中的 `ZumaHeart`、`WoomaHeart`、`StoneHeart`、`PrajnaHeart` 显示为中文。
- 将 NPC 费用、仓库兑换、寄售保证金等 `{Gold/...}` 显示统一为“金币”。
- 将屠夫说明中的 `Hens`、`Deer`、`Sheep`、`Wolves` 显示为中文。
- 将 `HolySword`、`DragonScale`、`TimeStonePiece`、`ScrollOfSeal`、`BlackIronOre`、`Rootspider/rootspider` 等可见引用改为中文显示。

保护边界：
- 未修改行会领地宠物购买清单 `GTPetNPC.txt` 中的宠物显示名，因为它们与 `GIVEPET`、`CHECKPET`、宠物令牌等内部键强绑定。
- 未修改 `CHECKITEM`、`TAKEITEM`、`GIVEPET`、`CHECKPET`、跳转标签和脚本命令参数。
- 仅替换 `{显示名/颜色}` 的显示名部分，保留颜色标记。

验证：
- 排除 `GuildTerritory/GA*/GTPetNPC.txt` 后，已复扫 NPC、事件、系统脚本中的英文彩色显示引用：无残留。
- 抽查古代入口与船夫费用样例，显示名已中文化，内部命令仍保留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百零三批 向导与时间石可见文本收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/*/MirGuide*.txt` 中各区域向导说明。
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/OddOldMan.txt`
- `Build/Server/Debug/Envir/Quests/MongchonProvince/StoneTomb/5.txt`

完成内容：
- 将向导文本中的 `Mir 世界` 统一改为“玛法世界”。
- 将石墓任务正文中的 `Ancient Layer` 改为“古代层”。
- 将奇怪老人界面中的 `TimeStone`、`TimeStonePiece` 可见文本改为“时间石”“时间石碎片”。

保护边界：
- 未修改 `CHECKITEM TimeStonePiece`、`TAKEITEM TimeStonePiece`、`GIVEITEM TimeStonePiece` 等内部命令。
- 未处理 WasteLand 古代装备锻造脚本中的 `AncientHeavyArmour` 等装备名。
- 未处理行会领地宠物脚本中的宠物名、宠物令牌和 `GIVEPET/CHECKPET` 联动内容。

验证：
- 已复扫本批目标旧英文显示词：无残留。
- 抽查奇怪老人脚本，确认时间石内部物品键仍保留。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百零四批 金币颜色显示变体收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BichonWall` 中仓库兑换与彩票脚本。
- `Build/Server/Debug/Envir/NPCs/BichonProvince/BorderVillage` 中仓库兑换脚本。
- `Build/Server/Debug/Envir/NPCs/MongchonProvince/MudWall` 中仓库兑换与彩票脚本。
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/Castle-GI` 中仓库兑换脚本。

完成内容：
- 将遗留的 `{Gold/GOLD}` 显示统一改为 `{金币/GOLD}`。
- 覆盖仓库“金条/金条包/金币箱兑换为金币”说明和彩票金额说明。

保护边界：
- 保留颜色标记 `GOLD`。
- 未修改 `GOLD`、`Gold` 相关脚本命令、物品键、交易列表和兑换跳转标签。
- 未处理古代装备、宠物购买清单、纪念碑专名和测试服品牌名。

验证：
- 已复扫 `{Gold/GOLD}`：无残留。
- 抽查仓库兑换与彩票脚本，金币显示已中文化。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百零五批 皇帝任务显示与颜色标记修正

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/Quests/BichonProvince/BichonWall/26.txt`
- `Build/Server/Debug/Envir/NPCs/AncientCaves/AncientWooma-D023.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/WoomaTemple/Stone.txt`

完成内容：
- 将任务描述中的 `{Emperor/LIMEGREEN}` 显示改为 `{皇帝/LIMEGREEN}`。
- 修正两处古代沃玛石碑脚本的颜色标记拼写：`KHAI` -> `KHAKI`。

保护边界：
- 保留 `@Emperor` 跳转目标、任务段落标签和任务条件。
- 保留沃玛之心物品显示引用、等级文本和传送跳转。
- 未处理物品名、怪物名、宠物名、交易清单和外部纪念专名。

验证：
- 已复扫 `{Emperor/LIMEGREEN}` 与 `KHAI`：无残留。
- 抽查任务文件与两处石碑脚本，中文显示和颜色标记均正常。
- 已复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百零六批 比奇省乱码 NPC 与白色山谷对话收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/BichonProvince/ArcherCaptain.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/ArcherCaptain-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/ArcherMage.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/ArchMage-0115.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/Cloud-0.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/HighPriest.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/HighPriest-0.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Captain.txt`

完成内容：
- 修复比奇省 7 个 NPC 文件中的可见乱码对话和关闭/确定按钮。
- 将白色山谷队长对话中的 `creatures`、`SnowCavern`、`Ancient Creature` 显示改为中文。

保护边界：
- 保留 `[Quests]` 段落和任务编号。
- 保留 `@exit`、`@Exit` 等跳转目标。
- 保留 GM 传送器中的 `@SnowCavern` 标签和跳转目标。
- 未处理压缩包 `GM.rar`，不将二进制内容纳入脚本文本翻译。

验证：
- 已复扫比奇省乱码特征与白色山谷英文残留：无残留。
- 已确认 `SnowCavern` 仅剩 GM 传送器标签/目标。
- 已排除 `*.rar` 后复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百零七批 圣剑传说对话显示收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/6Wrt.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Perry.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/9Wbt.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/BigTaoist.txt`
- `Build/Server/Debug/Envir/NPCs/BichonProvince/OmaCave/Grim.txt`

完成内容：
- 将道士村圣剑传说对话中的 `HolySword`、`Holy Sword` 显示统一为“圣剑”。
- 清理替换后残留的中英文间空格，使句子显示更自然。
- 将奥玛洞窟服装商对话入口中的 `CastleBichon` 显示改为“比奇城”。

保护边界：
- 保留 `@Quest`、`@next_quest`、`@talk` 等跳转目标。
- 未修改 `RedMoonEvil`、`RedMoonSword`、`Redmoon Valley`、`Tree path` 等名称联动项。
- 未修改任务击杀键、怪物名、物品名和地图内部名。

验证：
- 已复扫 `HolySword`、`Holy Sword`、`CastleBichon`：无残留。
- 已复扫“圣剑”前后异常空格：无残留。
- 已排除 `*.rar` 后复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百零八批 圣剑剧情赤月专名显示收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/6Wrt.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Perry.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/9Wbt.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/BigTaoist.txt`

完成内容：
- 将圣剑剧情对话中的 `TreePath`、`Tree path` 显示改为“树径”。
- 将普通对话中的 `RootSpider` 显示改为“根须蜘蛛”。
- 将 `RedMoon Valley`、`Redmoon Valley`、`Redmoon valley` 显示统一改为“赤月山谷”。
- 将对话中的 `RedMoonEvil` 显示改为“赤月恶魔”。
- 将对话中的 `RedMoonSword` 显示改为“赤月魔剑”。
- 将大道士对话中的 `RedMoonChip` 显示改为“赤月碎片”。
- 清理替换后残留的中英文间空格。

保护边界：
- 保留任务 `KillTasks`、`CarryItems`、`FixedRewards` 中的 `RootSpider`、`RedMoonChip`、`RedMoonEvil`、`RedMoonSword`。
- 保留 GM 清单、传送器标签和系统脚本中的 `@TreePath`、`@RedMoonEvil`、`RedMoonSword`、`RedMoonChip`。
- 暂未处理 `JadeCrystal`、`JadeStick`、`RedEvilApe`，这些继续留给名称引用专项。

验证：
- 已复扫本批显示残留：NPC 对话中无 `RedMoonEvil`、`RedMoonSword`、`RedMoon Valley`、`Redmoon Valley`、`Tree path`、`TreePath`、`RootSpider`、`RedMoonChip` 残留。
- 已确认残留英文仅位于任务键、GM 清单、脚本标签或待名称联动项。
- 已排除 `*.rar` 后复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百零九批 圣剑剧情翡翠与红猿显示收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/6Wrt.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/Perry.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/9Wbt.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/BigTaoist.txt`

完成内容：
- 将圣剑剧情对话中的 `JadeCrystal` 显示改为“翡翠水晶”。
- 将说明句中的 `JadeStick` 显示改为“翡翠杖”。
- 将大道士对话中的 `RedEvilApe` 显示改为“红色邪恶猿人”。
- 将 `Thunderman` 绰号显示改为“雷霆者”。
- 清理替换后残留的中英文间空格。

保护边界：
- 保留任务 `ItemTasks` 中的 `JadeCrystal`。
- 保留 GM 清单与掉落表中的 `JadeCrystal`。
- 保留人物专名 `Abel`，未做音译。

验证：
- 已复扫 `WoomyonWoods/TaoistVillage` 目标 NPC：无 `JadeCrystal`、`JadeStick`、`RedEvilApe`、`Thunderman` 残留。
- 已确认残留 `JadeCrystal` 仅位于任务键、GM 清单和掉落表。
- 已排除 `*.rar` 后复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第一百一十批 宽扫可见英文显示收口

日期：2026-06-05

处理范围：
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/14Plv-12.txt`
- `Build/Server/Debug/Envir/NPCs/WoomyonWoods/TaoistVillage/TreePath/Pillar.txt`
- `Build/Server/Debug/Envir/NPCs`
- `Build/Server/Debug/Envir/Quests`
- `Build/Server/Debug/Envir/Events`
- `Build/Server/Debug/Envir/SystemScripts`

完成内容：
- 扩大扫描范围，复查 NPC、任务、事件和系统脚本中的可见英文残留。
- 将两处石柱入口按钮中的 `Ancient Post Piece` 显示改为“古代柱片”。
- 将宽扫命中的剩余英文按类型重新分类：任务键、GM 清单、事件刷怪命令、传送标签、测试服品牌或名称引用专项。

保护边界：
- 保留 `@next2` 跳转目标。
- 未修改任务键、掉落键、GM 清单、事件 `MonGen`/`MONGEN` 参数和传送器标签。
- 未修改 `Crystal Mir` 测试服品牌显示。

验证：
- 已复扫 `Ancient Post Piece`：无残留。
- 已确认本轮宽扫命中的剩余英文位于任务键、GM 清单、事件刷怪命令或传送标签。
- 已排除 `*.rar` 后复扫脚本坏标签、乱码特征、尖括号跳转误伤、连续问号：无命中。
- 服务端重启验证：待补。
## 第五阶段结束记录

日期：2026-06-05

结论：
- 第五阶段 NPC、任务、事件与系统脚本的自然语言汉化工作到此收口。
- 本阶段以 `Build/Server/Debug/Envir` 运行目录为准，重点处理玩家可见文本。
- 已完成公告、任务正文、NPC 对话、传送/仓库/商人/行会/活动/系统脚本等可见文本的大范围汉化与多轮复扫。
- 最后一轮宽扫确认：剩余英文主要位于任务键、GM 清单、事件刷怪命令、传送标签、测试服品牌或数据库名称引用链路。

阶段边界：
- 不在第五阶段继续修改 `ItemInfo.Name`、`MonsterInfo.Name`、`NPCInfo.Name`。
- 不在第五阶段继续修改掉落表、配方、交易清单、GM 清单、任务键、事件刷怪命令和传送标签中的英文内部名。
- 名称引用联动汉化转入下一阶段单独处理。
- `Crystal Mir` 测试服品牌显示暂时保留。

下一阶段移交：
- 下一阶段目标调整为“名称引用联动汉化”。
- 重点处理物品名、怪物名、NPC 名、地图/传送点名在数据库、脚本、任务、掉落表、配方、交易清单、GM 清单和配置文件中的同步修改。
- 已有名称审计材料可继续沿用：`docs/localization-phase-5-name-candidates.tsv`、`docs/localization-phase-5-name-references*.tsv`、`docs/localization-phase-5-name-linkage.zh-CN.md`。

最终验证：
- 已完成文本级坏标签、乱码特征、尖括号跳转误伤、连续问号复扫：无命中。
- 已完成宽扫可见英文分类：剩余项均归入内部引用、品牌保留或下一阶段名称联动。
- 服务端重启验证：本阶段最终收口未补跑，留待下一阶段开始前统一做一次基线验证。
## 第三十一批 WasteLand 古代装备锻造脚本

更新时间：2026-06-04

本批处理 WasteLand 剩余的古代装备锻造长脚本：

- `Build/Server/Debug/Envir/NPCs/WasteLand/20Qah-NAMMAND_1.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Unused.txt`

覆盖内容：

- craft NPC 问候、制作入口、关闭/离开按钮。
- 可锻造物品列表、锻造费用说明。
- AncientHeavyArmour、AncientRobe、AncientPlate、AncientSuit、AncientLeather 的防御属性说明、锻造所需物品标题、立即锻造/返回按钮。
- 材料不足提示与各物品锻造成功 `LINEMESSAGE`。

处理原则：

- 保留 `CHECKGOLD`、`CHECKITEM`、`TAKEGOLD`、`TAKEITEM`、`GIVEITEM`、`GIVEitem`、`GENDER`、`GOTO`、`BREAK`、`CLOSE` 等脚本命令。
- 保留所有物品名、数量、性别后缀、属性缩写和跳转标签，如 `AncientHeavyArmour(M)`、`RustyArmour`、`ArmourBook(DC)`、`@arm12`。
- 保留原脚本中既有的 `TAKEITEM ArmourBookSC) 12` 与 `GIVEitem AncientPlate(M) 1` 拼写/大小写异常，不在翻译批次中修正逻辑。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `Hello traveler`、`Craft Items`、`Close/@`、`The following`、`Forge Now`、`Back/@`、`Defense stats`、`Items required for forging`、`ingredients`、`You have forged`、`It will cost` 等旧英文显示文本，未再命中。
- `20Qah-NAMMAND_1.txt` 与 `Unused.txt` 内容保持一致。
- 针对整个 `WasteLand` 目录扫描明显旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，`Logs/Server/Server (04-06-2026).log` 显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (04-06-2026).log` 为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第三十批 WasteLand 基础商人与短 NPC

更新时间：2026-06-03

本批开始处理 WasteLand 区域，优先翻译基础商人、镜像商人、DeadForest 商人和短说明 NPC：

- `Build/Server/Debug/Envir/NPCs/WasteLand/Weaponsmith.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/2Hvwe-HELL00.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Clothes.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/3Hvdr-HELL00.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Jewellery.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/8hvac-HELL00.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Potion.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/4Hvdu-HELL00.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Peddlar.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Grocery-HELL00.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/ABst-NAMMAN.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/ABdu-NAMMAN.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/DeadForest/Peddlar.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/DeadForest/Potion.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Soldier.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/BSoldier-HELL00.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Excavenger.txt`
- `Build/Server/Debug/Envir/NPCs/WasteLand/Excavenger-HELL00.txt`

覆盖内容：

- 武器、服装、首饰、药水、杂货商和 DeadForest 商人的问候、买卖、回购、修理与物品说明。
- 杂货商的 Candle、Dungeonescape、RandomTeleport、RepairOil、TownTeleport 说明。
- Soldier / BSoldier 的临时基地提示。
- Excavenger 的 cave / Evil / reward 短说明。

处理原则：

- 保留 `[Trade]`、`[Types]` 数据段及物品名。
- 保留 `@BuySell`、`@BuyBack`、`@Repair`、`@Ask`、`@candle`、`@dungeonescape`、`@randomteleport`、`@repairoil`、`@townteleport` 等脚本入口。
- 保留 `Candle`、`Dungeonescape`、`RandomTeleport`、`RepairOil`、`TownTeleport`、`Monsters`、`caves`、`Evil` 等名称引用。
- 统一本批目标文件为 UTF-8 BOM。
- `20Qah-NAMMAND_1.txt` 与 `Unused.txt` 为古代装备锻造长脚本，本批暂不混入，后续单独处理。

验证：

- 针对本批文件扫描 `I will not help`、`Close/@`、`Hello`、`Welcome`、`Which`、`Would you`、`Back/@`、`Buy Back`、`View/@`、`Repair/@`、`Store.`、`Ask/@`、`RepairOil makes`、`TownTeleport scrolls`、`temporary base`、`Have you ventured` 等旧英文显示文本，未再命中。
- 针对整个 `WasteLand` 目录扫描后，剩余明显旧英文集中在 `20Qah-NAMMAND_1.txt` 与 `Unused.txt` 两个古代装备锻造长脚本。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第二十九批 SerpentValley 任务入口与 Seokcho TrollMine 传送

更新时间：2026-06-03

本批处理 SerpentValley 任务入口类 NPC，并顺手收口 SeokchoValley / TrollMine 的传送员：

- `Build/Server/Debug/Envir/NPCs/SerpentValley/ConnectionPath/MysteriousYu.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/MysteriousYu-E603.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/SDeadMine/SkullPile.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/SkullPile-D4221.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/VulnerableSon.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/VulnerableSon-2.txt`
- `Build/Server/Debug/Envir/NPCs/SeokchoValley/TrollMine/Transporter.txt`

覆盖内容：

- MysteriousYu 的周末 unknown dungeon 入口提示、certificate 条件、进入/等待/返回对话。
- SkullPile 的 note/scroll/book 提示、ScrollOfSeal 缺失提示和进入按钮。
- VulnerableSon 的求救短句。
- TrollMine Transporter 的第 4 层 beast 传闻、准备确认和传送按钮。

处理原则：

- 保留 `DAYOFWEEK`、`HOUR`、`MIN`、`MOVE`、`CHECKITEM`、`TAKEITEM` 等脚本命令。
- 保留 `@talkwith_*`、`@untalkwith`、`@Enter`、`@transport`、`@yes`、`@no`、`@Exit` 等跳转标签。
- 保留 `unknown dungeon`、`certificate`、`dungeon`、`cave`、`ScrollOfSeal`、`Father`、`Mines`、`Loot`、`beast` 等名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `What brought`、`Talk with`、`good information`、`If you got`、`Get in`、`Have you ever`、`Back/@`、`Hmm`、`Read/@`、`From another`、`I should`、`You need to have`、`Stranger`、`Have you heard`、`rumours`、`Please let me know` 等旧英文显示文本，未再命中；剩余命中为保留名称引用。
- 针对 `SerpentValley` 与 `SeokchoValley` 扫描明显旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第二十八批 SerpentValley 村庄基础功能 NPC

更新时间：2026-06-03

本批开始处理 SerpentValley / Village 区域的基础功能 NPC 和镜像文件：

- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Blacksmith.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Blacksmith-2.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Clothes.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Clothes-2.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Potion.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Potion-2.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Grocery.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Grocery-2.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Warehouse-2.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Collector.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Crafting.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/11Scft.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Transport.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/SValley_Transport-2.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Signpost.txt`
- `Build/Server/Debug/Envir/NPCs/SerpentValley/Village/Signpost-2.txt`

覆盖内容：

- 武器、服装、药水、杂货、仓库、收购商、制作女士的问候、买卖、回购、修理、仓库/包裹、材料说明和制作入口。
- 杂货商的 Candle、Dungeonescape、RandomTeleport、RepairOil、TownTeleport 说明。
- Chandler 传送员的服务说明、目的地列表和 gold 不足提示。
- 村庄路牌的商店传送入口与目标按钮。

处理原则：

- 保留 `[Trade]`、`[RECIPE]`、`[Types]`、`[Quests]` 数据段及物品/配方/任务编号。
- 保留 `@BuySell`、`@BuyBack`、`@Repair`、`@Storage`、`@SendParcel`、`@CollectParcel`、`@Craft`、`@tele`、`@Border`、`@BichonWall`、`@MudWall`、`@TaoVillage`、`@CastleGi`、`@Camp`、`@Weapon`、`@reagent` 等脚本入口。
- 保留 `Chandler`、`Candle`、`RepairOil`、`TownTeleport`、`Monsters`、`BorderVillage`、`BichonWall`、`MudWall`、`TaoistSchool`、`CastleGi-Ryoong`、`WoomyonCamp`、`gold` 等名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `I will not help`、`Close/@`、`Hello`、`Welcome`、`Which`、`Would you`、`Back/@`、`Buy Back`、`View/@`、`Repair/@`、`Access/@`、`Use.`、`teleport to village`、`wandering warrior`、`Which place`、`Recipe you`、`RepairOil makes`、`TownTeleport scrolls` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第二十七批 WhiteValley 技能书、入口与剧情短 NPC

更新时间：2026-06-03

本批继续处理 WhiteValley 剩余可见文本：

- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Book.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Captain.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/CaveGuard.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/SnowMonument.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Wanderer_Charlotte.txt`

覆盖内容：

- 技能书商人的商店入口、技能说明入口、职业技能清单标题、等级标签、更多/返回按钮。
- 营地 Captain 对 SnowCavern 与 Ancient Creature 的提示。
- CaveGuard 的看守说明。
- SnowMonument 的碑文、进入按钮与 Sinseok Miner 本地喊话。
- Wanderer Charlotte 的 Ice Hell 入口对话与确认按钮。

处理原则：

- 保留 `[Trade]`、`[Types]` 及技能书英文名。
- 保留 `@BuySell`、`@BuyBack`、`@helpbooks`、`@helpBooks`、`@War*`、`@Wiz*`、`@Tao*`、`@Assa*`、`@Arc*`、`@move1`、`@Go`、`@Exit` 等跳转标签。
- 保留 `SnowCavern`、`Ancient Creature`、`Cave`、`Monument`、`Sinseok Miner`、`Ice Hell` 等名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `I will not help`、`Close/@`、`Hello`、`Welcome`、`How many`、`View/@`、`Store.`、`explanation`、`Please select`、`Buy Back`、`Back/@`、`What kind`、`Skill List`、`Level`、`More/@`、`Becarful`、`challanege`、`Monument which`、`Have you heard` 等旧英文显示文本，未再命中。
- 针对整个 `WhiteValley` 目录扫描明显旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第二十六批 WhiteValley 营地基础功能 NPC

更新时间：2026-06-03

本批开始处理 WhiteValley / Encampment 区域，优先翻译基础商人、仓库、船夫、路牌和委托商人：

- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Blacksmith.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Drapery.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Accessory.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/General.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Storage.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Collector.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Fisheries.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Sailor.txt`
- `Build/Server/Debug/Envir/NPCs/WhiteValley/Encampment/Signpost.txt`

覆盖内容：

- 武器、服装、首饰、杂货/药水商人的问候、买卖、回购和修理说明。
- 仓库的存取、包裹发送和领取说明。
- 收购商与渔具商的出售、购买、回购提示。
- 委托商人的市场入口与帮助说明。
- 船夫的 Bichon Province / Prajna Island 航线、费用、边境说明和金币不足提示。
- 营地路牌的商店传送入口与目的地按钮。

处理原则：

- 保留 `[Trade]`、`[Types]`、`[Quests]` 数据段及其物品/任务编号。
- 保留 `@BuySell`、`@BuyBack`、`@Repair`、`@Storage`、`@SendParcel`、`@CollectParcel`、`@Market`、`@wvmove`、`@wvmove1`、`@go-*` 等脚本入口。
- 保留 `Bichon Province`、`Prajna Island`、`WhiteValley`、`Gold`、`monsters` 等名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `I will not help`、`Close/@`、`Hello`、`Welcome`、`Which`、`Would you`、`Back/@`、`Buy Back`、`View/@`、`Repair/@`、`Access/@`、`Please`、`The boat`、`Passengers`、`Pay ... Board`、`Dont Have`、`View Market`、`Commission Merchant`、`Weapon shop` 等旧英文显示文本，未再命中；剩余命中均为 `BuyBack` 等脚本标签。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第二十五批 PastBichon 时间石、护卫与武器精炼 NPC

更新时间：2026-06-03

本批继续收口 PastBichon 剩余 NPC 可见文本：

- `Build/Server/Debug/Envir/NPCs/PastBichon/Timestone.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/PastBichon_Timestone2.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/ChaosNode.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/General.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/LeftEscort.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/RightEscort.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/2Pbbl.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/Unused.txt`

覆盖内容：

- 时间石返回未来的说明与按钮。
- ChaosNode 进入 Dragons Lair 与缺少 DragonScale 的提示。
- 将军与左右护卫的战场叙述、交谈按钮和关闭按钮。
- 武器精炼 NPC 的功能入口、材料说明、费用说明、精炼窗口提示、成功/处理中/失败/取回武器状态提示。

处理原则：

- 保留 `MOVE`、`CHECKITEM`、`TAKEITEM`、`$UPGRADEWEAPONFEE`、`$USERWEAPON` 等命令和变量。
- 保留 `@tele`、`@upgrade`、`@Biron`、`@Etc`、`@Weapon`、`@Gold`、`@upgradenow`、`@getbackupgnow`、`~@upgradenow_*`、`~@getbackupgnow_*` 等跳转和状态段。
- 保留 `Omas`、`DragonScale`、`Dragons Lair`、`BlackIronOre`、`Accessories`、`Necklaces`、`Bracelets`、`Rings` 等名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `Feature to come`、`available weapon`、`Ask to refine`、`Take the refined`、`Accept refining`、`Cancel Upgrade`、`Get back weapon`、`Collect weapon`、`Come back later`、`Worship me`、`boundary`、`Enter the`、`Close/@` 等旧英文显示文本，未再命中；剩余命中均为保留名称引用。
- 针对整个 `PastBichon` 目录扫描明显旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第二十四批 PastBichon 基础商人与编号镜像 NPC

更新时间：2026-06-03

本批开始处理 PastBichon 区域，优先翻译基础功能 NPC 与对应编号镜像文件：

- `Build/Server/Debug/Envir/NPCs/PastBichon/Weaponsmith.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/2Pbwe.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/Clothes.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/3Pbdr.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/Jewellery.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/8Pbac.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/Potion.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/4Pbdu.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/Warehouse.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/6Pbwh.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/Peddlar.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/7Pbst.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/TrustMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/12Pbtm.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/GTMerchant.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/13Pbgtm.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/CraftsLady.txt`
- `Build/Server/Debug/Envir/NPCs/PastBichon/11Pbcft.txt`

覆盖内容：

- 武器、服装、首饰、药水、仓库、杂货商的问候、装备提示、买卖、回购、修理、仓库与包裹说明。
- 杂货商的 Candle、Dungeonescape、RandomTeleport、RepairOil、TownTeleport 说明。
- 委托商人的市场入口、帮助说明、手续费/保证金/期限/数量提示。
- 行会领地商人的领地说明、租赁、传送、列表查看和交易说明。
- 制作女士的制作入口说明和 Recipe 买卖提示。

处理原则：

- 保留 `[Trade]`、`[RECIPE]`、`[Types]`、`[QUESTS]` 数据段及其物品/配方英文名。
- 保留 `@BuySell`、`@BuyBack`、`@Repair`、`@Storage`、`@SendParcel`、`@Market`、`@agitreg`、`@agitmove`、`@agittrade`、`@Craft` 等脚本入口。
- 保留 `Bichon Province`、`Mongchon Province`、`Tao Village`、`Prajna Island`、`Past Bichon`、`GT Steward`、`Armed Transporter`、`Candle`、`RepairOil` 等名称引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `I will not help`、`Close/@`、`Hello`、`Welcome`、`Which`、`Would you`、`Back/@`、`Buy Back`、`View/@`、`Repair/@`、`Access/@`、`Please`、`Guild territory`、`View Market`、`Commission Merchant`、`RepairOil makes`、`TownTeleport scrolls` 等旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。

## 第二十三批般若岛告示牌、船夫与渔夫 NPC

更新时间：2026-06-03

本批继续收口 Prajna Island 中剩余的低风险可见文本：

- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Board.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Prajna_Signpos.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Fisherman.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Pifsh.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/Sailor.txt`
- `Build/Server/Debug/Envir/NPCs/PrajnaIsland/OddOldMan.txt`

覆盖内容：

- 村庄商店传送告示牌的入口说明与目的地按钮。
- 渔夫/编号镜像渔夫的 PK 拒绝、闲置对话与确认按钮。
- 船夫的登船说明、Bichon Province / White Valley 航线费用、边境介绍和金币不足提示。
- 修复 `OddOldMan.txt` 的编码异常并恢复 TimeStone / TimeStonePiece 相关中文显示文本。

处理原则：

- 保留 `MOVE` 坐标、`CHECKGOLD`、`TAKEGOLD`、`GIVEITEM`、`CHECKPKPOINT` 等脚本命令。
- 保留 `@main-1`、`@weap`、`@cloth`、`@pot`、`@jewel`、`@chief`、`@brdmove`、`@talkWV`、`@buystonenow` 等跳转标签。
- 保留 `Bichon Province`、`White Valley`、`WhiteValley`、`TimeStone`、`TimeStonePiece`、`Gold` 等名称/物品引用。
- 统一本批目标文件为 UTF-8 BOM。

验证：

- 针对本批文件扫描 `I will not help`、`Close/@`、`Use.`、`teleport to the village`、`Hello there`、`The boat`、`Passengers`、`Pay ... Board`、`Back/@`、`Dont Have`、`Exit/@` 等旧英文显示文本，未再命中。
- 针对整个 `PrajnaIsland` 目录扫描同类明显旧英文显示文本，未再命中。
- 使用本地 `.tools/dotnet/dotnet.exe` 重启 `Build/Server/Debug/Server.dll`，进程正常运行，日志显示“环境已启动”“网络已启动”。
- `Logs/Debug/Debug (03-06-2026).log` 仍为 0 字节。
- 已知旧问题：日志仍有既存 `HiGreatGhoul` 掉落项 `RedDagger Q` 无法加载，非本批 NPC 翻译引入。


