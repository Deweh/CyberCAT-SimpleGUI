using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("MarkToSell.System.MarkToSellSystem")]
public class MarkToSellSystem : gameScriptableSystem
{
    [RED("markers")]
    public CArray<CUInt64> Markers
    {
        get => GetPropertyValue<CArray<CUInt64>>();
        set => SetPropertyValue<CArray<CUInt64>>(value);
    }
}