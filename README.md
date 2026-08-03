# Crystal Mir2 中文本地化分支

本仓库基于 [Suprcode/Crystal](https://github.com/Suprcode/Crystal)，用于《热血传奇 2》Crystal 客户端、服务端和游戏数据的简体中文本地化。

## 当前状态

- 客户端与服务端默认语言已设为 `Chinese`。
- 客户端 1,229 个、服务端/管理端 766 个语言键已与英文资源完整对齐。
- 技能名、地图标题和任务数据库文本已完成一轮汉化。
- NPC、任务、公告等脚本自然语言已在本地运行目录完成大范围处理。
- 数据库的 3,823 个可翻译字段已迁移到四个 Weblate PO 组件，可进行多人翻译和进度统计。
- 当前仍在处理名称引用联动；物品名、怪物名和部分 NPC 名尚未完成。
- 硬编码英文清理和完整游戏内视觉验收尚未完成。

准确进度、覆盖统计和版本控制边界见 [汉化状态](docs/localization-status.zh-CN.md)。

## 构建环境

- Windows x64
- Visual Studio 2022 17.8 或更高版本，或 .NET 8 SDK
- 客户端运行所需的地图、素材和数据库资源

常用构建命令：

```powershell
dotnet build Client/Client.csproj -c Debug
dotnet build Server.MirForms/Server.csproj -c Debug
```

`Build/` 是本地运行产物目录，不受 Git 跟踪。仅克隆源码无法得到当前已处理的数据库和环境脚本；交付或迁移前请同时准备对应的运行资源包。

## 文档

- [当前汉化状态](docs/localization-status.zh-CN.md)
- [剩余工作计划](docs/localization-plan.zh-CN.md)
- [术语表](docs/localization-glossary.zh-CN.md)
- [数据库与名称联动流程](docs/localization-database.zh-CN.md)
- [Weblate 数据库目录](Localization/Weblate/README.md)

Weblate PO 是后续数据库翻译的协作源。既有 `docs/localization-*-mapping*.tsv` 暂时保留，用于恢复英文原文和审计历史；数据库 dump、候选表和引用扫描属于可再生成产物，不提交仓库。

## 上游资料

- [Crystal 上游仓库](https://github.com/Suprcode/Crystal)
- [LOMCN Crystal Wiki](https://www.lomcn.net/wiki/index.php/Crystal)
- [Crystal 数据库](https://github.com/Suprcode/Crystal.Database)
- [地图编辑器](https://github.com/Suprcode/Crystal.MapEditor)

本项目沿用上游仓库声明的 GPL-2.0 许可证。
