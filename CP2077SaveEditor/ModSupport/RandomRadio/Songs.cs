using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

public class Songs : gameScriptableSystem
{
    [RED("_songs")]
    public CArray<SongStorage> _Songs
    {
        get => GetPropertyValue<CArray<SongStorage>>();
        set => SetPropertyValue<CArray<SongStorage>>(value);
    }

    [RED("lastRequestedSongIndex")]
    public CInt32 LastRequestedSongIndex
    {
        get => GetPropertyValue<CInt32>();
        set => SetPropertyValue<CInt32>(value);
    }

    [RED("newsWasPlaying")]
    public CBool NewsWasPlaying
    {
        get => GetPropertyValue<CBool>();
        set => SetPropertyValue<CBool>(value);
    }
}