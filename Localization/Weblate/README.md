# Weblate 数据库翻译目录

这里是 `Server.MirDB` 可翻译字段的 Git 源目录。Weblate 和翻译者只应修改各组件 `zh_CN.po` 中的 `msgstr`；不要修改 `msgctxt`、`msgid` 或 `catalog-manifest.json`。

## 组件

| 组件 | 内容 | 当前规模 |
| --- | --- | ---: |
| `database-text` | 技能名、地图标题、任务文本、物品提示 | 1,265 |
| `npc-names` | NPC 显示名 | 375 |
| `item-names` | 物品显示名 | 1,628 |
| `monster-names` | 怪物显示名 | 555 |

稳定键格式为 `类型:索引:字段`，例如 `NPC:283:Name`。相同英文名称位于不同数据库记录时仍有独立稳定键，Weblate 可通过翻译记忆复用译文。

`catalog-manifest.json` 冻结原文和稳定键，并带有 SHA-256 指纹。它用于阻止误改 `msgid`、索引漂移或组件错配。

## Weblate 配置

创建一个项目和以下四个双语 GNU gettext PO 组件：

| 组件 | 文件掩码 |
| --- | --- |
| 数据库文本 | `Localization/Weblate/database-text/*.po` |
| NPC 名称 | `Localization/Weblate/npc-names/*.po` |
| 物品名称 | `Localization/Weblate/item-names/*.po` |
| 怪物名称 | `Localization/Weblate/monster-names/*.po` |

- 源语言：English
- 目标文件：`zh_CN.po`
- 文件格式：GNU gettext PO
- 建议启用建议与审核，普通贡献者提交建议，由维护者批准。
- 项目术语以 `docs/localization-glossary.zh-CN.md` 为基础建立 Weblate 术语表。

Weblate 官方资料：[文件格式](https://docs.weblate.org/en/latest/formats.html)、[版本控制集成](https://docs.weblate.org/en/latest/vcs.html)、[翻译工作流](https://docs.weblate.org/en/latest/workflows.html)。

## 本地命令

以下命令从 `Build/Server/Debug` 执行：

```powershell
$dotnet = '..\..\..\..\.tools\dotnet\dotnet.exe'
$tool = '..\..\..\Tools\LocalizationDbTool\bin\Debug\net8.0\LocalizationDbTool.dll'

# 从活动数据库生成或刷新目录；不会覆盖 PO 中尚未部署的译文
& $dotnet $tool export-weblate `
  ..\..\..\Localization\Weblate `
  ..\..\..\docs

# 校验稳定键、冻结原文、组件归属和占位符
& $dotnet $tool validate-weblate `
  ..\..\..\Localization\Weblate

# 安全回写；默认扫描当前目录下的 Envir 和 Configs
& $dotnet $tool apply-weblate `
  ..\..\..\Localization\Weblate
```

`apply-weblate` 的规则：

1. 先完成全目录校验；任何错误都不修改数据库。
2. 空译文和 `fuzzy` 译文不回写。
3. 自动备份 `Server.MirDB` 后才应用普通字段或没有引用的名称。
4. 名称旧值只要在 `Envir` 或 `Configs` 中出现，就以退出码 `3` 阻止整批回写并打印引用位置。
5. 被阻止的名称由维护者审查，使用带 `REFERENCE_PREFIXES` 的 TSV 映射同步引用；完成后再次导出，PO 译文会被保留。

## 翻译规则

- 只修改 `msgstr`，不要把中文写入 `msgid`。
- 不翻译脚本命令、变量、跳转标签、文件名、协议字段和内部枚举。
- 人名、地名和职业前缀遵守项目术语表。
- 原文包含繁体字或其他东亚字符的条目仍按“未翻译”处理，并在 PO 注释中提示人工确认。
- 合并前运行 `validate-weblate`；部署后重新构建服务端并检查名称引用。
