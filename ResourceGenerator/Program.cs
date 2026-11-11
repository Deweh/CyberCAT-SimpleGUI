using System.Text.Json;
using System.Text.Json.Serialization;
using WolvenKit.Common.Services;
using WolvenKit.Core.CRC;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.TweakDB;
using WolvenKit.RED4.TweakDB.Helper;
using WolvenKit.RED4.Types;
using EFileReadErrorCodes = WolvenKit.RED4.Archive.IO.EFileReadErrorCodes;

namespace ResourceGenerator
{
    internal class Program
    {
        private static string _gameDir = @"C:\Program Files (x86)\Steam\steamapps\common\Cyberpunk 2077";

        // Usage: ResourceGenerator [gameDir] [generator ...] [--out <folder>]
        // Generators: vehicles, mods, items, modifiergroups, names, perks, cyberware, or "all" for every file the editor uses.
        // The json files are written to --out (e.g. CP2077SaveEditor/Resources) or the current directory.
        static void Main(string[] args)
        {
            var arguments = args.ToList();

            if (arguments.Contains("--help") || arguments.Contains("-h"))
            {
                PrintUsage();
                return;
            }

            var outIndex = arguments.IndexOf("--out");
            if (outIndex >= 0)
            {
                if (outIndex + 1 >= arguments.Count)
                {
                    Console.WriteLine("--out needs a folder");
                    return;
                }

                Directory.CreateDirectory(arguments[outIndex + 1]);
                Directory.SetCurrentDirectory(arguments[outIndex + 1]);
                arguments.RemoveRange(outIndex, 2);
            }

            var gameDir = arguments.Count > 0 ? arguments[0] : _gameDir;
            var generators = arguments.Skip(1).Select(x => x.ToLowerInvariant()).ToHashSet();
            if (generators.Count == 0)
            {
                generators.Add("modifiergroups");
            }

            if (generators.Remove("all"))
            {
                // "mods" is left out, the editor doesn't use Mods.json
                generators.UnionWith(new[] { "vehicles", "items", "modifiergroups", "names", "perks", "cyberware" });
            }

            Console.WriteLine($"Writing {string.Join(", ", generators)} to {Directory.GetCurrentDirectory()}");
            GenerateTweakFiles(gameDir, generators);
            //GenerateAppearanceValues(Path.Combine(gameDir, "archive", "pc", "content", "basegame_1_engine.archive"));
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: ResourceGenerator <game folder> <generator ...> [--out <folder>]");
            Console.WriteLine("Generators: vehicles, mods, items, modifiergroups, names, perks, cyberware, all");
            Console.WriteLine(@"Example: ResourceGenerator ""C:\Program Files (x86)\Steam\steamapps\common\Cyberpunk 2077"" all --out CP2077SaveEditor/Resources");
        }

        private static void GenerateTweakFiles(string gameDir, HashSet<string> generators)
        {
            var tweakPath = Path.Combine(gameDir, "r6", "cache", "tweakdb_ep1.bin");
            if (!File.Exists(tweakPath))
            {
                Console.WriteLine($"{tweakPath} not found, is \"{gameDir}\" the game folder?");
                PrintUsage();
                return;
            }

            var stringHelper = new TweakDBStringHelper();
            stringHelper.LoadFromStream(typeof(TweakDBService).Assembly.GetManifestResourceStream("WolvenKit.Common.Resources.tweakdbstr.kark"));

            // Registers the native TweakDB names, so GetResolvedText works
            _ = new HashService();

            using var fh = File.OpenRead(tweakPath);
            using var reader = new TweakDBReader(fh);

            var status = reader.ReadFile(out var tweakDb);
            if (status != WolvenKit.RED4.TweakDB.EFileReadErrorCodes.NoError)
            {
                Console.WriteLine($"Failed to read {tweakPath}: {status}");
                return;
            }

            //Test(tweakDb!);
            if (generators.Contains("mods")) GenerateModList(tweakDb!);
            if (generators.Contains("vehicles")) GenerateVehicles(tweakDb!);
            if (generators.Contains("items")) GenerateItemClasses(tweakDb!);
            if (generators.Contains("modifiergroups")) GenerateModifierGroupsHash(tweakDb!);
            if (generators.Contains("names")) GenerateDisplayNames(tweakDb!, gameDir);
            if (generators.Contains("perks")) GeneratePerks(tweakDb!, gameDir);
            if (generators.Contains("cyberware")) GenerateCyberware(tweakDb!);
        }

        // Writes Cyberware.json: { item hash: equipment area } for every cyberware item, e.g. "EyesCW"
        private static void GenerateCyberware(TweakDB tweakDb)
        {
            var result = new SortedDictionary<ulong, string>();

            foreach (var (id, type) in tweakDb.Records)
            {
                if (!type.IsAssignableTo(typeof(gamedataItem_Record)) || tweakDb.GetFullRecord(id) is not gamedataItem_Record record)
                {
                    continue;
                }

                // EquipmentArea.EyesCW -> EyesCW
                var area = record.EquipArea.GetResolvedText()?.Replace("EquipmentArea.", "");
                if (area != null && area.EndsWith("CW") && Enum.TryParse<Enums.gamedataEquipmentArea>(area, out _))
                {
                    result[(ulong)id] = area;
                }
            }

            Console.WriteLine($"Cyberware: {result.Count} items");
            File.WriteAllText("Cyberware.json", JsonSerializer.Serialize(result));
        }

        private record PerkInfo(string Type, string Attribute, int MaxLevel, string Tier, Dictionary<string, string> Names);

        // Writes Perks.json: every 2.0 perk with its attribute tree, number of levels and localized name
        private static void GeneratePerks(TweakDB tweakDb, string gameDir)
        {
            var texts = s_nameLanguages.ToDictionary(x => x.Language, x => LoadTexts(gameDir, x.ArchiveCode));
            var perks = new List<PerkInfo>();

            foreach (var (id, type) in tweakDb.Records)
            {
                if (!type.IsAssignableTo(typeof(gamedataNewPerk_Record)) || tweakDb.GetFullRecord(id) is not gamedataNewPerk_Record record)
                {
                    continue;
                }

                // A few records are cyberware abilities, not perks of the attribute trees
                if (record.Levels.Count == 0 || record.EnumName == "None")
                {
                    continue;
                }

                var names = new Dictionary<string, string>();
                var locName = ((string?)record.Loc_name_key) ?? "";
                if (locName.StartsWith("LocKey#") && ulong.TryParse(locName["LocKey#".Length..], out var locKey))
                {
                    foreach (var (language, languageTexts) in texts)
                    {
                        if (languageTexts.TryGetValue(locKey, out var text) && !string.IsNullOrWhiteSpace(text))
                        {
                            names[language] = text;
                        }
                    }
                }

                // NewPerks.BodyAttributeData -> Body
                var attribute = record.Attribute.GetResolvedText()?.Replace("NewPerks.", "").Replace("AttributeData", "") ?? "";
                var tier = record.Tier.GetResolvedText()?.Replace("NewPerks.", "").Replace("NewPerkTier", "") ?? "";

                perks.Add(new PerkInfo(record.EnumName, attribute, record.Levels.Count, tier, names));
            }

            perks = perks.OrderBy(x => x.Attribute).ThenBy(x => x.Type).ToList();
            Console.WriteLine($"Perks: {perks.Count}");

            File.WriteAllText("Perks.json", JsonSerializer.Serialize(perks, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        }

        private static Dictionary<ulong, string> LoadTexts(string gameDir, string archiveCode)
        {
            var texts = new Dictionary<ulong, string>();
            foreach (var folder in new[] { "content", "ep1" })
            {
                var archivePath = Path.Combine(gameDir, "archive", "pc", folder, $"lang_{archiveCode}_text.archive");
                if (File.Exists(archivePath))
                {
                    ReadOnScreenTexts(archivePath, texts);
                }
            }

            return texts;
        }

        // Game language code -> folder used in the localization archives
        private static readonly (string Language, string ArchiveCode)[] s_nameLanguages =
        {
            ("en-us", "en"),
            ("pt-br", "pt"),
        };

        // Writes DisplayNames.json: { language: { record hash: localized name } } for items and vehicles
        private static void GenerateDisplayNames(TweakDB tweakDb, string gameDir)
        {
            var locKeys = new Dictionary<ulong, ulong>();
            foreach (var (id, type) in tweakDb.Records)
            {
                if (!type.IsAssignableTo(typeof(gamedataItem_Record)) && !type.IsAssignableTo(typeof(gamedataVehicle_Record)))
                {
                    continue;
                }

                var displayName = tweakDb.GetFullRecord(id) switch
                {
                    gamedataItem_Record item => item.DisplayName,
                    gamedataVehicle_Record vehicle => vehicle.DisplayName,
                    _ => null
                };

                if (displayName != null && displayName.Key != 0)
                {
                    locKeys[(ulong)id] = displayName.Key;
                }
            }

            var result = new Dictionary<string, SortedDictionary<ulong, string>>();
            foreach (var (language, archiveCode) in s_nameLanguages)
            {
                var texts = LoadTexts(gameDir, archiveCode);

                var names = new SortedDictionary<ulong, string>();
                foreach (var (recordId, locKey) in locKeys)
                {
                    if (texts.TryGetValue(locKey, out var text) && !string.IsNullOrWhiteSpace(text))
                    {
                        names[recordId] = text;
                    }
                }

                result[language] = names;
                Console.WriteLine($"DisplayNames: {language} {names.Count}/{locKeys.Count} names");
            }

            File.WriteAllText("DisplayNames.json", JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                // Keep accented characters readable in the file
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));
        }

        private static void ReadOnScreenTexts(string archivePath, Dictionary<ulong, string> texts)
        {
            _hashService ??= new HashService();

            if (new ArchiveReader().ReadArchive(archivePath, _hashService, out var archive) != EFileReadErrorCodes.NoError)
            {
                Console.WriteLine($"Failed to read {archivePath}");
                return;
            }

            // onscreens_final.json patches onscreens.json, so read it last
            var files = archive.Files.Values
                .Where(x => x.FileName != null && x.FileName.Contains(@"\onscreens\") && x.FileName.EndsWith(".json"))
                .OrderBy(x => x.FileName.EndsWith("_final.json"));

            foreach (var file in files)
            {
                var ms = new MemoryStream();
                file.Extract(ms);
                ms.Position = 0;

                using var reader = new CR2WReader(ms);
                if (reader.ReadFile(out var cr2w) != EFileReadErrorCodes.NoError ||
                    cr2w.RootChunk is not JsonResource { Root.Chunk: localizationPersistenceOnScreenEntries entries })
                {
                    continue;
                }

                foreach (var entry in entries.Entries)
                {
                    // The female variant is the default text, the male one is only set when it differs
                    var text = string.IsNullOrEmpty(entry.FemaleVariant) ? (string)entry.MaleVariant : (string)entry.FemaleVariant;
                    texts[entry.PrimaryKey] = text;
                }
            }
        }

        private record ModifierGroup(string Name, uint CRC, List<string> StatTypes);

        private static void GenerateModifierGroupsHash(TweakDB tweakDb)
        {
            var lst = new List<ModifierGroup>();

            foreach (var (id, type) in tweakDb.Records)
            {
                // Records added in newer patches may not have a known name yet
                if (id.ResolvedText == null || !id.ResolvedText.StartsWith("ModifierGroups.") || tweakDb.GetFullRecord(id) is not gamedataStatModifierGroup_Record record)
                {
                    continue;
                }

                var statTypes = new List<string>();

                foreach (var statModifierId in record.StatModifiers)
                {
                    var statModifier = (gamedataStatModifier_Record)tweakDb.GetFullRecord(statModifierId)!;
                    var statType = statModifier.StatType.ResolvedText![10..];

                    if (!statTypes.Contains(statType))
                    {
                        statTypes.Add(statType);
                    }
                }

                statTypes.Sort();

                lst.Add(new ModifierGroup(id.ResolvedText[15..], Crc32Algorithm.Compute(id.ResolvedText), statTypes));
            }

            lst = lst.OrderBy(x => x.Name).ToList();

            File.WriteAllText("ModifierGroups.json", JsonSerializer.Serialize(lst, new JsonSerializerOptions { WriteIndented = true }));
        }

        private record RootSlot(string Slot, string Item);

        private static void Test(TweakDB tweakDb)
        {
            var dict = new Dictionary<string, RootSlot>();

            foreach (var (id, type) in tweakDb.Records)
            {
                ArgumentNullException.ThrowIfNull(id.ResolvedText);

                if (id.ResolvedText.StartsWith("Items") && type.IsAssignableTo(typeof(gamedataItem_Record)))
                {
                    gamedataItem_Record fullRecord;
                    try
                    {
                        fullRecord = (gamedataItem_Record)tweakDb.GetFullRecord(id)!;
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                    if (fullRecord.SlotPartList is { Count: > 0 })
                    {
                        var itemPart = (gamedataSlotItemPartListElement_Record)tweakDb.GetFullRecord(fullRecord.SlotPartList[0])!;
                        if (itemPart.ItemPartList is { Count: > 0 })
                        {
                            var tmp = (gamedataItemPartListElement_Record)tweakDb.GetFullRecord(itemPart.ItemPartList[0])!;

                            dict.Add(id.ResolvedText, new RootSlot(itemPart.Slot.ResolvedText!, tmp.Item.ResolvedText!));
                        }

                        if (fullRecord.SlotPartList.Count > 1)
                        {

                        }
                    }
                }
            }
        }

        private static void GenerateModList(TweakDB tweakDb)
        {
            var dict = new Dictionary<string, Dictionary<ulong, List<ulong>>>
                {
                    { "Clothing", new Dictionary<ulong, List<ulong>>() },
                    { "Weapon", new Dictionary<ulong, List<ulong>>() }
                };

            foreach (var (id, type) in tweakDb.Records)
            {
                if (!type.IsAssignableTo(typeof(gamedataItem_Record)))
                {
                    continue;
                }

                if (tweakDb.GetFullRecord(id) is not gamedataItem_Record record)
                {
                    throw new Exception();
                }

                if (record.Tags == null)
                {
                    continue;
                }

                foreach (var tag in record.Tags)
                {
                    if (tag == "FabricEnhancer")
                    {
                        foreach (var placementSlot in record.PlacementSlots)
                        {
                            if (!dict["Clothing"].ContainsKey(placementSlot))
                            {
                                dict["Clothing"].Add(placementSlot, new List<ulong>());
                            }

                            if (!dict["Clothing"][placementSlot].Contains(id))
                            {
                                dict["Clothing"][placementSlot].Add(id);
                            }
                        }
                    }

                    if (tag == "WeaponMod")
                    {
                        foreach (var placementSlot in record.PlacementSlots)
                        {
                            if (!dict["Weapon"].ContainsKey(placementSlot))
                            {
                                dict["Weapon"].Add(placementSlot, new List<ulong>());
                            }

                            if (!dict["Weapon"][placementSlot].Contains(id))
                            {
                                dict["Weapon"][placementSlot].Add(id);
                            }
                        }
                    }
                }
            }

            File.WriteAllText("Mods.json", JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static void GenerateVehicles(TweakDB tweakDb)
        {
            // The shipped list also has hand-picked entries that don't match the filter below, so merge instead of replacing
            var list = File.Exists("Vehicles.json")
                ? JsonSerializer.Deserialize<List<string>>(File.ReadAllText("Vehicles.json"))!
                : new List<string>();
            var unresolved = 0;

            foreach (var (id, type) in tweakDb.Records)
            {
                if (!type.IsAssignableTo(typeof(gamedataVehicle_Record)))
                {
                    continue;
                }

                if (tweakDb.GetFullRecord(id) is not gamedataVehicle_Record record)
                {
                    throw new Exception();
                }

                var resolvedText = id.GetResolvedText();
                if (resolvedText == null)
                {
                    unresolved++;
                }
                // "Vehicle.qXXX_" records are quest-only variants
                else if (resolvedText.StartsWith("Vehicle.") && !resolvedText.StartsWith("Vehicle.q") && resolvedText.EndsWith("_player") && !list.Contains(resolvedText))
                {
                    list.Add(resolvedText);
                }
            }

            if (unresolved > 0)
            {
                Console.WriteLine($"Vehicles: {unresolved} records have no known name (tweakdbstr.kark in WolvenKit is outdated) and were skipped");
            }

            list.Sort();
            File.WriteAllText("Vehicles.json", JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
        }

        public record SlotPartRecord(ulong ItemPartPreset, ulong Slot);
        // Quality: the record's fixed rarity ("Rare", "LegendaryPlus"...), left out when it is rolled on loot (Quality.Random)
        public record ItemRecord(string Type, bool IsSingleInstance, int IsItemPlus, List<SlotPartRecord> SlotParts,
            [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Quality);

        private static bool ModifiesQuality(TweakDB tweakDb, gamedataItem_Record record)
        {
            var quality = (TweakDBID)"BaseStats.Quality";
            var modifiers = (record.StatModifiers ?? new()).ToList();
            foreach (var groupId in record.StatModifierGroups ?? new())
            {
                if (tweakDb.GetFullRecord(groupId) is gamedataStatModifierGroup_Record group && group.StatModifiers != null)
                {
                    modifiers.AddRange(group.StatModifiers);
                }
            }

            return modifiers.Any(x => tweakDb.GetFullRecord(x) is gamedataStatModifier_Record modifier && modifier.StatType == quality);
        }

        private static void GenerateItemClasses(TweakDB tweakDb)
        {
            var dict = new Dictionary<ulong, ItemRecord>();

            foreach (var (id, type) in tweakDb.Records)
            {
                if (!type.IsAssignableTo(typeof(gamedataItem_Record)))
                {
                    continue;
                }

                if (tweakDb.GetFullRecord(id) is not gamedataItem_Record record)
                {
                    throw new Exception();
                }

                var isItemPlus = 0;
                if (record.Quality == "Quality.LegendaryPlusPlus")
                {
                    isItemPlus = 2;
                }
                else if (record.Quality == "Quality.LegendaryPlus")
                {
                    isItemPlus = 1;
                }

                var itemType = type.Name[8..^7];
                if (record.ItemCategory != TweakDBID.Empty)
                {
                    itemType = record.ItemCategory.GetResolvedText()![13..];
                }

                var presets = new List<SlotPartRecord>();

                if (record.SlotPartListPreset is { Count: > 0 })
                {
                    foreach (var slotPartId in record.SlotPartListPreset)
                    {
                        var slotPart = (gamedataSlotItemPartPreset_Record)tweakDb.GetFullRecord(slotPartId)!;

                        if (slotPart.ItemPartPreset == TweakDBID.Empty)
                        {
                            continue;
                        }

                        presets.Add(new SlotPartRecord(slotPart.ItemPartPreset, slotPart.Slot));
                    }
                }

                string? quality = null;
                var qualityName = record.Quality.GetResolvedText();
                // Records that modify Quality themselves roll or scale it, so their quality field isn't the real tier
                if (qualityName != null && qualityName.StartsWith("Quality.") && qualityName != "Quality.Random" && !ModifiesQuality(tweakDb, record))
                {
                    quality = qualityName["Quality.".Length..];
                }

                dict.Add(id, new ItemRecord(itemType, record.IsSingleInstance, isItemPlus, presets, quality));
            }

            dict = dict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            File.WriteAllText("ItemClasses.json", JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
        }

        public class ItemRecord2
        {
            public string? TweakName { get; set; }
            public string TypeName { get; set; } = null!;

            public string? Quality { get; set; }

            public string? FemaleName { get; set; }
            public string? FemaleDescription { get; set; }
        }

        private static void GenerateItemClasses2(TweakDB tweakDb)
        {
            using var fs = File.Open(@"C:\Dev\onscreens_final.json", FileMode.Open);
            using var cr = new CR2WReader(fs);
            if (cr.ReadFile(out var file) != EFileReadErrorCodes.NoError || file?.RootChunk is not JsonResource json)
            {
                return;
            }

            var primaryKeys = new Dictionary<ulong, localizationPersistenceOnScreenEntry>();
            var secondaryKeys = new Dictionary<CString, localizationPersistenceOnScreenEntry>();

            if (json.Root.Chunk is localizationPersistenceOnScreenEntries os)
            {
                foreach (var entry in os.Entries)
                {
                    primaryKeys[entry.PrimaryKey] = entry;
                    secondaryKeys[entry.SecondaryKey] = entry;
                }
            }

            var dict = new List<ItemRecord2>();

            foreach (var (id, type) in tweakDb.Records)
            {
                if (!type.IsAssignableTo(typeof(gamedataItem_Record)))
                {
                    continue;
                }

                if (tweakDb.GetFullRecord(id) is not gamedataItem_Record record)
                {
                    throw new Exception();
                }

                var itemRecord = new ItemRecord2()
                {
                    TweakName = id.ResolvedText,
                    TypeName = type.Name[8..^7]
                };

                if (record.Quality.IsResolvable)
                {
                    itemRecord.Quality = record.Quality.ResolvedText![8..];
                }

                if (record.DisplayName != null && primaryKeys.TryGetValue(record.DisplayName.Key, out var displayName))
                {
                    itemRecord.FemaleName = displayName.FemaleVariant;
                }

                if (record.LocalizedDescription != null && primaryKeys.TryGetValue(record.LocalizedDescription.Key, out var localizedDescription))
                {
                    itemRecord.FemaleDescription = localizedDescription.FemaleVariant;
                }

                if (itemRecord.TweakName != null && itemRecord.TweakName.StartsWith("Items.Preset_Katana"))
                {
                    dict.Add(itemRecord);
                }
            }

            dict = dict.OrderBy(x => x.TweakName).ToList();
            File.WriteAllText("Items.json", JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static HashService? _hashService = null;

        private static T? LoadFile<T>(string archivePath, CName filePath) where T : RedBaseClass
        {
            if (_hashService == null)
            {
                _hashService = new HashService();
            }

            var am = new ArchiveReader();

            if (am.ReadArchive(archivePath, _hashService, out var archive) == EFileReadErrorCodes.NoError)
            {
                var femaleOptions = archive.Files.FirstOrDefault(x => x.Key == filePath).Value;
                if (femaleOptions != null)
                {
                    var ms = new MemoryStream();
                    femaleOptions.Extract(ms);
                    ms.Position = 0;

                    using var cr = new CR2WReader(ms);
                    if (cr.ReadFile(out var cr2w) == EFileReadErrorCodes.NoError)
                    {
                        return (T)cr2w.RootChunk;
                    }
                }
            }

            return null;
        }

        private static void GenerateAppearanceValues(string archivePath)
        {
            var hashService = new HashService();

            // male : 5205075720019286593
            var femaleFile = LoadFile<gameuiCharacterCustomizationInfoResource>(archivePath, 3179227268398507574);
            if (femaleFile != null)
            {
                var dict = new Dictionary<string, List<string>>();

                var headOptions = ParseOptions(femaleFile.HeadCustomizationOptions);
                var armsOptions = ParseOptions(femaleFile.ArmsCustomizationOptions);
                var bodyOptions = ParseOptions(femaleFile.BodyCustomizationOptions);
            }

            Dictionary<string, List<string>> ParseOptions(CArray<CHandle<gameuiCharacterCustomizationInfo>> options)
            {
                var dict = new Dictionary<string, List<string>>();

                foreach (var optionHandle in options)
                {
                    if (optionHandle.Chunk is gameuiSwitcherInfo switcherInfo)
                    {
                        dict.Add(switcherInfo.Name, new List<string>());

                        foreach (var switcherOption in switcherInfo.Options)
                        {
                            dict[switcherInfo.Name].Add(switcherOption.Names[0]);
                        }
                    }
                }

                return dict;
            }
        }
    }
}