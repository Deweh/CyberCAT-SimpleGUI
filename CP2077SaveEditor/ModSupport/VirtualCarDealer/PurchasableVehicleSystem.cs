using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport;

[RED("CarDealer.System.PurchasableVehicleSystem")]
public class PurchasableVehicleSystem : gameScriptableSystem
{
    [RED("soldVehicles")]
    public CArray<TweakDBID> SoldVehicles
    {
        get => GetPropertyValue<CArray<TweakDBID>>();
        set => SetPropertyValue<CArray<TweakDBID>>(value);
    }
}