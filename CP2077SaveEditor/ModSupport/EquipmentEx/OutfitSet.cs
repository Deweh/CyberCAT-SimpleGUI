using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EquipmentEx.OutfitSet")]
public class OutfitSet : gameScriptableSystem
{
    [RED("name")]
    public CName Name
    {
        get => GetPropertyValue<CName>();
        set => SetPropertyValue<CName>(value);
    }

    [RED("parts")]
    public CArray<CHandle<OutfitPart>> Parts
    {
        get => GetPropertyValue<CArray<CHandle<OutfitPart>>>();
        set => SetPropertyValue<CArray<CHandle<OutfitPart>>>(value);
    }

    [RED("timestamp")]
    public CFloat Timestamp
    {
        get => GetPropertyValue<CFloat>();
        set => SetPropertyValue<CFloat>(value);
    }
}