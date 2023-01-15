using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EquipmentEx.OutfitPart")]
public class OutfitPart : gameScriptableSystem
{
    [RED("itemID")]
    public gameItemID ItemID
    {
        get => GetPropertyValue<gameItemID>();
        set => SetPropertyValue<gameItemID>(value);
    }

    [RED("slotID")]
    public TweakDBID SlotID
    {
        get => GetPropertyValue<TweakDBID>();
        set => SetPropertyValue<TweakDBID>(value);
    }
}