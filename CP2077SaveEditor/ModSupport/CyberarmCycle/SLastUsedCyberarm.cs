using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

public class SLastUsedCyberarm : IScriptable
{
    [RED("meleeware")]
    public gameItemID MeleeWare
    {
        get => GetPropertyValue<gameItemID>();
        set => SetPropertyValue<gameItemID>(value);
    }

    [RED("launcher")]
    public gameItemID Launcher
    {
        get => GetPropertyValue<gameItemID>();
        set => SetPropertyValue<gameItemID>(value);
    }
}