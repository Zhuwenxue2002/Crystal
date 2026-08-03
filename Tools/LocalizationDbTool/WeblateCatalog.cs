using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Server.MirDatabase;
using Server.MirEnvir;

internal static class WeblateCatalogService
{
    private const int ManifestVersion = 1;
    private const string ManifestFileName = "catalog-manifest.json";
    private const string TargetFileName = "zh_CN.po";

    private static readonly UTF8Encoding Utf8NoBom = new(false);
    private static readonly Regex NumericPlaceholderRegex = new(@"\{\d+(?:,[^{}]+)?(?::[^{}]+)?\}", RegexOptions.Compiled);
    private static readonly Regex PrintfPlaceholderRegex = new(@"%(?:\d+\$)?[-+#0]*\d*(?:\.\d+)?[A-Za-z]", RegexOptions.Compiled);
    private static readonly Regex CatalogIdRegex = new(@"^(Magic|Map|Quest|Item|Monster|NPC):(\d+):([A-Za-z]+)$", RegexOptions.Compiled);

    private static readonly string[] Components =
    {
        "database-text",
        "npc-names",
        "item-names",
        "monster-names"
    };

    public static int Export(Envir envir, string outputDirectory, string mappingDirectory)
    {
        if (!Directory.Exists(mappingDirectory))
        {
            Console.Error.WriteLine($"Mapping directory not found: {mappingDirectory}");
            return 1;
        }

        var root = Path.GetFullPath(outputDirectory);
        Directory.CreateDirectory(root);

        var fields = CollectDatabaseFields(envir);
        var history = LoadMappingHistory(mappingDirectory);
        var manifestPath = Path.Combine(root, ManifestFileName);
        var existingManifest = File.Exists(manifestPath) ? ReadManifest(manifestPath) : null;
        var existingManifestEntries = existingManifest?.Entries.ToDictionary(x => x.Id, StringComparer.Ordinal) ?? new Dictionary<string, CatalogManifestEntry>(StringComparer.Ordinal);
        var existingPoEntries = LoadPoEntriesIfPresent(root)
            .Where(x => !string.IsNullOrWhiteSpace(x.Context))
            .GroupBy(x => x.Context, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);

        var manifestEntries = new List<CatalogManifestEntry>(fields.Count);
        var newSourceCount = 0;

        foreach (var field in fields)
        {
            var id = CatalogId(field.Type, field.Index, field.Field);
            string source;

            if (existingManifestEntries.TryGetValue(id, out var existing))
            {
                source = existing.Source;
            }
            else
            {
                source = ResolveOriginalSource(field, history);
                newSourceCount++;
            }

            manifestEntries.Add(new CatalogManifestEntry
            {
                Id = id,
                Component = field.Component,
                Type = field.Type,
                Index = field.Index,
                Field = field.Field,
                Source = source
            });
        }

        manifestEntries = manifestEntries
            .OrderBy(x => Array.IndexOf(Components, x.Component))
            .ThenBy(x => x.Type, StringComparer.Ordinal)
            .ThenBy(x => x.Index)
            .ThenBy(x => x.Field, StringComparer.Ordinal)
            .ToList();

        var manifest = new CatalogManifest
        {
            Version = ManifestVersion,
            SourceLanguage = "en",
            TargetLanguage = "zh-CN",
            DatabaseFieldCount = manifestEntries.Count,
            SourceFingerprint = ComputeSourceFingerprint(manifestEntries),
            Entries = manifestEntries
        };

        WriteManifest(manifestPath, manifest);

        var fieldById = fields.ToDictionary(x => CatalogId(x.Type, x.Index, x.Field), StringComparer.Ordinal);
        var pendingTargets = 0;

        foreach (var component in Components)
        {
            var componentDirectory = Path.Combine(root, component);
            Directory.CreateDirectory(componentDirectory);
            var poPath = Path.Combine(componentDirectory, TargetFileName);
            var entries = new List<PoEntry>();

            foreach (var manifestEntry in manifestEntries.Where(x => x.Component == component))
            {
                var field = fieldById[manifestEntry.Id];
                var target = field.Value == manifestEntry.Source ? "" : field.Value;
                var flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var translatorComments = new List<string>();

                if (existingPoEntries.TryGetValue(manifestEntry.Id, out var existingPo) && existingPo.Source == manifestEntry.Source)
                {
                    target = existingPo.Target;
                    flags = new HashSet<string>(existingPo.Flags, StringComparer.OrdinalIgnoreCase);
                    translatorComments = existingPo.TranslatorComments.ToList();

                    if (!string.IsNullOrWhiteSpace(target) && target != field.Value)
                        pendingTargets++;
                }

                entries.Add(new PoEntry
                {
                    Context = manifestEntry.Id,
                    Source = manifestEntry.Source,
                    Target = target,
                    Component = component,
                    Flags = flags,
                    TranslatorComments = translatorComments
                });
            }

            WritePoFile(poPath, entries, manifestEntries.ToDictionary(x => x.Id, StringComparer.Ordinal));
            var translated = entries.Count(x => !string.IsNullOrWhiteSpace(x.Target) && !x.Flags.Contains("fuzzy"));
            Console.WriteLine($"Exported {component}: {translated}/{entries.Count} translated.");
        }

        Console.WriteLine($"Manifest: {manifestEntries.Count} fields, source fingerprint {manifest.SourceFingerprint}.");
        Console.WriteLine($"New source entries: {newSourceCount}; preserved pending Weblate targets: {pendingTargets}.");
        return 0;
    }

    public static int Validate(Envir envir, string catalogPath)
    {
        var result = InspectCatalog(envir, catalogPath);
        PrintValidation(result);
        return result.Errors.Count == 0 ? 0 : 1;
    }

    public static int Apply(Envir envir, string catalogPath, string[] requestedScanRoots)
    {
        var result = InspectCatalog(envir, catalogPath);
        PrintValidation(result);

        if (result.Errors.Count > 0)
        {
            Console.Error.WriteLine("Weblate catalog has validation errors; database was not modified.");
            return 1;
        }

        var changes = new List<PendingDatabaseChange>();

        foreach (var entry in result.SelectedEntries.Values)
        {
            if (string.IsNullOrWhiteSpace(entry.Target) || entry.Flags.Contains("fuzzy")) continue;
            if (!result.Fields.TryGetValue(entry.Context, out var field)) continue;
            if (field.Value == entry.Target) continue;

            changes.Add(new PendingDatabaseChange(field, field.Value, entry.Target));
        }

        if (changes.Count == 0)
        {
            Console.WriteLine("Weblate catalog matches the active database; no changes to apply.");
            return 0;
        }

        var nameChanges = changes.Where(x => IsLinkedNameField(x.Field)).ToList();
        List<string> scanRoots;
        try
        {
            scanRoots = ResolveScanRoots(requestedScanRoots);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine("Database was not modified.");
            return 2;
        }

        if (nameChanges.Count > 0 && scanRoots.Count == 0)
        {
            Console.Error.WriteLine("Name changes require at least one existing scan root (normally Envir and Configs); database was not modified.");
            return 2;
        }

        var references = FindBlockingNameReferences(nameChanges, scanRoots);
        if (references.Count > 0)
        {
            Console.Error.WriteLine("Name references require maintainer review; database was not modified.");
            Console.Error.WriteLine("#ID\tCURRENT\tTARGET\tREF_COUNT\tSAMPLES");

            foreach (var group in references.GroupBy(x => x.Change.Field.Id, StringComparer.Ordinal).OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                var first = group.First();
                var samples = string.Join(" | ", group.Take(5).Select(x => $"{x.Root}/{x.RelativePath}:{x.Line}"));
                Console.Error.WriteLine($"{group.Key}\t{EscapeTab(first.Change.Current)}\t{EscapeTab(first.Change.Target)}\t{group.Count()}\t{samples}");
            }

            Console.Error.WriteLine("Review these references and use the existing TSV mapping workflow with REFERENCE_PREFIXES for this batch.");
            return 3;
        }

        var databasePath = Path.GetFullPath("Server.MirDB");
        if (!File.Exists(databasePath))
        {
            Console.Error.WriteLine($"Active database not found: {databasePath}");
            return 1;
        }

        var backupDirectory = Path.Combine(Path.GetDirectoryName(databasePath)!, "Backups", $"weblate-{DateTime.Now:yyyyMMdd-HHmmss-fff}");
        Directory.CreateDirectory(backupDirectory);
        var backupPath = Path.Combine(backupDirectory, "Server.MirDB.bak");
        File.Copy(databasePath, backupPath, false);

        var applied = 0;
        foreach (var change in changes)
        {
            if (!SetDatabaseField(envir, change.Field, change.Current, change.Target))
                throw new InvalidOperationException($"Database changed during apply: {change.Field.Id}");
            applied++;
        }

        envir.SaveDB();
        Console.WriteLine($"Applied {applied} Weblate database changes.");
        Console.WriteLine($"Backup: {backupPath}");
        return 0;
    }

    private static CatalogInspection InspectCatalog(Envir envir, string catalogPath)
    {
        var inspection = new CatalogInspection();
        string root;

        try
        {
            root = FindCatalogRoot(catalogPath);
        }
        catch (Exception ex)
        {
            inspection.Errors.Add(ex.Message);
            return inspection;
        }

        var manifestPath = Path.Combine(root, ManifestFileName);
        CatalogManifest manifest;

        try
        {
            manifest = ReadManifest(manifestPath);
        }
        catch (Exception ex)
        {
            inspection.Errors.Add($"Could not read manifest: {ex.Message}");
            return inspection;
        }

        inspection.Manifest = manifest;
        if (manifest.Version != ManifestVersion)
            inspection.Errors.Add($"Unsupported manifest version: {manifest.Version}.");
        if (manifest.DatabaseFieldCount != manifest.Entries.Count)
            inspection.Errors.Add($"Manifest field count is {manifest.DatabaseFieldCount}, but it contains {manifest.Entries.Count} entries.");

        foreach (var entry in manifest.Entries)
        {
            if (entry.Id != CatalogId(entry.Type, entry.Index, entry.Field))
                inspection.Errors.Add($"Manifest stable key does not match its fields: {entry.Id}.");
            if (!Components.Contains(entry.Component, StringComparer.Ordinal))
                inspection.Errors.Add($"Unknown manifest component for {entry.Id}: {entry.Component}.");
            else if (entry.Component != ComponentOf(entry.Type, entry.Field))
                inspection.Errors.Add($"Manifest component does not match field type for {entry.Id}.");
        }

        var duplicateManifestIds = manifest.Entries.GroupBy(x => x.Id, StringComparer.Ordinal).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        foreach (var id in duplicateManifestIds)
            inspection.Errors.Add($"Duplicate manifest ID: {id}.");

        var fingerprint = ComputeSourceFingerprint(manifest.Entries);
        if (!string.Equals(fingerprint, manifest.SourceFingerprint, StringComparison.OrdinalIgnoreCase))
            inspection.Errors.Add("Manifest source fingerprint does not match its entries.");

        var manifestById = manifest.Entries
            .GroupBy(x => x.Id, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
        var fields = CollectDatabaseFields(envir);
        inspection.Fields = fields.ToDictionary(x => x.Id, StringComparer.Ordinal);

        List<PoEntry> poEntries;
        try
        {
            poEntries = LoadSelectedPoEntries(catalogPath, root);
        }
        catch (Exception ex)
        {
            inspection.Errors.Add($"Could not read PO catalog: {ex.Message}");
            return inspection;
        }

        foreach (var duplicate in poEntries.Where(x => !string.IsNullOrWhiteSpace(x.Context)).GroupBy(x => x.Context, StringComparer.Ordinal).Where(x => x.Count() > 1))
            inspection.Errors.Add($"Duplicate PO context: {duplicate.Key}.");

        foreach (var entry in poEntries)
        {
            if (string.IsNullOrWhiteSpace(entry.Context))
            {
                inspection.Errors.Add($"PO entry without msgctxt in {entry.SourcePath}.");
                continue;
            }

            if (!CatalogIdRegex.IsMatch(entry.Context))
            {
                inspection.Errors.Add($"Invalid stable key: {entry.Context}.");
                continue;
            }

            if (!manifestById.TryGetValue(entry.Context, out var manifestEntry))
            {
                inspection.Errors.Add($"PO entry is not present in manifest: {entry.Context}.");
                continue;
            }

            if (entry.Component != manifestEntry.Component)
                inspection.Errors.Add($"Component mismatch for {entry.Context}: expected {manifestEntry.Component}, found {entry.Component}.");
            if (entry.Source != manifestEntry.Source)
                inspection.Errors.Add($"Source text changed for {entry.Context}; edit msgstr, not msgid.");
            if (!inspection.Fields.ContainsKey(entry.Context))
                inspection.Errors.Add($"Database field no longer exists: {entry.Context}.");

            if (!string.IsNullOrWhiteSpace(entry.Target) && !PlaceholdersMatch(entry.Source, entry.Target))
                inspection.Errors.Add($"Placeholder mismatch for {entry.Context}.");

            inspection.SelectedEntries[entry.Context] = entry;
        }

        var inputIsFullCatalog = Directory.Exists(catalogPath)
                                 && PathsEqual(Path.GetFullPath(catalogPath), root);
        if (inputIsFullCatalog)
        {
            foreach (var missing in manifest.Entries.Where(x => !inspection.SelectedEntries.ContainsKey(x.Id)).Take(50))
                inspection.Errors.Add($"Missing PO entry: {missing.Id}.");

            if (manifest.Entries.Count(x => !inspection.SelectedEntries.ContainsKey(x.Id)) > 50)
                inspection.Errors.Add("More than 50 PO entries are missing; remaining messages omitted.");
        }

        foreach (var manifestEntry in manifest.Entries)
        {
            if (!inspection.Fields.ContainsKey(manifestEntry.Id))
                inspection.Errors.Add($"Manifest field no longer exists in database: {manifestEntry.Id}.");
        }

        inspection.Translated = inspection.SelectedEntries.Values.Count(x => !string.IsNullOrWhiteSpace(x.Target) && !x.Flags.Contains("fuzzy"));
        inspection.NeedsEditing = inspection.SelectedEntries.Values.Count(x => !string.IsNullOrWhiteSpace(x.Target) && x.Flags.Contains("fuzzy"));
        inspection.Untranslated = inspection.SelectedEntries.Count - inspection.Translated - inspection.NeedsEditing;
        return inspection;
    }

    private static void PrintValidation(CatalogInspection inspection)
    {
        foreach (var error in inspection.Errors)
            Console.Error.WriteLine($"ERROR: {error}");
        foreach (var warning in inspection.Warnings)
            Console.Error.WriteLine($"WARNING: {warning}");

        Console.WriteLine($"Catalog entries: {inspection.SelectedEntries.Count}; translated: {inspection.Translated}; needs editing: {inspection.NeedsEditing}; untranslated: {inspection.Untranslated}.");
        Console.WriteLine($"Validation errors: {inspection.Errors.Count}; warnings: {inspection.Warnings.Count}.");
    }

    private static List<DatabaseField> CollectDatabaseFields(Envir envir)
    {
        var fields = new List<DatabaseField>();

        foreach (MagicInfo info in envir.MagicInfoList.OrderBy(x => x.Spell))
            AddField(fields, "Magic", (int)info.Spell, "Name", info.Name);

        foreach (MapInfo info in envir.MapInfoList.OrderBy(x => x.Index))
            AddField(fields, "Map", info.Index, "Title", info.Title);

        foreach (QuestInfo info in envir.QuestInfoList.OrderBy(x => x.Index))
        {
            AddField(fields, "Quest", info.Index, "Name", info.Name);
            AddField(fields, "Quest", info.Index, "Group", info.Group);
            AddField(fields, "Quest", info.Index, "GotoMessage", info.GotoMessage);
            AddField(fields, "Quest", info.Index, "KillMessage", info.KillMessage);
            AddField(fields, "Quest", info.Index, "ItemMessage", info.ItemMessage);
            AddField(fields, "Quest", info.Index, "FlagMessage", info.FlagMessage);
        }

        foreach (ItemInfo info in envir.ItemInfoList.OrderBy(x => x.Index))
        {
            AddField(fields, "Item", info.Index, "Name", info.Name);
            AddField(fields, "Item", info.Index, "ToolTip", info.ToolTip);
        }

        foreach (MonsterInfo info in envir.MonsterInfoList.OrderBy(x => x.Index))
            AddField(fields, "Monster", info.Index, "Name", info.Name);

        foreach (NPCInfo info in envir.NPCInfoList.OrderBy(x => x.Index))
            AddField(fields, "NPC", info.Index, "Name", info.Name);

        return fields;
    }

    private static void AddField(List<DatabaseField> fields, string type, int index, string field, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        fields.Add(new DatabaseField(type, index, field, value, ComponentOf(type, field)));
    }

    private static string ComponentOf(string type, string field)
    {
        if (type == "NPC" && field == "Name") return "npc-names";
        if (type == "Item" && field == "Name") return "item-names";
        if (type == "Monster" && field == "Name") return "monster-names";
        return "database-text";
    }

    private static List<CatalogMapping> LoadMappingHistory(string mappingDirectory)
    {
        var mappings = new List<CatalogMapping>();

        foreach (var path in Directory.EnumerateFiles(mappingDirectory, "localization-phase-*-mapping*.tsv", SearchOption.TopDirectoryOnly).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            foreach (var rawLine in File.ReadLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(rawLine) || rawLine.StartsWith("#")) continue;
                var parts = rawLine.Split('\t');
                if (parts.Length < 5 || !int.TryParse(parts[1], out var index)) continue;

                var type = NormalizeType(parts[0]);
                var source = UnescapeMapping(parts[3]);
                var target = UnescapeMapping(parts[4]);
                if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(target) || source == target) continue;
                mappings.Add(new CatalogMapping(CatalogId(type, index, parts[2]), source, target));
            }
        }

        return mappings;
    }

    private static string ResolveOriginalSource(DatabaseField field, List<CatalogMapping> history)
    {
        var rows = history.Where(x => x.Id == field.Id).ToList();
        var value = field.Value;
        var visited = new HashSet<string>(StringComparer.Ordinal) { value };

        while (true)
        {
            var sources = rows
                .Where(x => x.Target == value)
                .Select(x => x.Source)
                .Distinct(StringComparer.Ordinal)
                .ToList();

            if (sources.Count == 0) return value;
            if (sources.Count > 1)
                throw new InvalidOperationException($"Ambiguous mapping history for {field.Id} at value '{value}'.");

            value = sources[0];
            if (!visited.Add(value))
                throw new InvalidOperationException($"Cyclic mapping history for {field.Id}.");
        }
    }

    private static string NormalizeType(string type) => type switch
    {
        "ItemCandidate" => "Item",
        "MonsterCandidate" => "Monster",
        "NPCCandidate" => "NPC",
        _ => type
    };

    private static string UnescapeMapping(string value)
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

    private static void WriteManifest(string path, CatalogManifest manifest)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        File.WriteAllText(path, JsonSerializer.Serialize(manifest, options) + Environment.NewLine, Utf8NoBom);
    }

    private static CatalogManifest ReadManifest(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("Catalog manifest not found.", path);
        return JsonSerializer.Deserialize<CatalogManifest>(File.ReadAllText(path, Encoding.UTF8))
               ?? throw new InvalidDataException("Catalog manifest is empty.");
    }

    private static string ComputeSourceFingerprint(IEnumerable<CatalogManifestEntry> entries)
    {
        var canonical = string.Join("\n", entries
            .OrderBy(x => x.Id, StringComparer.Ordinal)
            .Select(x => $"{x.Id}\t{x.Component}\t{x.Source.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n")}"));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    private static void WritePoFile(string path, List<PoEntry> entries, Dictionary<string, CatalogManifestEntry> manifestById)
    {
        var builder = new StringBuilder();
        builder.AppendLine("msgid \"\"");
        builder.AppendLine("msgstr \"\"");
        builder.AppendLine(QuotePo("Project-Id-Version: Crystal Mir2 Database\n"));
        builder.AppendLine(QuotePo("Language: zh_CN\n"));
        builder.AppendLine(QuotePo("MIME-Version: 1.0\n"));
        builder.AppendLine(QuotePo("Content-Type: text/plain; charset=UTF-8\n"));
        builder.AppendLine(QuotePo("Content-Transfer-Encoding: 8bit\n"));
        builder.AppendLine(QuotePo("Plural-Forms: nplurals=1; plural=0;\n"));
        builder.AppendLine(QuotePo("X-Generator: LocalizationDbTool\n"));

        foreach (var entry in entries)
        {
            var manifest = manifestById[entry.Context];
            builder.AppendLine();

            foreach (var comment in entry.TranslatorComments)
                builder.AppendLine($"# {comment}");

            builder.AppendLine($"#. 数据库字段：{manifest.Type}[{manifest.Index}].{manifest.Field}");
            if (manifest.Field == "Name" && manifest.Type is "Item" or "Monster" or "NPC")
                builder.AppendLine("#. 名称字段：回写前必须通过 Envir/Configs 引用扫描");
            if (ContainsCjk(manifest.Source))
                builder.AppendLine("#. 上游原文包含中文或东亚字符，请人工确认译义");

            builder.AppendLine($"#: Server.MirDB {manifest.Type}[{manifest.Index}].{manifest.Field}");
            if (entry.Flags.Count > 0)
                builder.AppendLine($"#, {string.Join(", ", entry.Flags.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))}");
            builder.AppendLine($"msgctxt {QuotePo(entry.Context)}");
            builder.AppendLine($"msgid {QuotePo(entry.Source)}");
            builder.AppendLine($"msgstr {QuotePo(entry.Target)}");
        }

        File.WriteAllText(path, builder.ToString(), Utf8NoBom);
    }

    private static string QuotePo(string value)
    {
        var escaped = value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\t", "\\t")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n");
        return $"\"{escaped}\"";
    }

    private static string UnquotePo(string value, string path, int lineNumber)
    {
        value = value.Trim();
        if (value.Length < 2 || value[0] != '"' || value[^1] != '"')
            throw new FormatException($"Invalid PO string at {path}:{lineNumber}.");

        var builder = new StringBuilder(value.Length - 2);
        for (var i = 1; i < value.Length - 1; i++)
        {
            var ch = value[i];
            if (ch != '\\')
            {
                builder.Append(ch);
                continue;
            }

            if (++i >= value.Length - 1)
                throw new FormatException($"Invalid PO escape at {path}:{lineNumber}.");

            builder.Append(value[i] switch
            {
                'n' => '\n',
                'r' => '\r',
                't' => '\t',
                '"' => '"',
                '\\' => '\\',
                _ => value[i]
            });
        }

        return builder.ToString();
    }

    private static List<PoEntry> ParsePoFile(string path, string component)
    {
        var entries = new List<PoEntry>();
        var current = new PoEntry { Component = component, SourcePath = path };
        string activeField = null;
        var hasDirective = false;
        var lines = File.ReadAllLines(path, Encoding.UTF8);

        void Commit()
        {
            if (!hasDirective) return;
            if (!(current.Context == null && current.Source == ""))
                entries.Add(current);
            current = new PoEntry { Component = component, SourcePath = path };
            activeField = null;
            hasDirective = false;
        }

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                Commit();
                continue;
            }

            if (line.StartsWith("#~", StringComparison.Ordinal) || line.StartsWith("#|", StringComparison.Ordinal)) continue;
            if (line.StartsWith("#.", StringComparison.Ordinal))
            {
                current.DeveloperComments.Add(line.Length > 2 ? line[2..].TrimStart() : "");
                continue;
            }
            if (line.StartsWith("#:", StringComparison.Ordinal))
            {
                current.References.AddRange(line[2..].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                continue;
            }
            if (line.StartsWith("#,") )
            {
                foreach (var flag in line[2..].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    current.Flags.Add(flag);
                continue;
            }
            if (line.StartsWith("# ", StringComparison.Ordinal))
            {
                current.TranslatorComments.Add(line[2..]);
                continue;
            }
            if (line.StartsWith("#", StringComparison.Ordinal)) continue;

            if (line.StartsWith("msgid_plural", StringComparison.Ordinal) || line.StartsWith("msgstr[", StringComparison.Ordinal))
                throw new FormatException($"Plural PO entries are not supported at {path}:{i + 1}.");

            if (line.StartsWith("msgctxt ", StringComparison.Ordinal))
            {
                current.Context = UnquotePo(line[8..], path, i + 1);
                activeField = "context";
                hasDirective = true;
                continue;
            }
            if (line.StartsWith("msgid ", StringComparison.Ordinal))
            {
                current.Source = UnquotePo(line[6..], path, i + 1);
                activeField = "source";
                hasDirective = true;
                continue;
            }
            if (line.StartsWith("msgstr ", StringComparison.Ordinal))
            {
                current.Target = UnquotePo(line[7..], path, i + 1);
                activeField = "target";
                hasDirective = true;
                continue;
            }
            if (line.TrimStart().StartsWith("\"", StringComparison.Ordinal))
            {
                var continuation = UnquotePo(line, path, i + 1);
                switch (activeField)
                {
                    case "context": current.Context += continuation; break;
                    case "source": current.Source += continuation; break;
                    case "target": current.Target += continuation; break;
                    default: throw new FormatException($"Unexpected PO continuation at {path}:{i + 1}.");
                }
                continue;
            }

            throw new FormatException($"Unsupported PO syntax at {path}:{i + 1}: {line}");
        }

        Commit();
        return entries;
    }

    private static List<PoEntry> LoadPoEntriesIfPresent(string root)
    {
        if (!Directory.Exists(root)) return new List<PoEntry>();
        var entries = new List<PoEntry>();
        foreach (var path in Directory.EnumerateFiles(root, TargetFileName, SearchOption.AllDirectories))
            entries.AddRange(ParsePoFile(path, Directory.GetParent(path)!.Name));
        return entries;
    }

    private static List<PoEntry> LoadSelectedPoEntries(string catalogPath, string root)
    {
        if (File.Exists(catalogPath))
            return ParsePoFile(Path.GetFullPath(catalogPath), Directory.GetParent(Path.GetFullPath(catalogPath))!.Name);

        if (!Directory.Exists(catalogPath))
            throw new DirectoryNotFoundException($"Catalog path not found: {catalogPath}");

        var paths = Directory.EnumerateFiles(Path.GetFullPath(catalogPath), TargetFileName, SearchOption.AllDirectories).ToList();
        if (paths.Count == 0)
            throw new FileNotFoundException($"No {TargetFileName} files found under {catalogPath}.");

        var entries = new List<PoEntry>();
        foreach (var path in paths)
            entries.AddRange(ParsePoFile(path, Directory.GetParent(path)!.Name));
        return entries;
    }

    private static string FindCatalogRoot(string catalogPath)
    {
        var start = File.Exists(catalogPath)
            ? Directory.GetParent(Path.GetFullPath(catalogPath))
            : new DirectoryInfo(Path.GetFullPath(catalogPath));

        for (var directory = start; directory != null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, ManifestFileName)))
                return directory.FullName;
        }

        throw new FileNotFoundException($"Could not find {ManifestFileName} from {catalogPath}.");
    }

    private static bool PlaceholdersMatch(string source, string target)
    {
        var sourceTokens = ExtractPlaceholders(source);
        var targetTokens = ExtractPlaceholders(target);
        return sourceTokens.SequenceEqual(targetTokens, StringComparer.Ordinal);
    }

    private static List<string> ExtractPlaceholders(string value)
    {
        return NumericPlaceholderRegex.Matches(value).Select(x => x.Value)
            .Concat(PrintfPlaceholderRegex.Matches(value).Select(x => x.Value))
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }

    private static bool ContainsCjk(string value) => value.Any(ch => ch is >= '\u3400' and <= '\u9fff');

    private static List<string> ResolveScanRoots(string[] requestedRoots)
    {
        var roots = new List<string>();
        if (requestedRoots.Length > 0)
        {
            foreach (var root in requestedRoots)
            {
                if (!Directory.Exists(root))
                    throw new DirectoryNotFoundException($"Scan root not found: {root}");
                roots.Add(Path.GetFullPath(root));
            }
            return roots;
        }

        foreach (var candidate in new[] { "Envir", "Configs" })
            if (Directory.Exists(candidate)) roots.Add(Path.GetFullPath(candidate));
        return roots;
    }

    private static List<BlockingNameReference> FindBlockingNameReferences(List<PendingDatabaseChange> changes, List<string> scanRoots)
    {
        var references = new List<BlockingNameReference>();
        var files = new List<CatalogScanFile>();

        foreach (var root in scanRoots)
        {
            foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).Where(IsTextCandidate))
                files.Add(new CatalogScanFile(root, path, Path.GetRelativePath(root, path).Replace('\\', '/'), File.ReadAllLines(path, Encoding.UTF8)));
        }

        foreach (var change in changes)
        {
            var regex = BuildNameRegex(change.Current);
            foreach (var file in files)
            {
                for (var i = 0; i < file.Lines.Length; i++)
                {
                    if (!regex.IsMatch(file.Lines[i])) continue;
                    references.Add(new BlockingNameReference(change, Path.GetFileName(file.Root), file.RelativePath, i + 1));
                }
            }
        }

        return references;
    }

    private static bool IsTextCandidate(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension != ".txt" && extension != ".ini" && extension != ".info" && extension != ".cfg") return false;
        return !path.Replace('\\', '/').Contains("/Previews/", StringComparison.OrdinalIgnoreCase);
    }

    private static Regex BuildNameRegex(string name)
    {
        return new Regex($@"(?<![A-Za-z0-9_]){Regex.Escape(name)}(?![A-Za-z0-9_])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    private static bool IsLinkedNameField(DatabaseField field)
    {
        return field.Field == "Name" && field.Type is "Item" or "Monster" or "NPC";
    }

    private static bool SetDatabaseField(Envir envir, DatabaseField field, string current, string target)
    {
        switch (field.Type)
        {
            case "Magic" when field.Field == "Name":
                var magic = envir.MagicInfoList.FirstOrDefault(x => (int)x.Spell == field.Index && x.Name == current);
                if (magic == null) return false;
                magic.Name = target;
                return true;

            case "Map" when field.Field == "Title":
                var map = envir.MapInfoList.FirstOrDefault(x => x.Index == field.Index && x.Title == current);
                if (map == null) return false;
                map.Title = target;
                return true;

            case "Quest":
                var quest = envir.QuestInfoList.FirstOrDefault(x => x.Index == field.Index);
                if (quest == null) return false;
                return SetQuestField(quest, field.Field, current, target);

            case "Item" when field.Field == "Name":
                var item = envir.ItemInfoList.FirstOrDefault(x => x.Index == field.Index && x.Name == current);
                if (item == null) return false;
                item.Name = target;
                return true;

            case "Item" when field.Field == "ToolTip":
                item = envir.ItemInfoList.FirstOrDefault(x => x.Index == field.Index && x.ToolTip == current);
                if (item == null) return false;
                item.ToolTip = target;
                return true;

            case "Monster" when field.Field == "Name":
                var monster = envir.MonsterInfoList.FirstOrDefault(x => x.Index == field.Index && x.Name == current);
                if (monster == null) return false;
                monster.Name = target;
                return true;

            case "NPC" when field.Field == "Name":
                var npc = envir.NPCInfoList.FirstOrDefault(x => x.Index == field.Index && x.Name == current);
                if (npc == null) return false;
                npc.Name = target;
                return true;

            default:
                return false;
        }
    }

    private static bool SetQuestField(QuestInfo quest, string field, string current, string target)
    {
        switch (field)
        {
            case "Name" when quest.Name == current: quest.Name = target; return true;
            case "Group" when quest.Group == current: quest.Group = target; return true;
            case "GotoMessage" when quest.GotoMessage == current: quest.GotoMessage = target; return true;
            case "KillMessage" when quest.KillMessage == current: quest.KillMessage = target; return true;
            case "ItemMessage" when quest.ItemMessage == current: quest.ItemMessage = target; return true;
            case "FlagMessage" when quest.FlagMessage == current: quest.FlagMessage = target; return true;
            default: return false;
        }
    }

    private static string CatalogId(string type, int index, string field) => $"{NormalizeType(type)}:{index}:{field}";
    private static string EscapeTab(string value) => value.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");

    private static bool PathsEqual(string left, string right)
    {
        return string.Equals(left.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), right.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase);
    }
}

internal sealed class CatalogManifest
{
    public int Version { get; set; }
    public string SourceLanguage { get; set; } = "en";
    public string TargetLanguage { get; set; } = "zh-CN";
    public int DatabaseFieldCount { get; set; }
    public string SourceFingerprint { get; set; } = "";
    public List<CatalogManifestEntry> Entries { get; set; } = new();
}

internal sealed class CatalogManifestEntry
{
    public string Id { get; set; } = "";
    public string Component { get; set; } = "";
    public string Type { get; set; } = "";
    public int Index { get; set; }
    public string Field { get; set; } = "";
    public string Source { get; set; } = "";
}

internal sealed class PoEntry
{
    public string Context { get; set; }
    public string Source { get; set; } = "";
    public string Target { get; set; } = "";
    public string Component { get; set; } = "";
    public string SourcePath { get; set; } = "";
    public HashSet<string> Flags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> TranslatorComments { get; set; } = new();
    public List<string> DeveloperComments { get; set; } = new();
    public List<string> References { get; set; } = new();
}

internal sealed class CatalogInspection
{
    public CatalogManifest Manifest { get; set; }
    public Dictionary<string, DatabaseField> Fields { get; set; } = new(StringComparer.Ordinal);
    public Dictionary<string, PoEntry> SelectedEntries { get; set; } = new(StringComparer.Ordinal);
    public List<string> Errors { get; } = new();
    public List<string> Warnings { get; } = new();
    public int Translated { get; set; }
    public int NeedsEditing { get; set; }
    public int Untranslated { get; set; }
}

internal sealed record DatabaseField(string Type, int Index, string Field, string Value, string Component)
{
    public string Id => $"{Type}:{Index}:{Field}";
}

internal sealed record CatalogMapping(string Id, string Source, string Target);
internal sealed record PendingDatabaseChange(DatabaseField Field, string Current, string Target);
internal sealed record CatalogScanFile(string Root, string Path, string RelativePath, string[] Lines);
internal sealed record BlockingNameReference(PendingDatabaseChange Change, string Root, string RelativePath, int Line);
