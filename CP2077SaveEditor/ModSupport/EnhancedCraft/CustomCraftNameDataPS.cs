using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EnhancedCraft.Common.CustomCraftNameDataPS")]
public class CustomCraftNameDataPS : RedBaseClass
{
    [RED("id")]
    public CUInt64 Id
    {
        get => GetPropertyValue<CUInt64>();
        set => SetPropertyValue<CUInt64>(value);
    }

    [RED("name")]
    public CName Name
    {
        get => GetPropertyValue<CName>();
        set => SetPropertyValue<CName>(value);
    }
}