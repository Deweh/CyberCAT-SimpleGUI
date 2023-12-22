using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

public class SongWrapper : IScriptable
{
    [RED("mode")]
    public CEnum<RRPlayListMode> Mode
    {
        get => GetPropertyValue<CEnum<RRPlayListMode>>();
        set => SetPropertyValue<CEnum<RRPlayListMode>>(value);
    }

    [RED("beenPlayed")]
    public CBool BeenPlayed
    {
        get => GetPropertyValue<CBool>();
        set => SetPropertyValue<CBool>(value);
    }
}