using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

public class ClothingSetExtra : IScriptable
{
    [RED("setID")]
    public CEnum<gameWardrobeClothingSetIndexExtra> SetID
    {
        get => GetPropertyValue<CEnum<gameWardrobeClothingSetIndexExtra>>();
        set => SetPropertyValue<CEnum<gameWardrobeClothingSetIndexExtra>>(value);
    }

    [RED("clothingList")]
    public CArray<gameSSlotVisualInfo> ClothingList
    {
        get => GetPropertyValue<CArray<gameSSlotVisualInfo>>();
        set => SetPropertyValue<CArray<gameSSlotVisualInfo>>(value);
    }

    [RED("iconID")]
    public TweakDBID IconID
    {
        get => GetPropertyValue<TweakDBID>();
        set => SetPropertyValue<TweakDBID>(value);
    }
}