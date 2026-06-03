using System.Text;
using Server.MirDatabase;
using Server.MirEnvir;

Console.OutputEncoding = Encoding.UTF8;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: LocalizationDbTool <dump|apply> [mapping.tsv]");
    return 2;
}

var envir = Envir.Main;
if (!envir.LoadDB())
{
    Console.Error.WriteLine("Failed to load Server.MirDB.");
    return 1;
}

switch (args[0].ToLowerInvariant())
{
    case "dump":
        Dump(envir);
        return 0;
    case "apply":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: LocalizationDbTool apply <mapping.tsv>");
            return 2;
        }

        var changes = Apply(envir, args[1]);
        envir.SaveDB();
        Console.WriteLine($"Applied {changes} database localization changes.");
        return 0;
    default:
        Console.Error.WriteLine($"Unknown command: {args[0]}");
        return 2;
}

static void Dump(Envir envir)
{
    Console.WriteLine("#TYPE\tINDEX\tFIELD\tVALUE");

    foreach (MagicInfo info in envir.MagicInfoList.OrderBy(x => x.Spell))
        Write("Magic", (int)info.Spell, "Name", info.Name);

    foreach (MapInfo info in envir.MapInfoList.OrderBy(x => x.Index))
        Write("Map", info.Index, "Title", info.Title);

    foreach (QuestInfo info in envir.QuestInfoList.OrderBy(x => x.Index))
    {
        Write("Quest", info.Index, "Name", info.Name);
        Write("Quest", info.Index, "Group", info.Group);
        Write("Quest", info.Index, "GotoMessage", info.GotoMessage);
        Write("Quest", info.Index, "KillMessage", info.KillMessage);
        Write("Quest", info.Index, "ItemMessage", info.ItemMessage);
        Write("Quest", info.Index, "FlagMessage", info.FlagMessage);
    }

    foreach (ItemInfo info in envir.ItemInfoList.OrderBy(x => x.Index))
    {
        Write("ItemCandidate", info.Index, "Name", info.Name);
        Write("Item", info.Index, "ToolTip", info.ToolTip);
    }

    foreach (MonsterInfo info in envir.MonsterInfoList.OrderBy(x => x.Index))
        Write("MonsterCandidate", info.Index, "Name", info.Name);

    foreach (NPCInfo info in envir.NPCInfoList.OrderBy(x => x.Index))
        Write("NPCCandidate", info.Index, "Name", info.Name);
}

static int Apply(Envir envir, string mappingPath)
{
    var changes = 0;

    foreach (var rawLine in File.ReadLines(mappingPath, Encoding.UTF8))
    {
        if (string.IsNullOrWhiteSpace(rawLine) || rawLine.StartsWith("#")) continue;

        var parts = rawLine.Split('\t');
        if (parts.Length < 5) continue;

        var type = parts[0];
        if (!int.TryParse(parts[1], out var index)) continue;
        var field = parts[2];
        var source = Unescape(parts[3]);
        var target = Unescape(parts[4]);

        if (string.IsNullOrWhiteSpace(target) || source == target) continue;

        changes += ApplyOne(envir, type, index, field, source, target) ? 1 : 0;
    }

    return changes;
}

static bool ApplyOne(Envir envir, string type, int index, string field, string source, string target)
{
    switch (type)
    {
        case "Magic" when field == "Name":
            var magic = envir.MagicInfoList.FirstOrDefault(x => (int)x.Spell == index && x.Name == source);
            if (magic == null) return false;
            magic.Name = target;
            return true;

        case "Map" when field == "Title":
            var map = envir.MapInfoList.FirstOrDefault(x => x.Index == index && x.Title == source);
            if (map == null) return false;
            map.Title = target;
            return true;

        case "Quest":
            var quest = envir.QuestInfoList.FirstOrDefault(x => x.Index == index);
            if (quest == null) return false;
            return ApplyQuest(quest, field, source, target);

        case "Item" when field == "ToolTip":
            var item = envir.ItemInfoList.FirstOrDefault(x => x.Index == index && x.ToolTip == source);
            if (item == null) return false;
            item.ToolTip = target;
            return true;

        default:
            return false;
    }
}

static bool ApplyQuest(QuestInfo quest, string field, string source, string target)
{
    switch (field)
    {
        case "Name" when quest.Name == source:
            quest.Name = target;
            return true;
        case "Group" when quest.Group == source:
            quest.Group = target;
            return true;
        case "GotoMessage" when quest.GotoMessage == source:
            quest.GotoMessage = target;
            return true;
        case "KillMessage" when quest.KillMessage == source:
            quest.KillMessage = target;
            return true;
        case "ItemMessage" when quest.ItemMessage == source:
            quest.ItemMessage = target;
            return true;
        case "FlagMessage" when quest.FlagMessage == source:
            quest.FlagMessage = target;
            return true;
        default:
            return false;
    }
}

static void Write(string type, int index, string field, string value)
{
    if (string.IsNullOrWhiteSpace(value)) return;
    Console.WriteLine($"{type}\t{index}\t{field}\t{Escape(value)}");
}

static string Escape(string value)
{
    return value.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");
}

static string Unescape(string value)
{
    var builder = new StringBuilder(value.Length);

    for (var i = 0; i < value.Length; i++)
    {
        if (value[i] != '\\' || i == value.Length - 1)
        {
            builder.Append(value[i]);
            continue;
        }

        var next = value[++i];
        builder.Append(next switch
        {
            'n' => '\n',
            'r' => '\r',
            't' => '\t',
            '\\' => '\\',
            _ => next
        });
    }

    return builder.ToString();
}
