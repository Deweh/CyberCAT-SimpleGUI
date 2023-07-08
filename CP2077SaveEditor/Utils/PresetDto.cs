using WolvenKit.RED4.Save;

namespace CP2077SaveEditor.Utils;

public class PresetDto
{
    public string JsonVersion { get; set; } = WolvenKit.Common.Constants.RedJsonVersion;
    public gameuiCharacterCustomizationPresetWrapper? Data { get; set; }
}