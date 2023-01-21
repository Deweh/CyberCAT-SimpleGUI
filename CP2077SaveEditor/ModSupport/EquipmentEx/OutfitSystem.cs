using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EquipmentEx.OutfitSystem")]
public class OutfitSystem : gameScriptableSystem
{
    [RED("state")]
    public CHandle<OutfitState> State
    {
        get => GetPropertyValue<CHandle<OutfitState>>();
        set => SetPropertyValue<CHandle<OutfitState>>(value);
    }
}