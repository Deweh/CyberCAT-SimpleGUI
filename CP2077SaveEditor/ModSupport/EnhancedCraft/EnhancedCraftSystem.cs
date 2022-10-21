using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("EnhancedCraft.System.EnhancedCraftSystem")]
public class EnhancedCraftSystem : gameScriptableSystem
{
    [RED("nameRecords")]
    public CArray<CHandle<CustomCraftNameDataPS>> NameRecords
    {
        get => GetPropertyValue<CArray<CHandle<CustomCraftNameDataPS>>>();
        set => SetPropertyValue<CArray<CHandle<CustomCraftNameDataPS>>>(value);
    }

    [RED("damageRecords")]
    public CArray<CHandle<DamageTypeStatsPS>> DamageRecords
    {
        get => GetPropertyValue<CArray<CHandle<DamageTypeStatsPS>>>();
        set => SetPropertyValue<CArray<CHandle<DamageTypeStatsPS>>>(value);
    }
}