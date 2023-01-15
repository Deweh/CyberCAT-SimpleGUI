using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EquipmentEx.OutfitState")]
public class OutfitState : gameScriptableSystem
{
    [RED("disabled")]
    public CBool Disabled
    {
        get => GetPropertyValue<CBool>();
        set => SetPropertyValue<CBool>(value);
    }

    [RED("active")]
    public CBool Active
    {
        get => GetPropertyValue<CBool>();
        set => SetPropertyValue<CBool>(value);
    }

    [RED("parts")]
    public CArray<CHandle<OutfitPart>> Parts
    {
        get => GetPropertyValue<CArray<CHandle<OutfitPart>>>();
        set => SetPropertyValue<CArray<CHandle<OutfitPart>>>(value);
    }

    [RED("outfits")]
    public CArray<CHandle<OutfitSet>> Outfits
    {
        get => GetPropertyValue<CArray<CHandle<OutfitSet>>>();
        set => SetPropertyValue<CArray<CHandle<OutfitSet>>>(value);
    }
}