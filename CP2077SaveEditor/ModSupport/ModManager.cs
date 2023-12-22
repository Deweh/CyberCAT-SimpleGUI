using CP2077SaveEditor.ModSupport.CyberwareMeshExt;
using CP2077SaveEditor.ModSupport.VirtualAtelier;
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

        // Equipment-EX
        RedReflection.AddEnumType(typeof(WardrobeItemSource));
        RedReflection.AddRedType(typeof(OutfitPart));
        RedReflection.AddRedType(typeof(OutfitSet));
        RedReflection.AddRedType(typeof(OutfitState));
        RedReflection.AddRedType(typeof(OutfitSystem));
        RedReflection.AddRedType(typeof(ViewManager));
        RedReflection.AddRedType(typeof(ViewState));

        // QuickhackLoadouts
        RedReflection.AddRedType(typeof(QuickhackLoadout));
        RedReflection.AddRedType(typeof(QuickhackLoadoutSystem));

        // CyberarmCycle
        RedReflection.AddRedType(typeof(SLastUsedCyberarm));

        // RandomRadio
        RedReflection.AddEnumType(typeof(RRPlayListMode));
        RedReflection.AddRedType(typeof(SongStorage));
        RedReflection.AddRedType(typeof(SongWrapper));
        RedReflection.AddRedType(typeof(Songs));

        // VirtualAtelier
        RedReflection.AddRedType(typeof(VirtualAtelierStoresManager));

        // VirtualCarDealer
        RedReflection.AddRedType(typeof(PurchasableVehicleSystem));

        // CyberwareMeshExt
        RedReflection.AddRedType(typeof(MeshToggle));
        RedReflection.AddRedType(typeof(CyberwareMeshSystem));

        _isLoaded = true;
    }
}