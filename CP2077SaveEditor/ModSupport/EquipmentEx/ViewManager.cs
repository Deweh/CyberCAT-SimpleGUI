using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EquipmentEx.ViewManager")]
public class ViewManager : gameScriptableSystem
{
    [RED("state")]
    public CHandle<ViewState> State
    {
        get => GetPropertyValue<CHandle<ViewState>>();
        set => SetPropertyValue<CHandle<ViewState>>(value);
    }
}