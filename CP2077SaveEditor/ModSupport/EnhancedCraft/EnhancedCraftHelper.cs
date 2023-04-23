using System.IO;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.TweakDB;

namespace CP2077SaveEditor.ModSupport;

public static class EnhancedCraftHelper
{
    public static bool IsInstalled(SaveFileHelper save)
    {
        return save.GetScriptableSystem<EnhancedCraftSystem>() != null;
    }

    public static void ParseTweaks()
    {
        using var fh = File.OpenRead(@"C:\Games\GOG Galaxy\Cyberpunk 2077\r6\cache\tweakdb.bin");
        using var reader = new TweakDBReader(fh);
        
        if (reader.ReadFile(out var tweakDb) != WolvenKit.RED4.TweakDB.EFileReadErrorCodes.NoError)
        {
            return;
        }

        LoadWeaponVariants(@"C:\Games\GOG Galaxy\Cyberpunk 2077\r6\tweaks\EnhancedCraft\WeaponVariants.yaml", tweakDb);
        LoadClothesVariants(@"C:\Games\GOG Galaxy\Cyberpunk 2077\r6\tweaks\EnhancedCraft\ClothesVariants.yaml", tweakDb);
    }

    private static void LoadWeaponVariants(string path, TweakDB tweakDb)
    {
        /*var visualTagList = new Dictionary<string, string>();
        foreach (var (id, value) in tweakDb.Flats)
        {
            if (id.ResolvedText.EndsWith(".visualTags") && value is CArray<CName> { Count: > 0 } visualTags)
            {
                var stringName = id.ResolvedText.Substring(0, id.ResolvedText.Length - ".visualTags".Length);
                visualTagList.Add(stringName, visualTags[0]);
            }
        }

        using var sr = File.OpenText(path);

        var deserializer = new DeserializerBuilder()
            .Build();
        var tmp = deserializer.Deserialize<Dictionary<string, object>>(sr);

        foreach (var kvp in tmp)
        {
            if (kvp.Key.EndsWith(".weaponVariants"))
            {
                var stringName = kvp.Key.Substring(0, kvp.Key.Length - ".weaponVariants".Length);

                foreach (var o in (IList<object>)kvp.Value)
                {
                    var variantName = ((string)o).Substring(2, ((string)o).Length - 3);
                    if (visualTagList.TryGetValue(variantName, out var visualTag))
                    {
                        var newRecordId = stringName + "_" + visualTag;
                        Form2.TweakDbStringHelper.AddRecordHash(newRecordId);
                    }
                }
            }
        }*/
    }

    private static void LoadClothesVariants(string path, TweakDB tweakDb)
    {
        /*var appearanceNames = new Dictionary<string, string>();
        foreach (var (id, value) in tweakDb.Flats)
        {
            if (id.ResolvedText.EndsWith(".appearanceName") && value is CName cName)
            {
                var stringName = id.ResolvedText.Substring(0, id.ResolvedText.Length - ".appearanceName".Length);
                appearanceNames.Add(stringName, cName);
            }
        }

        using var sr = File.OpenText(path);

        var deserializer = new DeserializerBuilder()
            .Build();
        var tmp = deserializer.Deserialize<Dictionary<string, object>>(sr);

        foreach (var kvp in tmp)
        {
            if (kvp.Key.EndsWith(".clothesVariants"))
            {
                var stringName = kvp.Key.Substring(0, kvp.Key.Length - ".clothesVariants".Length);

                foreach (var o in (IList<object>)kvp.Value)
                {
                    var variantName = ((string)o).Substring(2, ((string)o).Length - 3);
                    if (appearanceNames.TryGetValue(variantName, out var appearanceName))
                    {
                        if (appearanceName.EndsWith('_'))
                        {
                            appearanceName = appearanceName.Substring(0, appearanceName.Length - 1);
                        }

                        var newRecordId = stringName + "_" + appearanceName;
                        Form2.TweakDbStringHelper.AddRecordHash(newRecordId);
                    }
                }
            }
        }*/
    }

    public static string GetName(SaveFileHelper save, InventoryHelper.ItemData itemData)
    {
        var nameRecords = save.GetScriptableSystem<EnhancedCraftSystem>().NameRecords;
        if (nameRecords == null)
        {
            return null;
        }

        var itemId = InventoryHelper.GetItemIdHash(itemData.Header.ItemId.Id, itemData.Header.ItemId.RngSeed);
        foreach (var nameRecord in nameRecords)
        {
            if (nameRecord.Chunk.Id == itemId)
            {
                return nameRecord.Chunk.Name;
            }
        }
        return null;
    }
}