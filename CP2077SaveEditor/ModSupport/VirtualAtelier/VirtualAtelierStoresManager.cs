using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport.VirtualAtelier;

[RED("VirtualAtelier.Systems.VirtualAtelierStoresManager")]
public class VirtualAtelierStoresManager : gameScriptableSystem
{
    [RED("bookmarked")]
    public CArray<CName> Bookmarked
    {
        get => GetPropertyValue<CArray<CName>>();
        set => SetPropertyValue<CArray<CName>>(value);
    }

    [RED("prevStores")]
    public CArray<CName> PrevStores
    {
        get => GetPropertyValue<CArray<CName>>();
        set => SetPropertyValue<CArray<CName>>(value);
    }
}