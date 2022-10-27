using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

public static class ModManager
{
    private static bool _isLoaded;

    public static void LoadTypes()
    {
        if (_isLoaded)
        {
            return;
        }

        // CustomMapMarkers
        RedReflection.AddRedType(typeof(CustomMappinData));
        RedReflection.AddRedType(typeof(CustomMarkerSystem));

        // EnhancedCraft
        RedReflection.AddRedType(typeof(CustomCraftNameDataPS));
        RedReflection.AddRedType(typeof(DamageTypeStatsPS));
        RedReflection.AddRedType(typeof(EnhancedCraftSystem));

        // ExtraWardrobeSlots
        RedReflection.AddEnumType(typeof(gameWardrobeClothingSetIndexExtra));
        RedReflection.AddRedType(typeof(ClothingSetExtra));
        RedReflection.AddRedType(typeof(WardrobeSystemExtra));

        // Edgerunning
        RedReflection.AddRedType(typeof(EdgerunningSystem));

        // MarkToSell
        RedReflection.AddRedType(typeof(MarkToSellSystem));

        _isLoaded = true;
    }
}