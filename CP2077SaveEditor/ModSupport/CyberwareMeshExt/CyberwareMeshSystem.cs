using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.ModSupport.CyberwareMeshExt;

[RED("CyberwareMeshExt.CyberwareMeshSystem")]
public class CyberwareMeshSystem : gameScriptableSystem
{
    [RED("isInitialized")]
    public CBool IsInitialized
    {
        get => GetPropertyValue<CBool>();
        set => SetPropertyValue<CBool>(value);
    }

    [RED("meshToggles")]
    public CArray<MeshToggle> MeshToggles
    {
        get => GetPropertyValue<CArray<MeshToggle>>();
        set => SetPropertyValue<CArray<MeshToggle>>(value);
    }

    [RED("editedItems")]
    public CArray<TweakDBID> EditedItems
    {
        get => GetPropertyValue<CArray<TweakDBID>>();
        set => SetPropertyValue<CArray<TweakDBID>>(value);
    }

    [RED("fppHiddenItems")]
    public CArray<gameItemID> FppHiddenItems
    {
        get => GetPropertyValue<CArray<gameItemID>>();
        set => SetPropertyValue<CArray<gameItemID>>(value);
    }
}