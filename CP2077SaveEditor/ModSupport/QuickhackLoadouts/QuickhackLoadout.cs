using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("QuickhackLoadouts.QuickhackLoadout")]
public class QuickhackLoadout : IScriptable
{
    [RED("quickhacks")]
    public CArray<TweakDBID> Quickhacks
    {
        get => GetPropertyValue<CArray<TweakDBID>>();
        set => SetPropertyValue<CArray<TweakDBID>>(value);
    }

    [RED("name")]
    public CName Name
    {
        get => GetPropertyValue<CName>();
        set => SetPropertyValue<CName>(value);
    }

    [RED("saved")]
    public CBool Saved
    {
        get => GetPropertyValue<CBool>();
        set => SetPropertyValue<CBool>(value);
    }
}