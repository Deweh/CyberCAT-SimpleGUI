using System.Text.Json;
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
        private static string _gameDir = @"C:\Games\Steam\steamapps\common\Cyberpunk 2077";

        static void Main(string[] args)
        {
            GenerateTweakFiles(Path.Combine(_gameDir, "r6", "cache", "tweakdb_ep1.bin"));
            //GenerateAppearanceValues(Path.Combine(_gameDir, "archive", "pc", "content", "basegame_1_engine.archive"));
        }

        private static void GenerateTweakFiles(string tweakPath)
        {
            var stringHelper = new TweakDBStringHelper();
            stringHelper.LoadFromStream(typeof(TweakDBService).Assembly.GetManifestResourceStream("WolvenKit.Common.Resources.tweakdbstr.kark"));
            TweakDBIDPool.ResolveHashHandler += stringHelper.GetString;

            using var fh = File.OpenRead(tweakPath);
            using var reader = new TweakDBReader(fh);

            if (reader.ReadFile(out var tweakDb) == WolvenKit.RED4.TweakDB.EFileReadErrorCodes.NoError)
            {
                //Test(tweakDb!);
                //GenerateModList(tweakDb);
                //GenerateVehicles(tweakDb);
                //GenerateItemClasses(tweakDb);
                GenerateModifierGroupsHash(tweakDb!);
            }
        }

        private record ModifierGroup(string Name, uint CRC, List<string> StatTypes);

        private static void GenerateModifierGroupsHash(TweakDB tweakDb)
        {
            var lst = new List<ModifierGroup>();

            foreach (var (id, type) in tweakDb.Records)
            {
                ArgumentNullException.ThrowIfNull(id.ResolvedText);

                if (!id.ResolvedText.StartsWith("ModifierGroups.") || tweakDb.GetFullRecord(id) is not gamedataStatModifierGroup_Record record)
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
            var list = new List<string>();

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
                if (resolvedText != null && resolvedText.StartsWith("Vehicle.") && resolvedText.EndsWith("_player"))
                {
                    list.Add(resolvedText);
                }
            }

            list.Sort();
            File.WriteAllText("Vehicles.json", JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
        }

        public record SlotPartRecord(ulong ItemPartPreset, ulong Slot);
        public record ItemRecord(string Type, bool IsSingleInstance, int IsItemPlus, List<SlotPartRecord> SlotParts);

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

                dict.Add(id, new ItemRecord(itemType, record.IsSingleInstance, isItemPlus, presets));
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