using System.Text.Json;
using WolvenKit.Common.Services;
using WolvenKit.RED4.Archive.IO;
using WolvenKit.RED4.TweakDB;
using WolvenKit.RED4.Types;
using EFileReadErrorCodes = WolvenKit.RED4.Archive.IO.EFileReadErrorCodes;

namespace ResourceGenerator
{
    internal class Program
    {
        private static string _gameDir = @"C:\Games\GOG Galaxy\Cyberpunk 2077";

        static void Main(string[] args)
        {
            GenerateItemClasses(Path.Combine(_gameDir, "r6", "cache", "tweakdb.bin"));
            GenerateAppearanceValues(Path.Combine(_gameDir, "archive", "pc", "content", "basegame_1_engine.archive"));
        }

        private static void GenerateItemClasses(string tweakPath)
        {
            using var fh = File.OpenRead(tweakPath);
            using var reader = new TweakDBReader(fh);

            if (reader.ReadFile(out var tweakDb) == WolvenKit.RED4.TweakDB.EFileReadErrorCodes.NoError)
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
        }

        private static void GenerateAppearanceValues(string archivePath)
        {
            var hashService = new HashService();

            var am = new ArchiveReader();

            if (am.ReadArchive(archivePath, hashService, out var archive) == EFileReadErrorCodes.NoError)
            {
                var maleOptions = archive.Files.FirstOrDefault(x => x.Key == 5205075720019286593).Value;
                if (maleOptions != null)
                {
                    var ms = new MemoryStream();
                    maleOptions.Extract(ms);
                    ms.Position = 0;

                    using var cr = new CR2WReader(ms);
                    if (cr.ReadFile(out var cr2w) == EFileReadErrorCodes.NoError)
                    {

                    }
                }

                var femaleOptions = archive.Files.FirstOrDefault(x => x.Key == 3179227268398507574).Value;
                if (femaleOptions != null)
                {
                    var ms = new MemoryStream();
                    femaleOptions.Extract(ms);
                    ms.Position = 0;

                    using var cr = new CR2WReader(ms);
                    if (cr.ReadFile(out var cr2w) == EFileReadErrorCodes.NoError)
                    {

                    }
                }
            }
        }
    }
}