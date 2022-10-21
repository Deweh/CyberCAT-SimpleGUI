using WolvenKit.RED4.Save;

namespace CP2077SaveEditor.ModSupport;

public static class EnhancedCraftHelper
{
    public static bool IsInstalled(SaveFileHelper save)
    {
        return save.GetScriptableSystem<EnhancedCraftSystem>() != null;
    }

    public static string GetName(SaveFileHelper save, InventoryHelper.ItemData itemData)
    {
        var itemId = InventoryHelper.GetItemIdHash(itemData.Header.ItemId.Id, itemData.Header.ItemId.RngSeed);
        foreach (var nameRecord in save.GetScriptableSystem<EnhancedCraftSystem>().NameRecords)
        {
            if (nameRecord.Chunk.Id == itemId)
            {
                return nameRecord.Chunk.Name;
            }
        }
        return null;
    }
}