using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EquipmentEx.ViewState")]
public class ViewState : gameScriptableSystem
{
    [RED("itemSource")]
    public CEnum<WardrobeItemSource> ItemSource
    {
        get => GetPropertyValue<CEnum<WardrobeItemSource>>();
        set => SetPropertyValue<CEnum<WardrobeItemSource>>(value);
    }

    [RED("collapsedSlots")]
    public CArray<TweakDBID> CollapsedSlots
    {
        get => GetPropertyValue<CArray<TweakDBID>>();
        set => SetPropertyValue<CArray<TweakDBID>>(value);
    }
}