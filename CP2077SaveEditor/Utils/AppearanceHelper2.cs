#nullable enable
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using WolvenKit.RED4.CR2W.JSON;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Utils;

public class AppearanceHelper2
{
    private readonly SaveFileHelper _saveFileHelper;
    private gameuiCharacterCustomizationPresetWrapper PresetWrapper => _saveFileHelper.GetAppearanceContainer();

    public AppearanceHelper2(SaveFileHelper saveFileHelper)
    {
        _saveFileHelper = saveFileHelper;
    }

    public bool SuppressBodyGenderPrompt { get; set; }

    public AppearanceGender BodyGender
    {
        get => PresetWrapper.Preset.IsMale ? AppearanceGender.Male : AppearanceGender.Female;
        set
        {
            if (value == BodyGender)
            {
                return;
            }

            if (!Global.PSDataEnabled)
            {
                MessageBox.Show("PSData is disabled. Body Gender cannot be changed.", "Notice");
                return;
            }

            if (!SuppressBodyGenderPrompt)
            {
                if (MessageBox.Show("Changing body gender will reset all appearance options to default. Do you wish to continue?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }

                SuppressBodyGenderPrompt = false;
            }

            PresetWrapper.Preset.IsMale = value == AppearanceGender.Male;

            var playerPuppet = (PlayerPuppetPS?)_saveFileHelper.GetPSDataContainer()?.Entries.FirstOrDefault(x => x.Data is PlayerPuppetPS)?.Data;
            if (playerPuppet == null)
            {
                MessageBox.Show("Player data not found. Aborting.", "Notice");
                return;
            }

            var oldTone = PresetWrapper.IsBrainGenderMale;
            if (value == AppearanceGender.Female)
            {
                playerPuppet.Gender = "Female";
                _saveFileHelper.SetAppearanceContainer(RedJsonSerializer.Deserialize<gameuiCharacterCustomizationPresetWrapper>(Properties.Resources.FemaleDefaultPreset));
            }
            else
            {
                playerPuppet.Gender = "Male";
                _saveFileHelper.SetAppearanceContainer(RedJsonSerializer.Deserialize<gameuiCharacterCustomizationPresetWrapper>(Properties.Resources.MaleDefaultPreset));
            }
            PresetWrapper.IsBrainGenderMale = oldTone;
        }
    }

    public AppearanceGender VoiceTone
    {
        get => PresetWrapper.IsBrainGenderMale ? AppearanceGender.Male : AppearanceGender.Female;
        set
        {
            if (MessageBox.Show("Changing voice tone can lead to major bugs. Do you wish to continue?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            PresetWrapper.IsBrainGenderMale = value == AppearanceGender.Male;
        }
    }

    [Browsable(false)]
    public string HairStyle
    {
        get => (string)GetValue();
        set => SetValue(value);
    }

    [Browsable(false)]
    public string HairColor
    {
        get => (string)GetValue();
        set => SetValue(value);
    }

    [Browsable(false)]
    public string EyeColor
    {
        get => (string)GetValue();
        set => SetValue(value);
    }

    private object? GetValue([CallerMemberName] string? callerMemberName = null)
    {
        if (callerMemberName == nameof(HairStyle))
        {
            var app = GetAppearance(PresetWrapper.Preset.HeadGroups, "hairs");
            if (app == null)
            {
                return "None";
            }
            return (string)app.Name!;
        }

        if (callerMemberName == nameof(HairColor))
        {
            var app = GetAppearance(PresetWrapper.Preset.HeadGroups, "hairs");
            if (app == null)
            {
                return "None";
            }
            return (string)app.Definition!;
        }

        if (callerMemberName == nameof(EyeColor))
        {
            var app = GetAppearance(PresetWrapper.Preset.HeadGroups, "TPP", "eyes_color");
            if (app == null)
            {
                return "None";
            }
            return (string)app.Definition!;
        }

        throw new Exception();
    }

    private gameuiCustomizationAppearance? GetAppearance(CArray<gameuiCustomizationGroup> group, string entryName, string? customizationName = null)
    {
        foreach (var entry in group)
        {
            if (entry.Name == entryName)
            {
                if (entry.Customization.Count == 0)
                {
                    return null;
                }

                if (customizationName == null)
                {
                    if (entry.Customization.Count > 1)
                    {
                        throw new Exception();
                    }

                    return entry.Customization[0];
                }

                foreach (var customization in entry.Customization)
                {
                    if (customization.Name == customizationName)
                    {
                        return customization;
                    }
                }
            }
        }

        throw new Exception();
    }

    private void SetValue(object value, [CallerMemberName] string? callerMemberName = null)
    {
    }
}