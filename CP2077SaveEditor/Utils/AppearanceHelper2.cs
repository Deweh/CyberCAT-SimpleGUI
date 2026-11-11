#nullable enable
using System;
using System.Collections.Generic;
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
            // The prompt is only suppressed for this one change
            var suppressPrompt = SuppressBodyGenderPrompt;
            SuppressBodyGenderPrompt = false;

            if (value == BodyGender)
            {
                return;
            }

            // Everything is checked before the save is changed, so a failed change leaves it as it was
            var playerPuppet = (PlayerPuppetPS?)_saveFileHelper.GetPSDataContainer()?.Entries.FirstOrDefault(x => x.Data is PlayerPuppetPS)?.Data;
            if (playerPuppet == null)
            {
                MessageBox.Show("Player data not found. Body Gender cannot be changed.", "Notice");
                return;
            }

            // Default presets are taken from patch 2.5 saves, older presets lack groups the game needs
            var defaultPreset = value == AppearanceGender.Female ? Properties.Resources.FemaleDefaultPreset : Properties.Resources.MaleDefaultPreset;
            var newPreset = RedJsonSerializer.Deserialize<PresetDto>(defaultPreset, new RedJsonSerializerOptions { JsonVersion = WolvenKit.Common.Constants.RedJsonVersion })?.Data;
            if (newPreset?.Preset == null)
            {
                MessageBox.Show("The default appearance could not be loaded. Body Gender cannot be changed.", "Error");
                return;
            }

            if (!suppressPrompt && MessageBox.Show("Changing body gender will reset all appearance options to default. Do you wish to continue?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            newPreset.IsBrainGenderMale = PresetWrapper.IsBrainGenderMale;

            playerPuppet.Gender = value == AppearanceGender.Female ? "Female" : "Male";
            _saveFileHelper.SetAppearanceContainer(newPreset);
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
        get => GetValue();
        set => SetValue(value);
    }

    [Browsable(false)]
    public string HairColor
    {
        get => GetValue();
        set => SetValue(value);
    }

    [Browsable(false)]
    public string EyeColor
    {
        get => GetValue();
        set => SetValue(value);
    }

    private string GetValue([CallerMemberName] string? callerMemberName = null)
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