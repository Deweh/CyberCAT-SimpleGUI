using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

public class WardrobeSystemExtra : gameScriptableSystem
{
    [RED("activeSetIndex")]
    public CEnum<gameWardrobeClothingSetIndexExtra> ActiveSetIndex
    {
        get => GetPropertyValue<CEnum<gameWardrobeClothingSetIndexExtra>>();
        set => SetPropertyValue<CEnum<gameWardrobeClothingSetIndexExtra>>(value);
    }

    [RED("clothingSets")]
    public CArray<CHandle<ClothingSetExtra>> ClothingSets
    {
        get => GetPropertyValue<CArray<CHandle<ClothingSetExtra>>>();
        set => SetPropertyValue<CArray<CHandle<ClothingSetExtra>>>(value);
    }

    [RED("blacklist")]
    public CArray<gameItemID> Blacklist
    {
        get => GetPropertyValue<CArray<gameItemID>>();
        set => SetPropertyValue<CArray<gameItemID>>(value);
    }

    public WardrobeSystemExtra()
    {
        ActiveSetIndex = gameWardrobeClothingSetIndexExtra.INVALID;
    }
}