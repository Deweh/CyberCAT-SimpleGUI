using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("CustomMarkers.System.CustomMarkerSystem")]
public class CustomMarkerSystem : gameScriptableSystem
{
    [RED("mappins")]
    public CArray<CHandle<CustomMappinData>> Mappins
    {
        get => GetPropertyValue<CArray<CHandle<CustomMappinData>>>();
        set => SetPropertyValue<CArray<CHandle<CustomMappinData>>>(value);
    }
}