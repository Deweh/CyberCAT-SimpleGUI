using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EquipmentEx.ViewState")]
public class ViewState : gameScriptableSystem
{
    [RED("collapsedSlots")]
    public CArray<TweakDBID> CollapsedSlots
    {
        get => GetPropertyValue<CArray<TweakDBID>>();
        set => SetPropertyValue<CArray<TweakDBID>>(value);
    }
}