# 水晶传奇 Mir2 汉化计划

## 目标

将 Crystal 项目初始化为默认中文，并分阶段完成客户端、服务端管理端、服务端提示、游戏数据库文本、NPC 脚本、地图/怪物/物品名称以及运维文档的简体中文本地化。

## 当前初始化状态

- 客户端默认语言已从 `English` 改为 `Chinese`：`Client/Settings.cs`
- 服务端默认语言已从 `English` 改为 `Chinese`：`Server/Settings.cs`
- 当前服务器运行配置已显式设置 `Language=Chinese`：`Server/Configs/Setup.ini`
- 最新数据库 release `29.01.25` 已应用到 `Server/Configs`、`Server/Envir`、`Server/Maps`、`Server/Server.MirDB`

## 已发现的汉化入口

- 客户端界面与客户端提示：`Client/Localization/English.json`、`Client/Localization/Chinese.json`
- 服务端管理端与服务端提示：`Server.MirForms/Localization/English.json`、`Server.MirForms/Localization/Chinese.json`
- 服务端语言加载逻辑：`Shared/Language.cs`
- 服务端运行配置：`Server/Configs/Setup.ini`
- 数据库文本：`Server/Server.MirDB`
- NPC、掉落、公告、脚本与环境文本：`Server/Envir`
- 地图文件与地图名称配置：`Server/Maps` 以及数据库中的地图记录

## 第一阶段：语言资源健康检查

1. 对比 `English.json` 与 `Chinese.json` 的 key 完整性。
2. 清理客户端语言 JSON 中的重复 key，避免工具链解析失败。
3. 检查中文 JSON 是否存在乱码、缺失引号、占位符不一致等问题。
4. 建立占位符校验规则，确保 `{0}`、`{1}` 等参数在中英文中一一对应。
5. 统一术语表，例如：
   - Warrior：战士
   - Wizard：法师
   - Taoist：道士
   - Assassin：刺客
   - Archer：弓手
   - Guild：行会
   - DC/MC/SC：攻击/魔法/道术
   - AC/MAC：防御/魔御

## 第二阶段：客户端汉化

1. 修复 `Client/Localization/Chinese.json` 中的乱码与重复 key。
2. 汉化登录、角色选择、背包、人物、技能、任务、邮件、交易、拍卖、行会、好友、师徒、坐骑、钓鱼等界面文本。
3. 检查中文字体渲染，优先使用能覆盖简体中文的字体。
4. 检查中文文本长度导致的 UI 溢出，重点关注按钮、标签、弹窗和技能说明。
5. 运行客户端后逐页截图验收，记录仍显示英文的位置。

## 第三阶段：服务端与管理工具汉化

1. 修复 `Server.MirForms/Localization/Chinese.json` 中的术语一致性。
2. 汉化管理端窗口标题、菜单、按钮、日志、错误提示和配置页。
3. 汉化服务端控制台与系统消息。
4. 确认 `Server/Configs/Setup.ini` 中 `Language=Chinese` 能正确加载中文 JSON。
5. 将新增配置文件的默认语言也设置为中文。

## 第四阶段：数据库内容汉化

1. 使用项目自带数据库编辑器或服务端管理端打开 `Server.MirDB`。
2. 导出或逐表处理以下内容：
   - 物品名称、物品描述、装备属性说明
   - 技能名称、技能说明
   - 怪物名称
   - NPC 名称
   - 地图名称、安全区名称、传送点名称
   - 任务标题、任务描述、任务目标、奖励说明
3. 对照经典传奇术语，优先保留玩家熟悉的译名。
4. 每批修改后启动服务端检查数据库加载错误。
5. 保留英文原名到中文名的映射表，方便回滚和查漏。

## 第五阶段：NPC 与脚本汉化

1. 扫描 `Server/Envir/NPCs`、`Server/Envir/Quests`、`Server/Envir/NameLists`、`Server/Envir/Notice.txt`。
2. 汉化 NPC 对话、任务台词、系统公告、活动文本。
3. 保留脚本命令、变量名、跳转标签和文件引用，不翻译命令结构。
4. 对每个高频 NPC 做进入游戏验收，避免汉化时破坏脚本语法。

## 第六阶段：硬编码英文清理

1. 扫描 `.cs` 文件中的英文字符串字面量。
2. 判断字符串类型：
   - 用户可见文本：迁移到语言 JSON
   - 日志/开发调试文本：按需汉化或保留
   - 协议、枚举、文件名、命令：不要翻译
3. 对新增语言 key 添加到 `Shared/Language.cs` 枚举，并同步写入 English/Chinese JSON。
4. 每次迁移后做一次运行验证。

## 第七阶段：构建与验收

1. 安装 .NET 8 SDK 和 Visual Studio Build Tools/MSBuild。
2. 执行解决方案构建：`Legend of Mir.sln`
3. 启动服务端，检查数据库、地图、NPC、怪物加载日志。
4. 启动客户端，登录本地服务器并完成以下路径：
   - 新建账号
   - 创建角色
   - 进入游戏
   - 打开主要 UI 面板
   - 与 NPC 对话
   - 接取任务
   - 击杀怪物并拾取物品
   - 使用技能、交易、邮件、行会相关功能
5. 建立“仍为英文/乱码/显示溢出”的问题清单。

## 风险与注意事项

- 不要直接翻译脚本命令、枚举名、文件名、数据库内部标识符，否则可能导致加载失败。
- 中文 JSON 必须保持合法 JSON，且占位符数量和顺序必须与英文一致。
- 客户端 UI 是固定坐标风格，中文文本更长，容易出现遮挡和溢出。
- 数据库汉化会影响游戏内显示，但也可能影响脚本按名称查找的逻辑；物品、怪物、NPC 名称需要逐步验证。
- 修改数据库前应备份 `Server.MirDB`。

## 建议执行顺序

1. 先修复语言 JSON 质量。
2. 再完成客户端和服务端管理端 UI。
3. 然后处理服务端提示和硬编码文本。
4. 最后批量处理数据库与 NPC 脚本。

这个顺序可以先让项目稳定进入中文环境，再逐步扩大汉化范围。
