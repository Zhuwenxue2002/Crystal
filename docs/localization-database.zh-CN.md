# 数据库与名称联动流程

更新日期：2026-08-03

## 当前资源

- 活动数据库：`Build/Server/Debug/Server.MirDB`
- 活动脚本和配置：`Build/Server/Debug/Envir`、`Build/Server/Debug/Configs`
- 数据库工具：`Tools/LocalizationDbTool`
- Weblate 目录：`Localization/Weblate`
- 历史映射：`docs/localization-*-mapping*.tsv`

工具从当前工作目录读取 `Server.MirDB`，因此应在 `Build/Server/Debug` 中运行。`Build/` 不受 Git 跟踪；Weblate PO 是后续翻译的 Git 协作源，二进制数据库只是部署产物。

## Weblate 目录

数据库字段按稳定键 `类型:索引:字段` 导出为四个双语 PO 组件：

| 组件 | 字段数 | 内容 |
| --- | ---: | --- |
| `database-text` | 1,265 | 技能名、地图标题、任务文本、物品提示 |
| `npc-names` | 375 | NPC 显示名 |
| `item-names` | 1,628 | 物品显示名 |
| `monster-names` | 555 | 怪物显示名 |

`catalog-manifest.json` 冻结原文和稳定键。翻译者只能修改 `zh_CN.po` 的 `msgstr`；`msgctxt`、`msgid` 和清单不得在 Weblate 中改写。详细配置见 [Weblate 数据库目录](../Localization/Weblate/README.md)。

首次导出已从历史映射恢复 1,452 条原文/译文，未翻译条目的 `msgstr` 保持为空，因此 Weblate 的完成率可直接反映真实进度。

## 日常流程

以下命令从仓库根目录开始：

```powershell
Push-Location Build/Server/Debug

$dotnet = '..\..\..\..\.tools\dotnet\dotnet.exe'
$tool = '..\..\..\Tools\LocalizationDbTool\bin\Debug\net8.0\LocalizationDbTool.dll'

# 数据库或历史映射变化后刷新目录；保留尚未部署的 PO 译文
& $dotnet $tool export-weblate `
  ..\..\..\Localization\Weblate `
  ..\..\..\docs

# 合并或部署前校验
& $dotnet $tool validate-weblate `
  ..\..\..\Localization\Weblate

# 安全回写活动数据库
& $dotnet $tool apply-weblate `
  ..\..\..\Localization\Weblate

Pop-Location
```

`validate-weblate` 检查清单指纹、稳定键、组件归属、冻结原文和占位符。空译文与 `fuzzy` 译文不会部署。

`apply-weblate` 在修改前自动备份数据库。名称旧值只要仍出现在 `Envir` 或 `Configs` 中，工具就拒绝整批回写并列出引用；普通字段和没有引用的名称可以直接部署。

## 名称引用

每个名称都要检查：

- 数据库名称字段；
- NPC 和任务脚本；
- 掉落表、配方和商店清单；
- GM 清单、事件脚本和刷新配置；
- 服务端设置中的按名查找项；
- 玩家可见的任务正文和传送按钮。

不要翻译脚本命令、变量、跳转标签、文件名、协议字段和内部枚举。无法确认用途的英文标识先保留并记录。

被阻止的名称仍使用旧 TSV 流程人工审核：

```text
#TYPE  INDEX  FIELD  SOURCE  TARGET  REFERENCE_PREFIXES
```

第六列用分号限制允许修改的相对路径；例如 `Quests;SET [].txt`。完成数据库与引用同步后再次运行 `export-weblate`，工具会保留 Weblate 译文并更新部署状态。

现有 `docs/localization-*-mapping*.tsv` 暂时保留，用于恢复最初原文和审计迁移。待 Weblate 目录经过发布资源包的完整重建验证后，再决定是否归档历史 TSV。
