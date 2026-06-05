using System.Text;
using System.Text.RegularExpressions;
using Server.MirDatabase;
using Server.MirEnvir;

Console.OutputEncoding = Encoding.UTF8;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: LocalizationDbTool <dump|apply|name-refs|apply-name-refs> [mapping.tsv|scanRoot]");
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
    case "name-refs":
        var envirPath = args.Length >= 2 ? args[1] : Path.Combine(AppContext.BaseDirectory, "Envir");
        DumpNameReferences(envir, envirPath);
        return 0;
    case "apply-name-refs":
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: LocalizationDbTool apply-name-refs <mapping.tsv> <scanRoot>");
            return 2;
        }

        var replacements = ApplyNameReferences(args[1], args[2]);
        Console.WriteLine($"Applied {replacements} name reference replacements.");
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

static void DumpNameReferences(Envir envir, string envirPath)
{
    if (!Directory.Exists(envirPath))
    {
        Console.Error.WriteLine($"Envir path not found: {envirPath}");
        return;
    }

    var files = Directory.EnumerateFiles(envirPath, "*", SearchOption.AllDirectories)
        .Where(IsTextCandidate)
        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
        .ToList();

    var fileTexts = files.Select(path => new ScanFile(path, RelativePath(envirPath, path), File.ReadAllLines(path, Encoding.UTF8))).ToList();

    Console.WriteLine("#TYPE\tINDEX\tNAME\tREF_COUNT\tFILE_COUNT\tAREAS\tRISK\tSAMPLES");

    foreach (ItemInfo info in envir.ItemInfoList.OrderBy(x => x.Index))
        WriteReferenceSummary("Item", info.Index, info.Name, fileTexts);

    foreach (MonsterInfo info in envir.MonsterInfoList.OrderBy(x => x.Index))
        WriteReferenceSummary("Monster", info.Index, info.Name, fileTexts);

    foreach (NPCInfo info in envir.NPCInfoList.OrderBy(x => x.Index))
        WriteReferenceSummary("NPC", info.Index, info.Name, fileTexts);
}

static bool IsTextCandidate(string path)
{
    var extension = Path.GetExtension(path).ToLowerInvariant();
    if (extension != ".txt" && extension != ".ini" && extension != ".info" && extension != ".cfg") return false;

    var relative = path.Replace('\\', '/');
    return !relative.Contains("/Previews/", StringComparison.OrdinalIgnoreCase);
}

static int ApplyNameReferences(string mappingPath, string scanRoot)
{
    if (!Directory.Exists(scanRoot))
    {
        Console.Error.WriteLine($"Scan root not found: {scanRoot}");
        return 0;
    }

    var mappings = ReadMappings(mappingPath)
        .Where(x => x.Field == "Name" && !string.IsNullOrWhiteSpace(x.Target) && x.Source != x.Target)
        .GroupBy(x => x.Source, StringComparer.OrdinalIgnoreCase)
        .Select(x => x.First())
        .ToList();

    if (mappings.Count == 0) return 0;

    var files = Directory.EnumerateFiles(scanRoot, "*", SearchOption.AllDirectories)
        .Where(IsTextCandidate)
        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
        .ToList();

    var replacements = 0;

    foreach (var file in files)
    {
        var text = File.ReadAllText(file, Encoding.UTF8);
        var updated = text;

        foreach (var mapping in mappings)
        {
            var regex = BuildNameRegex(mapping.Source);
            var count = regex.Matches(updated).Count;
            if (count == 0) continue;

            updated = regex.Replace(updated, mapping.Target);
            replacements += count;
        }

        if (updated == text) continue;

        File.WriteAllText(file, updated, Encoding.UTF8);
    }

    return replacements;
}

static void WriteReferenceSummary(string type, int index, string name, List<ScanFile> files)
{
    if (string.IsNullOrWhiteSpace(name)) return;

    var refs = new List<NameReference>();
    var regex = BuildNameRegex(name);

    foreach (var file in files)
    {
        for (var i = 0; i < file.Lines.Length; i++)
        {
            var line = file.Lines[i];
            if (!regex.IsMatch(line)) continue;

            refs.Add(new NameReference(file.RelativePath, i + 1, line.Trim()));
        }
    }

    if (refs.Count == 0) return;

    var fileCount = refs.Select(x => x.Path).Distinct(StringComparer.OrdinalIgnoreCase).Count();
    var areas = refs
        .Select(x => AreaOf(x.Path))
        .Where(x => x.Length > 0)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase);
    var risk = ClassifyRisk(type, refs);
    var samples = refs
        .Take(5)
        .Select(x => $"{x.Path}:{x.Line}:{TrimSample(x.Text)}");

    Console.WriteLine($"{type}\t{index}\t{Escape(name)}\t{refs.Count}\t{fileCount}\t{Escape(string.Join(",", areas))}\t{risk}\t{Escape(string.Join(" | ", samples))}");
}

static Regex BuildNameRegex(string name)
{
    var escaped = Regex.Escape(name);
    return new Regex($@"(?<![A-Za-z0-9_]){escaped}(?![A-Za-z0-9_])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
}

static string ClassifyRisk(string type, List<NameReference> refs)
{
    var areas = refs.Select(x => AreaOf(x.Path)).ToList();
    var hasLookupArea = areas.Any(x => x.Equals("Envir/Drops", StringComparison.OrdinalIgnoreCase)
                                    || x.Equals("Envir/Recipe", StringComparison.OrdinalIgnoreCase)
                                    || x.Equals("Envir/SystemScripts", StringComparison.OrdinalIgnoreCase)
                                    || x.Equals("Envir/NPCs", StringComparison.OrdinalIgnoreCase)
                                    || x.Equals("Envir/Quests", StringComparison.OrdinalIgnoreCase)
                                    || x.Equals("Configs", StringComparison.OrdinalIgnoreCase));
    var hasCommandLine = refs.Any(x => x.Text.StartsWith("#", StringComparison.Ordinal)
                                    || x.Text.StartsWith("[", StringComparison.Ordinal)
                                    || x.Text.Contains("CHECK", StringComparison.OrdinalIgnoreCase)
                                    || x.Text.Contains("GIVE", StringComparison.OrdinalIgnoreCase)
                                    || x.Text.Contains("TAKE", StringComparison.OrdinalIgnoreCase)
                                    || x.Text.Contains("MONGEN", StringComparison.OrdinalIgnoreCase));

    if (hasLookupArea && hasCommandLine) return "High";
    if (hasLookupArea || type == "Item" || type == "Monster") return "Medium";
    return "Low";
}

static string AreaOf(string relativePath)
{
    var parts = relativePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length == 0) return "";
    if (parts[0].Equals("Envir", StringComparison.OrdinalIgnoreCase) && parts.Length > 1)
        return $"Envir/{parts[1]}";
    return parts[0];
}

static string RelativePath(string root, string path)
{
    return Path.GetRelativePath(root, path).Replace('\\', '/');
}

static string TrimSample(string text)
{
    if (text.Length <= 120) return text;
    return text.Substring(0, 117) + "...";
}

static int Apply(Envir envir, string mappingPath)
{
    var changes = 0;

    foreach (var row in ReadMappings(mappingPath))
        changes += ApplyOne(envir, row.Type, row.Index, row.Field, row.Source, row.Target) ? 1 : 0;

    return changes;
}

static List<MappingRow> ReadMappings(string mappingPath)
{
    var rows = new List<MappingRow>();

    foreach (var rawLine in File.ReadLines(mappingPath, Encoding.UTF8))
    {
        if (string.IsNullOrWhiteSpace(rawLine) || rawLine.StartsWith("#")) continue;

        var parts = rawLine.Split('\t');
        if (parts.Length < 5) continue;
        if (!int.TryParse(parts[1], out var index)) continue;

        var source = Unescape(parts[3]);
        var target = Unescape(parts[4]);
        if (string.IsNullOrWhiteSpace(target) || source == target) continue;

        rows.Add(new MappingRow(parts[0], index, parts[2], source, target));
    }

    return rows;
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

        case "Item" when field == "Name":
        case "ItemCandidate" when field == "Name":
            item = envir.ItemInfoList.FirstOrDefault(x => x.Index == index && x.Name == source);
            if (item == null) return false;
            item.Name = target;
            return true;

        case "Monster" when field == "Name":
        case "MonsterCandidate" when field == "Name":
            var monster = envir.MonsterInfoList.FirstOrDefault(x => x.Index == index && x.Name == source);
            if (monster == null) return false;
            monster.Name = target;
            return true;

        case "NPC" when field == "Name":
        case "NPCCandidate" when field == "Name":
            var npc = envir.NPCInfoList.FirstOrDefault(x => x.Index == index && x.Name == source);
            if (npc == null) return false;
            npc.Name = target;
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

record ScanFile(string Path, string RelativePath, string[] Lines);
record NameReference(string Path, int Line, string Text);
record MappingRow(string Type, int Index, string Field, string Source, string Target);
