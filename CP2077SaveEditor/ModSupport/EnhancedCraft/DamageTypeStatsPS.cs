using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EnhancedCraft.Common.DamageTypeStatsPS")]
public class DamageTypeStatsPS : RedBaseClass
{
    [RED("id")]
    public CUInt64 Id
    {
        get => GetPropertyValue<CUInt64>();
        set => SetPropertyValue<CUInt64>(value);
    }

    [RED("type")]
    public CInt32 Type
    {
        get => GetPropertyValue<CInt32>();
        set => SetPropertyValue<CInt32>(value);
    }

    [RED("damage")]
    public CFloat Damage
    {
        get => GetPropertyValue<CFloat>();
        set => SetPropertyValue<CFloat>(value);
    }

    [RED("minDamage")]
    public CFloat MinDamage
    {
        get => GetPropertyValue<CFloat>();
        set => SetPropertyValue<CFloat>(value);
    }

    [RED("maxDamage")]
    public CFloat MaxDamage
    {
        get => GetPropertyValue<CFloat>();
        set => SetPropertyValue<CFloat>(value);
    }

    [RED("percentDamage")]
    public CFloat PercentDamage
    {
        get => GetPropertyValue<CFloat>();
        set => SetPropertyValue<CFloat>(value);
    }
}