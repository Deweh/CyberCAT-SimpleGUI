using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("CustomMarkers.System.CustomMappinData")]
public class CustomMappinData : RedBaseClass
{
    [RED("position")]
    public Vector4 Position
    {
        get => GetPropertyValue<Vector4>();
        set => SetPropertyValue<Vector4>(value);
    }

    [RED("description")]
    public CName Description
    {
        get => GetPropertyValue<CName>();
        set => SetPropertyValue<CName>(value);
    }

    [RED("type")]
    public CName Type
    {
        get => GetPropertyValue<CName>();
        set => SetPropertyValue<CName>(value);
    }
}