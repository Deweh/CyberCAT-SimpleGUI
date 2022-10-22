using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("Edgerunning.System.EdgerunningSystem")]
public class EdgerunningSystem : gameScriptableSystem
{
    [RED("currentHumanityDamage")]
    public CInt32 CurrentHumanityDamage
    {
        get => GetPropertyValue<CInt32>();
        set => SetPropertyValue<CInt32>(value);
    }
}