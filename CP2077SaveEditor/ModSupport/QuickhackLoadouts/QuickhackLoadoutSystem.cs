using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("QuickhackLoadouts.QuickhackLoadoutSystem")]
public class QuickhackLoadoutSystem : gameScriptableSystem
{
    [RED("quickhackLoadouts")]
    public CArray<CHandle<QuickhackLoadout>> QuickhackLoadouts
    {
        get => GetPropertyValue<CArray<CHandle<QuickhackLoadout>>>();
        set => SetPropertyValue<CArray<CHandle<QuickhackLoadout>>>(value);
    }
}