using WolvenKit.RED4.Types;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveEditor.ModSupport.CyberwareMeshExt;

[RED("CyberwareMeshExt.MeshToggle")]
public class MeshToggle : IScriptable
{
    [RED("equipArea")]
    public CEnum<gamedataEquipmentArea> EquipArea
    {
        get => GetPropertyValue<CEnum<gamedataEquipmentArea>>();
        set => SetPropertyValue<CEnum<gamedataEquipmentArea>>(value);
    }

    [RED("is_toggled")]
    public CBool IsToggled
    {
        get => GetPropertyValue<CBool>();
        set => SetPropertyValue<CBool>(value);
    }

    [RED("interactDelayID")]
    public gameDelayID InteractDelayID
    {
        get => GetPropertyValue<gameDelayID>();
        set => SetPropertyValue<gameDelayID>(value);
    }
}