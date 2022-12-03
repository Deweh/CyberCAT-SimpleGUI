using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using WolvenKit.Common.Services;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.TweakDB;
using WolvenKit.RED4.TweakDB.Helper;
using WolvenKit.RED4.Types;
using EFileReadErrorCodes = WolvenKit.RED4.Archive.IO.EFileReadErrorCodes;

namespace ResourceGenerator
{
    internal class Program
    {
        private static string _gameDir = @"C:\Games\GOG Galaxy\Cyberpunk 2077";

        static void Main(string[] args)
        {
            GenerateTweakFiles(Path.Combine(_gameDir, "r6", "cache", "tweakdb.bin"));
            GenerateAppearanceValues(Path.Combine(_gameDir, "archive", "pc", "content", "basegame_1_engine.archive"));
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
                GenerateModList(tweakDb);
                GenerateVehicles(tweakDb);
                GenerateItemClasses(tweakDb);
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
                if (id.ResolvedText.StartsWith("Items") && type == typeof(gamedataItem_Record))
                {
                    var fullRecord = (gamedataItem_Record)tweakDb.GetFullRecord(id);
                    if (fullRecord.Tags == null)
                    {
                        continue;
                    }

                    foreach (var tag in fullRecord.Tags)
                    {
                        if (tag == "FabricEnhancer")
                        {
                            foreach (var placementSlot in fullRecord.PlacementSlots)
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
                            foreach (var placementSlot in fullRecord.PlacementSlots)
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
            }

            File.WriteAllText("Mods.json", JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static void GenerateVehicles(TweakDB tweakDb)
        {
            var list = new List<string>();

            foreach (var (id, _) in tweakDb.Records)
            {
                var resolvedText = id.GetResolvedText();
                if (resolvedText != null && resolvedText.StartsWith("Vehicle.") && resolvedText.EndsWith("_player"))
                {
                    list.Add(resolvedText);
                }
            }

            list.Sort();
            File.WriteAllText("Vehicles.json", JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static void GenerateItemClasses(TweakDB tweakDb)
        {
            var dict = new Dictionary<string, string>();

            foreach (var (id, type) in tweakDb.Records)
            {
                var idStr = $"{id & 0xFFFFFFFF:X8}:{(id >> 32 & 0xFF):X2}";

                if (type == typeof(gamedataItem_Record))
                {
                    dict.Add(idStr, "Item");
                    continue;
                }

                if (type == typeof(gamedataClothing_Record))
                {
                    dict.Add(idStr, "Clothing");
                    continue;
                }

                if (type == typeof(gamedataWeaponItem_Record))
                {
                    dict.Add(idStr, "WeaponItem");
                    continue;
                }

                if (type == typeof(gamedataItemRecipe_Record))
                {
                    dict.Add(idStr, "ItemRecipe");
                    continue;
                }

                if (type == typeof(gamedataConsumableItem_Record))
                {
                    dict.Add(idStr, "ConsumableItem");
                    continue;
                }

                if (type == typeof(gamedataGrenade_Record))
                {
                    dict.Add(idStr, "Grenade");
                    continue;
                }

                if (type == typeof(gamedataGadget_Record))
                {
                    dict.Add(idStr, "Gadget");
                    continue;
                }
            }

            dict = dict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            File.WriteAllText("ItemClasses.json", JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
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