using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CP2077SaveEditor.Extensions;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.CR2W.JSON;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Types;
using static CP2077SaveEditor.AppearanceHelper;

namespace CP2077SaveEditor
{
    public class AppearanceHelper
    {
        public enum CustomizationGroupType
        {
            Customization,
            Morphs
        }

        public enum CustomizationAppearanceField
        {
            Resource,
            Definition,
            Name
        }

        public enum CustomizationMorphField
        {
            RegionName,
            TargetName
        }

        public enum CustomizationField
        {
            FirstCName,
            Resource,
            SecondCName
        }

        private SaveFileHelper activeSave;

        public AppearanceHelper(SaveFileHelper _saveFile)
        {
            activeSave = _saveFile;
        }

        public gameuiCharacterCustomizationPresetWrapper PresetWrapper => activeSave.GetAppearanceContainer();

        public bool SuppressBodyGenderPrompt { get; set; } = false;

        public AppearanceGender BodyGender
        {
            get
            {
                return PresetWrapper.Preset.IsMale ? AppearanceGender.Male : AppearanceGender.Female;
            }
            set
            {
                if (!Global.PSDataEnabled)
                {
                    MessageBox.Show("PSData is disabled. Body Gender cannot be changed.", "Notice");
                    return;
                }

                if (value != BodyGender)
                {
                    if (!SuppressBodyGenderPrompt)
                    {
                        if (MessageBox.Show("Changing body gender will reset all appearance options to default. Do you wish to continue?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }

                activeSave.GetAppearanceContainer().Preset.IsMale = value == AppearanceGender.Male;

                var playerPuppet = (PlayerPuppetPS)activeSave.GetPSDataContainer().Entries.Where(x => x.Data is PlayerPuppetPS).FirstOrDefault().Data;
                if (value == AppearanceGender.Female)
                {
                    playerPuppet.Gender = "Female";
                    if (!SuppressBodyGenderPrompt)
                    {
                        SetAllValues(RedJsonSerializer.Deserialize<gameuiCharacterCustomizationPresetWrapper>(Properties.Resources.FemaleDefaultPreset));
                    }
                }
                else
                {
                    playerPuppet.Gender = "Male";
                    if (!SuppressBodyGenderPrompt)
                    {
                        SetAllValues(RedJsonSerializer.Deserialize<gameuiCharacterCustomizationPresetWrapper>(Properties.Resources.MaleDefaultPreset));
                    }
                }

                if(SuppressBodyGenderPrompt) SuppressBodyGenderPrompt = false;
            }
        }

        public AppearanceGender VoiceTone
        {
            get
            {
                return PresetWrapper.IsBrainGenderMale ? AppearanceGender.Male : AppearanceGender.Female;
            }
            set
            {
                PresetWrapper.IsBrainGenderMale = value == AppearanceGender.Male;
            }
        }

        public int SkinTone {
            get
            {
                return AppearanceValueLists.SkinTones.FindIndex(x => x == GetConcatedValue("third.main.first.body_color")) + 1;
            }
            set
            {
                if (value > AppearanceValueLists.SkinTones.Count || value < 1)
                {
                    return;
                }
                SetConcatedValue("third.main.first.body_color", AppearanceValueLists.SkinTones[value - 1], -1, true, AppearanceValueLists.SkinTones);
            }
        }

        public int SkinType
        {
            get
            {
                return AppearanceValueLists.SkinTypes.FindIndex(x => x == ulong.Parse(GetValue("first.main.hash.skin_type_"))) + 1;
            }
            set
            {
                if (value > AppearanceValueLists.SkinTypes.Count || value < 1)
                {
                    return;
                }

                SetAllEntries(CustomizationGroupType.Customization, "skin_type_", (entry) => { ((gameuiCustomizationAppearance)entry).Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.SkinTypes[value - 1], InternalEnums.EImportFlags.Soft); });
            }
        }

        public string HairStyle
        {
            get
            {
                var str = "head.customization.resource.hair_color";
                if (Cyberware > 0)
                {
                    str += "_cyberware_";
                }

                var hash = GetValue(str);
                if (hash == "default")
                {
                    return "Shaved";
                }
                else 
                {
                    return AppearanceValueLists.HairStylesDict.Where(x => x.Value == ulong.Parse(hash)).FirstOrDefault().Key;
                }
            }
            set
            {
                if (!AppearanceValueLists.HairStylesDict.ContainsKey(value))
                {
                    return;
                }

                SetCustomizationAppearance("hair_color", new gameuiCustomizationAppearance
                    {
                        Name = "hair_color1",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.HairStylesDict[value], InternalEnums.EImportFlags.Soft),
                        Definition =  AppearanceValueLists.HairColors[0]
                    },
                    new CName[] { "hairs" });
            }
        }

        public int HairColor
        {
            get
            {
                var value = GetValue("first.main.first.hair_color");
                if (value == "default")
                {
                    return -1;
                }
                else
                {
                    return AppearanceValueLists.HairColors.FindIndex(x => x == value) + 1;
                }
            }
            set
            {
                if (value > AppearanceValueLists.HairColors.Count || value < 1)
                {
                    return;
                }

                if (GetValue("first.main.first.hair_color") != "default")
                {
                    SetValue(CustomizationField.FirstCName, "first.main.hair_color", AppearanceValueLists.HairColors[value - 1]);
                    if (PresetWrapper.Preset.Tags.Tags.Count == 0)
                    {
                        PresetWrapper.Preset.Tags.Tags.Add(AppearanceValueLists.HairColors[value - 1].Substring(3));
                        PresetWrapper.Preset.Tags.Tags.Add("Short");
                    }
                    else
                    {
                        PresetWrapper.Preset.Tags.Tags[0] = AppearanceValueLists.HairColors[value - 1].Substring(3);
                    }
                }
            }
        }

        public int Eyes
        {
            get
            {
                return GetFacialValue("eyes");
            }
            set
            {
                if (value > 21 || value < 1)
                {
                    return;
                }

                SetFacialValue("eyes", 1, value);
            }
        }

        public int EyeColor
        {
            get
            {
                return AppearanceValueLists.EyeColors.FindIndex(x => x == GetConcatedValue("first.main.first.eyes_color")) + 1;
            }
            set
            {
                if (value > AppearanceValueLists.EyeColors.Count || value < 1)
                {
                    return;
                }

                SetConcatedValue("first.main.first.eyes_color", AppearanceValueLists.EyeColors[value - 1]);
            }
        }

        public int Eyebrows
        {
            get
            {
                var hash = GetValue("first.main.hash.eyebrows_color");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.Eyebrows.FindIndex(x => x == ulong.Parse(hash));
                }
            }
            set
            {
                if (value > (AppearanceValueLists.Eyebrows.Count - 1) || value < 0)
                {
                    return;
                }

                SetCustomizationAppearance("eyebrows_color", new gameuiCustomizationAppearance
                    {
                        Name = "eyebrows_color1",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.Eyebrows[value], InternalEnums.EImportFlags.Soft),
                        Definition =  "heb_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead__01_black"
                    },
                    new CName[] { "TPP", "character_customization" }, CustomizationAppearanceField.Resource, null, true);
            }
        }

        public int EyebrowColor
        {
            get
            {
                var result = GetConcatedValue("first.main.first.eyebrows_color");
                if (result == "default")
                {
                    return -1;
                }
                else
                {
                    return AppearanceValueLists.EyebrowColors.FindIndex(x => x == result) + 1;
                }
            }
            set
            {
                if (value > AppearanceValueLists.EyebrowColors.Count || value < 1)
                {
                    return;
                }

                SetConcatedValue("first.main.first.eyebrows_color", AppearanceValueLists.EyebrowColors[value - 1]);
            }
        }

        public int Nose
        {
            get
            {
                return GetFacialValue("nose");
            }
            set
            {
                if (value > 21 || value < 1)
                {
                    return;
                }

                SetFacialValue("nose", 2, value);
            }
        }

        public int Mouth
        {
            get
            {
                return GetFacialValue("mouth");
            }
            set
            {
                if (value > 21 || value < 1)
                {
                    return;
                }

                SetFacialValue("mouth", 3, value);
            }
        }

        public int Jaw
        {
            get
            {
                return GetFacialValue("jaw");
            }
            set
            {
                if (value > 21 || value < 1)
                {
                    return;
                }

                SetFacialValue("jaw", 4, value);
            }
        }

        public int Ears
        {
            get
            {
                return GetFacialValue("ear");
            }
            set
            {
                if (value > 21 || value < 1)
                {
                    return;
                }

                SetFacialValue("ear", 5, value);
            }
        }

        public int Beard
        {
            get
            {
                if(BodyGender != AppearanceGender.Female)
                {
                    var entries = GetEntries("first.main.beard_color_");
                    foreach (var entry in entries)
                    {
                        if (entry is not gameuiCustomizationAppearance singleEntry)
                        {
                            throw new Exception();
                        }

                        var name = singleEntry.Resource.DepotPath.ToString().Split('\\').Last();
                        var searchString = name.Substring(0, name.Length - ".app".Length).Substring("hb_000_pma__".Length).Split("__");

                        return AppearanceValueLists.Beards.FindIndex(x => x == searchString[0]) + 1;
                    }
                    return 0;
                }
                return -1;
            }
            set
            {
                if (value < 0 || value > AppearanceValueLists.Beards.Count)
                {
                    return;
                }

                SetCustomizationAppearance("beard_color_", new gameuiCustomizationAppearance
                    {
                        Name = "beard_color1_0",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>("base\\characters\\head\\player_base_heads\\appearances\\facial_hairs\\hb_000_pma__" + AppearanceValueLists.Beards[value - 1] + ".app", InternalEnums.EImportFlags.Soft),
                        Definition =  "01_blonde_platinum"
                    },
                    new CName[] { "TPP", "character_customization" }, CustomizationAppearanceField.Resource);
            }
        }

        public int BeardStyle
        {
            get
            {
                if (BodyGender != AppearanceGender.Female)
                {
                    var entries = GetEntries("first.main.beard_color_");
                    foreach (var entry in entries)
                    {
                        if (entry is not gameuiCustomizationAppearance singleEntry)
                        {
                            throw new Exception();
                        }

                        var name = singleEntry.Resource.DepotPath.ToString().Split('\\').Last();
                        var searchString = name.Substring(0, name.Length - ".app".Length).Substring("hb_000_pma__".Length).Split("__");

                        if (searchString.Count() > 1)
                        {
                            return AppearanceValueLists.BeardStyles[searchString[0]].FindIndex(x => x == searchString[1]) + 1;
                        }
                        else if (AppearanceValueLists.BeardStyles[searchString[0]].Count < 2)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                return -1;
            }
            set
            {
                var entries = GetEntries("first.main.beard_color_");

                var path = ((gameuiCustomizationAppearance)entries[0]).Resource.DepotPath.ToString().Split('\\');
                var parts = path.Last().Substring(0, path.Last().Length - ".app".Length).Split("__").ToList();

                var options = AppearanceValueLists.BeardStyles[parts[1]];

                if (value < 1 || value > options.Count)
                {
                    return;
                }

                if (options[value - 1] == string.Empty && parts.Count > 2)
                {
                    parts.RemoveAt(2);
                }
                else if (options[value - 1] != string.Empty && parts.Count < 3)
                {
                    parts.Add(options[value - 1]);
                }
                else if (options[value - 1] != string.Empty && parts.Count > 2)
                {
                    parts[2] = options[value - 1];
                }

                path[path.Length - 1] = string.Join("__", parts) + ".app";
                var finalPath = string.Join('\\', path);

                foreach (var entry in entries)
                {
                    if (entry is not gameuiCustomizationAppearance singleEntry)
                    {
                        throw new Exception();
                    }

                    singleEntry.Resource = new CResourceAsyncReference<appearanceAppearanceResource>(finalPath, singleEntry.Resource.Flags);
                }
            }
        }

        public int Cyberware
        {
            get
            {
                var result = GetConcatedValue("first.main.first.cyberware_", 1);
                if (result == "default")
                {
                    return 0;
                }
                else
                {
                    return int.Parse(result.Split("_").Last());
                }
            }
            set
            {
                if (value > 8 || value < 0)
                {
                    return;
                }

                string first = null;
                if (value > 0)
                {
                    first = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__cyberware_" + value.ToString("00") + "__" + AppearanceValueLists.SkinTones[SkinTone - 1];
                }

                SetCustomizationAppearance("cyberware_", new gameuiCustomizationAppearance
                    {
                        Name = "cyberware_" + value.ToString("00"),
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(6513893019731746558, InternalEnums.EImportFlags.Soft),
                        Definition =  first
                    },
                    new CName[] { "TPP", "character_customization" }, CustomizationAppearanceField.Definition, null, true);
            }
        }

        public int FacialScars
        {
            get
            {
                var value = GetConcatedValue("first.main.first.scars");
                if (value == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.FacialScars.FindIndex(x => x == value) + 1;
                }
            }
            set
            {
                if (value > AppearanceValueLists.FacialScars.Count || value < 0)
                {
                    return;
                }

                SetCustomizationAppearance("scars", new gameuiCustomizationAppearance
                    {
                        Name = "scars",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(5491315604699331944, InternalEnums.EImportFlags.Soft),
                        Definition =  (value > 0 ? "h0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__scars_01__" + AppearanceValueLists.FacialScars[value - 1] : null)
                    },
                    new CName[] { "TPP", "character_customization" }, CustomizationAppearanceField.Definition);
            }
        }

        public int FacialTattoos
        {
            get
            {
                var hash = GetValue("first.main.hash.facial_tattoo_");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.FacialTattoos.FindIndex(x => x == ulong.Parse(hash));
                }
            }
            set
            {
                if (value > (AppearanceValueLists.FacialTattoos.Count - 1) || value < 0)
                {
                    return;
                }

                SetCustomizationAppearance("facial_tattoo_", new gameuiCustomizationAppearance
                    {
                        Name = "facial_tattoo_" + value.ToString("00"),
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.FacialTattoos[value], InternalEnums.EImportFlags.Soft),
                        Definition = (BodyGender == AppearanceGender.Female ? "w" : "m") + "__" + AppearanceValueLists.SkinTones[SkinTone - 1]
                    },
                    new CName[] { "TPP", "character_customization" }, CustomizationAppearanceField.Resource, null, false, true);

                SetCustomizationAppearance("tattoo", new gameuiCustomizationAppearance
                    {
                        Name = "tattoo",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(2355758180805363120, InternalEnums.EImportFlags.Soft),
                        Definition = (value == 0 ? null : "h0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__tattoo_" + value.ToString("00"))
                    },
                    new CName[] { "TPP", "character_customization" }, CustomizationAppearanceField.Definition);
            }
        }

        public int Piercings
        {
            get
            {
                var hash = GetValue("first.main.hash.piercings_");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    var ind = AppearanceValueLists.Piercings.FindIndex(x => x == ulong.Parse(hash));
                    return (ind < 0 ? 1 : ind);
                }
            }
            set
            {
                if (value > (AppearanceValueLists.Piercings.Count - 1) || value < 0)
                {
                    return;
                }

                SetCustomizationAppearance("piercings_", new gameuiCustomizationAppearance
                    {
                        Name = "piercings_01",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.Piercings[value]),
                        Definition = "i0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__earring__07_pearl"
                    },
                    new CName[] { "TPP", "character_customization" }, CustomizationAppearanceField.Resource);
            }
        }

        public int PiercingColor
        {
            get
            {
                var value = GetConcatedValue("first.main.first.piercings_");
                if (value == "default")
                {
                    return -1;
                }
                else
                {
                    return AppearanceValueLists.PiercingColors.FindIndex(x => x == value) + 1;
                }
            }
            set
            {
                if (value > AppearanceValueLists.PiercingColors.Count || value < 1)
                {
                    return;
                }

                SetConcatedValue("first.main.first.piercings_", AppearanceValueLists.PiercingColors[value - 1]);
            }
        }

        public int Teeth
        {
            get
            {
                var value = GetValue("first.main.first.teeth");
                if (value.EndsWith("basehead"))
                {
                    return 0;
                }
                else
                {
                    var ind = AppearanceValueLists.Teeth.FindIndex(x => x == "__" + value.Split("__", StringSplitOptions.None).Last());
                    return ind;
                }
            }
            set
            {
                if (value > (AppearanceValueLists.Teeth.Count - 1) || value < 0)
                {
                    return;
                }

                SetValue(CustomizationField.FirstCName, "first.main.first.teeth", (BodyGender == AppearanceGender.Female ? "female" : "male") + "_ht_000__basehead" + AppearanceValueLists.Teeth[value]);
            }
        }

        public int EyeMakeup
        {
            get
            {
                var hash = GetValue("first.main.hash.makeupEyes_");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.EyeMakeups.FindIndex(x => x == ulong.Parse(hash));
                }
            }
            set
            {
                if (value > (AppearanceValueLists.EyeMakeups.Count - 1) || value < 0)
                {
                    return;
                }

                SetCustomizationAppearance("makeupEyes_", new gameuiCustomizationAppearance
                    {
                        Name = "makeupEyes_01",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.EyeMakeups[value], InternalEnums.EImportFlags.Soft),
                        Definition = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead_makeup_eyes__01_black"
                    },
                    new CName[] { "TPP", "character_customization" }, CustomizationAppearanceField.Resource, null, true);

                SetAllEntries(CustomizationGroupType.Customization, "makeupEyes_", (entry) => { ((gameuiCustomizationAppearance)entry).Name = "makeupEyes_" + value.ToString("00"); });
            }
        }

        public int EyeMakeupColor
        {
            get
            {
                var result = GetConcatedValue("first.main.first.makeupEyes_");
                if (result == "default")
                {
                    return -1;
                }
                else
                {
                    return AppearanceValueLists.EyeMakeupColors.FindIndex(x => x == result) + 1;
                }
            }
            set
            {
                if (value > AppearanceValueLists.EyeMakeupColors.Count || value < 1)
                {
                    return;
                }

                SetConcatedValue("first.main.first.makeupEyes_", AppearanceValueLists.EyeMakeupColors[value - 1]);
            }
        }

        public int LipMakeup
        {
            get
            {
                var hash = GetValue("first.main.hash.makeupLips_");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.LipMakeups.FindIndex(x => x == ulong.Parse(hash));
                }
            }
            set
            {
                var max = AppearanceValueLists.LipMakeups.Count - 1;
                if (value > (BodyGender == AppearanceGender.Male ? (max - 1) : max) || value < 0)
                {
                    return;
                }

                SetCustomizationAppearance("makeupLips_", new gameuiCustomizationAppearance
                    {
                        Name = "makeupLips_01",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.LipMakeups[value], InternalEnums.EImportFlags.Soft),
                        Definition = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead__makeup_lips_01__01_black"
                    },
                    new CName[] { "TPP", "character_customization" }, CustomizationAppearanceField.Resource, null, true);
            }
        }

        public int LipMakeupColor
        {
            get
            {
                var result = GetConcatedValue("first.main.first.makeupLips_");
                if (result == "default")
                {
                    return -1;
                }
                else
                {
                    return AppearanceValueLists.LipMakeupColors.FindIndex(x => x == result) + 1;
                }
            }
            set
            {
                if (value > AppearanceValueLists.LipMakeupColors.Count || value < 1)
                {
                    return;
                }

                SetConcatedValue("first.main.first.makeupLips_", AppearanceValueLists.LipMakeupColors[value - 1]);
            }
        }

        public int CheekMakeup
        {
            get
            {
                var hash = GetValue("first.main.hash.makeupCheeks_");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.CheekMakeups.FindIndex(x => x == ulong.Parse(hash));
                }
            }
            set
            {
                if (value > (AppearanceValueLists.CheekMakeups.Count - 1) || value < 0)
                {
                    return;
                }

                if (CheekMakeupColor > -1)
                {
                    if (CheekMakeup != 5 && value == 5)
                    {
                        SetConcatedValue("first.main.first.makeupCheeks_", "02_pink");
                    }
                    else if(CheekMakeup == 5 && value < 5 && value > 0)
                    {
                        SetConcatedValue("first.main.first.makeupCheeks_", "03_light_brown");
                    }
                }

                SetCustomizationAppearance("makeupCheeks_", new gameuiCustomizationAppearance
                    {
                        Name = "makeupCheeks_01",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.CheekMakeups[value], InternalEnums.EImportFlags.Soft),
                        Definition = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__morphs_makeup_freckles_01__" + (value == 5 ? "02_pink" : "03_light_brown")
                    },
                    new CName[] { "TPP", "character_customization" });
            }
        }

        public int CheekMakeupColor
        {
            get
            {
                var value = GetConcatedValue("first.main.first.makeupCheeks_");
                if (value == "default")
                {
                    return -1;
                }
                else
                {
                    return (AppearanceValueLists.CheekMakeupColors.FindIndex(x => x == value) + 1);
                }
            }
            set
            {
                if (value > (CheekMakeup == 5 ? 7 : 4) || value < (CheekMakeup == 5 ? 5 : 1))
                {
                    return;
                }

                SetConcatedValue("first.main.first.makeupCheeks_", AppearanceValueLists.CheekMakeupColors[value - 1]);
            }
        }

        public int Blemishes
        {
            get
            {
                var hash = GetValue("first.main.hash.makeupPimples_");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.Blemishes.FindIndex(x => x == ulong.Parse(hash));
                }
            }
            set
            {
                if (value < 0 || value > (AppearanceValueLists.Blemishes.Count - 1))
                {
                    return;
                }

                SetCustomizationAppearance("makeupPimples_", new gameuiCustomizationAppearance
                    {
                        Name = "makeupPimples_01",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.Blemishes[value], InternalEnums.EImportFlags.Soft),
                        Definition = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead_pimples_01__brown_01"
                    },
                    new CName[] { "TPP", "character_customization" });
            }
        }

        public string Nails
        {
            get
            {
                return (GetValue("second.additional.second.nails_l") == "default" ? "Short" : "Long");
            }
            set
            {
                if (value != "Short" && value != "Long")
                {
                    return;
                }

                var entries = GetEntries("second.additional.nails_l"); entries.AddRange(GetEntries("second.additional.nails_r"));
                if (value == "Short")
                {
                    RemoveEntries(entries);
                } else {
                    if (entries.Count < 1)
                    {
                        var sectionNames = new CName[]
                        {
                            "holstered_default",
                            "holstered_nanowire",
                            "unholstered_nanowire",
                            "character_customization",
                            "holstered_launcher",
                            "unholstered_launcher",
                            "holstered_mantis",
                            "unholstered_mantis"
                        };

                        if (BodyGender == AppearanceGender.Female)
                        {
                            sectionNames = new CName[]
                            {
                                "holstered_default_tpp",
                                "holstered_default_fpp",
                                "holstered_nanowire_tpp",
                                "holstered_nanowire_fpp",
                                "unholstered_nanowire",
                                "character_customization",
                                "holstered_launcher_tpp",
                                "holstered_launcher_fpp",
                                "unholstered_launcher",
                                "holstered_mantis_tpp",
                                "holstered_mantis_fpp",
                                "unholstered_mantis"
                            };
                        }

                        var leftRight = new[] { "l", "r" };

                        foreach (string side in leftRight)
                        {
                            CreateEntry(new gameuiCustomizationMorph()
                            {
                                RegionName = "nails_" + side,
                                TargetName = "a0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_base__nails_" + side + (BodyGender == AppearanceGender.Male ? "_001" : string.Empty)
                            }, sectionNames, PresetWrapper.Preset.ArmsGroups);
                        }
                    }
                }
            }
        }

        public int NailColor
        {
            get
            {
                var value = GetValue("second.main.first.nails_color" + (BodyGender == AppearanceGender.Female ? "_tpp" : string.Empty)).Substring("a0_000_pwa_base__nails_".Length);
                return AppearanceValueLists.NailColors.FindIndex(x => x == value) + 1;
            }
            set
            {
                if (value > AppearanceValueLists.NailColors.Count() || value < 1)
                {
                    return;
                }

                List<gameuiCensorshipInfo> entries;
                if (BodyGender == AppearanceGender.Female)
                {
                    entries = GetEntries("second.main.nails_color_tpp");
                    entries.AddRange(GetEntries("second.main.nails_color_fpp"));
                }
                else
                {
                    entries = GetEntries("second.main.nails_color");
                }

                entries.AddRange(GetEntries("second.main.u_launcher_nails_color"));
                entries.AddRange(GetEntries("second.main.u_mantise_nails_color"));

                SetAllEntries(entries, (entry) =>
                {
                    var entryValue = entry as gameuiCustomizationAppearance;

                    entryValue.Definition = "a0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_" + (entryValue.Definition.Contains("fpp") ? "fpp" : "base") + "__nails_" + AppearanceValueLists.NailColors[value - 1];
                });
            }
        }

        public int Chest
        {
            get
            {
                if (BodyGender == AppearanceGender.Male)
                {
                    return -1;
                }

                var result = GetConcatedValue("third.additional.second.breast");
                switch (result)
                {
                    case "full_breast_big":
                        return 3;
                    case "full_breast_small":
                        return 1;
                    default:
                        return 2;
                }
            }
            set
            {
                if (value < 1 || value > 3)
                {
                    return;
                }

                var entries = GetEntries("third.additional.breast");
                if (value == 2)
                {
                    RemoveEntries(entries);
                }
                else
                {
                    var valueString = (value == 1 ? "full_breast_small" : "full_breast_big");

                    if (entries.Count < 1)
                    {
                        CreateEntry(new gameuiCustomizationMorph { RegionName = "breast" }, new CName[] { "breast", "character_creation" }, PresetWrapper.Preset.BodyGroups);
                        entries = GetEntries("third.additional.breast");
                    }

                    foreach (var entry in entries)
                    {
                        if (entry is not gameuiCustomizationMorph singleEntry)
                        {
                            throw new Exception();
                        }

                        singleEntry.TargetName = "t0_000_wa_base__" + valueString;
                        singleEntry.CensorFlag = Enums.CensorshipFlags.Censor_Nudity;
                        singleEntry.CensorFlagAction = Enums.gameuiCharacterCustomizationActionType.Deactivate;
                    }
                }
            }
        }

        public int Nipples
        {
            get
            {
                var value = GetConcatedValue("third.main.first.nipples_", 0);
                if (value == "default")
                {
                    return 1;
                }
                else
                {
                    var num = int.Parse(value.Split("_")[2]);
                    return (num == 0 ? 0 : (num + 1));
                }
            }
            set
            {
                if (value > (BodyGender == AppearanceGender.Female ? 3 : 1) || value < 0)
                {
                    return;
                }

                string first = null;
                if (value != 1)
                {
                    first = (BodyGender == AppearanceGender.Female ? "female" : "male") + "_i0_00" + (value == 0 ? 0 : (value - 1)).ToString() + "_base__nipple__" + AppearanceValueLists.SkinTones[SkinTone - 1];
                }

                SetCustomizationAppearance("fpp_nipples_", new gameuiCustomizationAppearance
                    {
                        Name = "fpp_nipples_01",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(8383615550749140678, InternalEnums.EImportFlags.Soft),
                        Definition = (first == null ? null : first.Replace("base", "fpp"))
                    },
                    new CName[] { "FPP_Body" }, CustomizationAppearanceField.Definition, PresetWrapper.Preset.BodyGroups);

                SetCustomizationAppearance("nipples_", new gameuiCustomizationAppearance
                    {
                        Name = "nipples_01",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(17949477145130904651, InternalEnums.EImportFlags.Soft),
                        Definition = first
                    },
                    new CName[] { "TPP_Body", "character_creation" }, CustomizationAppearanceField.Definition, PresetWrapper.Preset.BodyGroups);
            }
        }

        public int BodyTattoos
        {
            get
            {
                var hash = GetValue("third.main.hash.body_tattoo_");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.BodyTattoos["TPP"].FindIndex(x => x == ulong.Parse(hash));
                }
            }
            set
            {
                if (value > (AppearanceValueLists.BodyTattoos["TPP"].Count - 1) || value < 0)
                {
                    return;
                }

                SetCustomizationAppearance("body_tattoo_", new gameuiCustomizationAppearance
                    {
                        Name = "body_tattoo_01",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.BodyTattoos["TPP"][value], InternalEnums.EImportFlags.Soft),
                        Definition = (BodyGender == AppearanceGender.Female ? "w" : "m") + "__" + AppearanceValueLists.SkinTones[SkinTone - 1]
                    },
                    new CName[] { "TPP_Body", "character_creation" }, CustomizationAppearanceField.Resource, PresetWrapper.Preset.BodyGroups);

                SetCustomizationAppearance("fpp_body_tattoo_", new gameuiCustomizationAppearance
                    {
                        Name = "fpp_body_tattoo_01",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.BodyTattoos["FPP"][value], InternalEnums.EImportFlags.Soft),
                        Definition = (BodyGender == AppearanceGender.Female ? "w" : "m") + "__" + AppearanceValueLists.SkinTones[SkinTone - 1]
                    },
                    new CName[] { "FPP_Body" }, CustomizationAppearanceField.Resource, PresetWrapper.Preset.BodyGroups);
            }
        }

        public int BodyScars
        {
            get
            {
                var hash = GetValue("third.main.hash.body_scars_");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.BodyScars.FindIndex(x => x == ulong.Parse(hash));
                }
            }
            set
            {
                if (value > (AppearanceValueLists.BodyScars.Count - 1) || value < 0)
                {
                    return;
                }

                SetCustomizationAppearance("body_scars_", new gameuiCustomizationAppearance
                    {
                        Name = "body_scars_" + value.ToString("00"),
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.BodyScars[value], InternalEnums.EImportFlags.Soft),
                        Definition = "scars_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_001__" + AppearanceValueLists.SkinTones[SkinTone - 1]
                    },
                    new CName[] { "FPP_Body", "TPP_Body", "character_creation" }, CustomizationAppearanceField.Resource, PresetWrapper.Preset.BodyGroups, false, true);
            }
        }

        public int Genitals
        {
            get
            {
                var value = GetConcatedValue("third.main.first.genitals_", 1);
                if (value == "default")
                {
                    return 0;
                }
                else
                {
                    return (AppearanceValueLists.Genitals.FindIndex(x => x == value) + 1);
                }
            }
            set
            {
                if (value > AppearanceValueLists.Genitals.Count || value < 0)
                {
                    return;
                }

                string first = null;
                if (value > 0)
                {
                    first = "i0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_base__" + AppearanceValueLists.Genitals[value - 1] + "__" + AppearanceValueLists.SkinTones[SkinTone - 1];
                }

                if (value < 2)
                {
                    RemoveEntries(GetEntries("third.additional.penis_base"));
                }

                if (Genitals > 0)
                {
                    var entries = GetEntries("third.main." + AppearanceValueLists.Genitals[Genitals - 1] + "_hairstyle_");
                    if (value < 1)
                    {
                        RemoveEntries(entries);
                    }
                    else
                    {
                        SetAllEntries(entries, (entry) =>
                        {
                            var entryValue = entry as gameuiCustomizationAppearance;

                            var parts = entryValue.Definition.Split("__", StringSplitOptions.None);
                            parts[1] = AppearanceValueLists.Genitals[value - 1] + "_hairstyle";
                            entryValue.Definition = string.Join("__", parts);

                            entryValue.Name = AppearanceValueLists.Genitals[value - 1] + "_hairstyle_01";
                        });
                    }
                }

                SetCustomizationAppearance("genitals_", new gameuiCustomizationAppearance
                    {
                        Definition = first,
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(3178724759333055970, InternalEnums.EImportFlags.Soft),
                        Name = "genitals_" + value.ToString("00")
                    },
                    new CName[] { "character_creation", "genitals" }, CustomizationAppearanceField.Definition, PresetWrapper.Preset.BodyGroups, false, true);
            }
        }

        public int PenisSize
        {
            get
            {
                if (Genitals < 2)
                {
                    return -1;
                }

                var value = GetValue("third.additional.second.penis_base");
                if (value == "default")
                {
                    return 2;
                }
                else
                {
                    return (AppearanceValueLists.PenisSizes.FindIndex(x => x == value.Split("__", StringSplitOptions.None)[1]) + 1);
                }
            }
            set
            {
                if (value > AppearanceValueLists.PenisSizes.Count || value < 1)
                {
                    return;
                }

                var entries = GetEntries("third.additional.penis_base");
                if (value == 2)
                {
                    RemoveEntries(entries);
                }
                else
                {
                    var newValue = "i0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_base__" + AppearanceValueLists.PenisSizes[value - 1];
                    if (entries.Count < 1)
                    {
                        CreateEntry(new gameuiCustomizationMorph { RegionName = "penis_base", TargetName = newValue }, new CName[] { "genitals", "character_creation" }, PresetWrapper.Preset.BodyGroups);
                    }
                    else
                    {
                        SetValue(CustomizationField.SecondCName, "third.additional.penis_base", newValue);
                    }
                }
            }
        }

        public int PubicHairStyle
        {
            get
            {
                if (Genitals < 1)
                {
                    return -1;
                }

                var hash = GetValue("third.main.hash." + AppearanceValueLists.Genitals[Genitals - 1] + "_hairstyle_");
                if (hash == "default")
                {
                    return 0;
                }
                else
                {
                    return AppearanceValueLists.PubicHairStyles.FindIndex(x => x == ulong.Parse(hash));
                }
            }
            set
            {
                if (value > (AppearanceValueLists.PubicHairStyles.Count - 1) || value < 0)
                {
                    return;
                }

                SetCustomizationAppearance(AppearanceValueLists.Genitals[Genitals - 1] + "_hairstyle_", new gameuiCustomizationAppearance
                    {
                        Definition = "i0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_base__" + AppearanceValueLists.Genitals[Genitals - 1] + "_hairstyle__01_black",
                        Resource = new CResourceAsyncReference<appearanceAppearanceResource>(AppearanceValueLists.PubicHairStyles[value], InternalEnums.EImportFlags.Soft),
                        Name = AppearanceValueLists.Genitals[Genitals - 1] + "_hairstyle_01"
                    },
                    new CName[] { "character_creation", "genitals" }, CustomizationAppearanceField.Resource, PresetWrapper.Preset.BodyGroups);
            }
        }

        public object PubicHairColor { get; set; }

        public List<gameuiCensorshipInfo> GetEntries(CArray<gameuiCustomizationGroup> customizationGroups, CustomizationGroupType entryType, string searchString)
        {
            var foundEntries = new List<gameuiCensorshipInfo>();

            if (entryType == CustomizationGroupType.Customization)
            {
                foundEntries.AddRange(customizationGroups.SelectMany(x => x.Customization).Where(x => CompareMainListAppearanceEntries(x.Name, searchString)).ToList());
            }
            else
            {
                foundEntries.AddRange(customizationGroups.SelectMany(x => x.Morphs).Where(x => x.RegionName == searchString).ToList());
            }

            return foundEntries;
        }

        public List<gameuiCensorshipInfo> GetEntries(string searchString)
        {
            var location = StringToLocation(searchString);
            if (location != null)
            {
                return GetEntries(location.Groups, location.EntryType, location.SearchString);
            }
            else
            {
                return new List<gameuiCensorshipInfo>();
            }
        }

        public void SetAllEntries(CustomizationGroupType entryType, string searchString, Action<gameuiCensorshipInfo> entriesAction)
        {
            var entries = GetAllEntries(entryType, searchString);
            foreach (var entry in entries)
            {
                entriesAction(entry);
            }
        }

        public void SetAllEntries(List<gameuiCensorshipInfo> entries, Action<gameuiCensorshipInfo> entriesAction)
        {
            foreach (var entry in entries)
            {
                entriesAction(entry);
            }
        }

        public void SetAllEntries(List<object> entries, Action<object> entriesAction)
        {
            foreach (object entry in entries)
            {
                entriesAction(entry);
            }
        }

        public List<gameuiCensorshipInfo> GetAllEntries(CustomizationGroupType entryType, string searchString)
        {
            var foundEntries = new List<gameuiCensorshipInfo>();

            foundEntries.AddRange(GetEntries(PresetWrapper.Preset.HeadGroups, entryType, searchString));
            foundEntries.AddRange(GetEntries(PresetWrapper.Preset.ArmsGroups, entryType, searchString));
            foundEntries.AddRange(GetEntries(PresetWrapper.Preset.BodyGroups, entryType, searchString));

            return foundEntries;
        }

        public void EnumerateAllEntries(Action<gameuiCensorshipInfo> entryAction)
        {
            foreach (var customizationGroup in PresetWrapper.Preset.HeadGroups)
            {
                foreach (var customizationAppearance in customizationGroup.Customization)
                {
                    entryAction(customizationAppearance);
                }

                foreach (var customizationMorph in customizationGroup.Morphs)
                {
                    entryAction(customizationMorph);
                }
            }

            foreach (var customizationGroup in PresetWrapper.Preset.ArmsGroups)
            {
                foreach (var customizationAppearance in customizationGroup.Customization)
                {
                    entryAction(customizationAppearance);
                }

                foreach (var customizationMorph in customizationGroup.Morphs)
                {
                    entryAction(customizationMorph);
                }
            }

            foreach (var customizationGroup in PresetWrapper.Preset.BodyGroups)
            {
                foreach (var customizationAppearance in customizationGroup.Customization)
                {
                    entryAction(customizationAppearance);
                }

                foreach (var customizationMorph in customizationGroup.Morphs)
                {
                    entryAction(customizationMorph);
                }
            }
        }

        public void RemoveEntry(gameuiCensorshipInfo entry)
        {
            foreach (var customizationGroup in PresetWrapper.Preset.HeadGroups)
            {
                if (entry is gameuiCustomizationAppearance)
                {
                    if (customizationGroup.Customization.Contains((gameuiCustomizationAppearance)entry))
                    {
                        customizationGroup.Customization.Remove((gameuiCustomizationAppearance)entry);
                    }
                }
                else
                {
                    if (customizationGroup.Morphs.Contains((gameuiCustomizationMorph)entry))
                    {
                        customizationGroup.Morphs.Remove((gameuiCustomizationMorph)entry);
                    }
                }
            }

            foreach (var customizationGroup in PresetWrapper.Preset.ArmsGroups)
            {
                if (entry is gameuiCustomizationAppearance)
                {
                    if (customizationGroup.Customization.Contains((gameuiCustomizationAppearance)entry))
                    {
                        customizationGroup.Customization.Remove((gameuiCustomizationAppearance)entry);
                    }
                }
                else
                {
                    if (customizationGroup.Morphs.Contains((gameuiCustomizationMorph)entry))
                    {
                        customizationGroup.Morphs.Remove((gameuiCustomizationMorph)entry);
                    }
                }
            }

            foreach (var customizationGroup in PresetWrapper.Preset.BodyGroups)
            {
                if (entry is gameuiCustomizationAppearance)
                {
                    if (customizationGroup.Customization.Contains((gameuiCustomizationAppearance)entry))
                    {
                        customizationGroup.Customization.Remove((gameuiCustomizationAppearance)entry);
                    }
                }
                else
                {
                    if (customizationGroup.Morphs.Contains((gameuiCustomizationMorph)entry))
                    {
                        customizationGroup.Morphs.Remove((gameuiCustomizationMorph)entry);
                    }
                }
            }
        }

        public void RemoveEntries(List<gameuiCensorshipInfo> entries)
        {
            foreach (var entry in entries)
            {
                RemoveEntry(entry);
            }
        }

        public void CreateEntry(gameuiCensorshipInfo entry, CName[] groupNames, CArray<gameuiCustomizationGroup> groupArray)
        {
            foreach (var customizationGroup in groupArray.Where(x => groupNames.Contains(x.Name)))
            {
                if (entry is gameuiCustomizationAppearance)
                {
                    customizationGroup.Customization.Add((gameuiCustomizationAppearance)entry);
                }
                else
                {
                    customizationGroup.Customization.Add((gameuiCustomizationMorph)entry);
                }
            }
        }

        public void SetValue(CustomizationField field, string searchString, object value)
        {
            var entries = GetEntries(searchString);
            foreach (var entry in entries)
            {
                if (entry is gameuiCustomizationAppearance)
                {
                    var castedEntry = (gameuiCustomizationAppearance)entry;
                    switch (field)
                    {
                        case CustomizationField.FirstCName:
                            castedEntry.Definition = (string)value;
                            break;
                        case CustomizationField.Resource:
                            castedEntry.Resource = new CResourceAsyncReference<appearanceAppearanceResource>((ulong)value, InternalEnums.EImportFlags.Soft);
                            break;
                        case CustomizationField.SecondCName:
                            castedEntry.Name = (string)value;
                            break;
                    }
                }
                else
                {
                    var castedEntry = (gameuiCustomizationMorph)entries[0];
                    switch (field)
                    {
                        case CustomizationField.FirstCName:
                            castedEntry.RegionName = (string)value;
                            break;
                        case CustomizationField.SecondCName:
                            castedEntry.TargetName = (string)value;
                            break;
                    }
                }
            }
        }

        public string GetValue(CArray<gameuiCustomizationGroup> customizationGroups, CustomizationGroupType entryType, CustomizationField fieldToGet, string searchString)
        {
            var entries = GetEntries(customizationGroups, entryType, searchString);

            if (entries.Count == 0)
            {
                return "default";
            }
            if (entries[0] is gameuiCustomizationAppearance)
            {
                var castedEntry = (gameuiCustomizationAppearance)entries[0];
                switch (fieldToGet)
                {
                    case CustomizationField.FirstCName:
                        return castedEntry.Definition;
                    case CustomizationField.Resource:
                        return ((ulong)castedEntry.Resource.DepotPath).ToString();
                    case CustomizationField.SecondCName:
                        return castedEntry.Name;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fieldToGet), fieldToGet, null);
                }
            } 
            else 
            {
                var castedEntry = (gameuiCustomizationMorph)entries[0];
                switch (fieldToGet)
                {
                    case CustomizationField.FirstCName:
                        return castedEntry.RegionName;
                    case CustomizationField.SecondCName:
                        return castedEntry.TargetName;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fieldToGet), fieldToGet, null);
                }
            }
        }

        public string GetValue(string searchString)
        {
            var location = StringToLocation(searchString);
            if (location != null)
            {
                return GetValue(location.Groups, location.EntryType, location.EntryField, location.SearchString);
            }
            else
            {
                return "default";
            }
        }

        public void SetConcatedValue(string searchString, string newValue, int position = -1, bool wideSearch = false, IEnumerable<string> searchCollection = null)
        {
            if (searchCollection == null)
            {
                searchCollection = new[] { GetValue(searchString).Split("__", StringSplitOptions.None).LastOrIndex(position) };
            }

            EnumerateAllEntries((entry) =>
            {
                if (entry is gameuiCustomizationAppearance mainEntry)
                {
                    try
                    {
                        if (CompareMainListAppearanceEntries(mainEntry.Name, searchString.Split(".").Last()) != true && wideSearch == false)
                        {
                            return;
                        }

                        var valueParts = mainEntry.Definition.Split("__", StringSplitOptions.None);
                        var targetPart = valueParts.LastOrIndex(position);

                        if (searchCollection.Contains(targetPart))
                        {
                            if (position < 0)
                            {
                                valueParts[valueParts.Length - 1] = newValue;
                            }
                            else
                            {
                                valueParts[position] = newValue;
                            }

                            mainEntry.Definition = string.Join("__", valueParts);
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            });
        }

        public string GetConcatedValue(string searchString, int position = -1)
        {
            var result = GetValue(searchString).Split("__", StringSplitOptions.None);
            if (result[0] == "default")
            {
                return "default";
            }

            if (position < 0)
            {
                return result.Last();
            } else {
                return result[position];
            }
        }

        public void SetFacialValue(string fieldName, int fieldNum, int value)
        {
            var entries = GetEntries("first.additional." + fieldName);

            if (entries.Count == 0)
            {
                CreateEntry(new gameuiCustomizationMorph { RegionName = fieldName, TargetName = "h000" }, new CName[] { "TPP", "character_creation" }, PresetWrapper.Preset.HeadGroups);
                SetFacialValue(fieldName, fieldNum, value);
            }
            else
            {
                if (value == 1)
                {
                    foreach (var entry in entries)
                    {
                        RemoveEntry(entry);
                    }
                }
                else
                {
                    foreach (var entry in entries)
                    {
                        if (entry is not gameuiCustomizationMorph singleEntry)
                        {
                            throw new Exception();
                        }

                        var finalValue = value;
                        if (fieldName == "nose" && BodyGender == AppearanceGender.Female)
                        {
                            //Those are some fine spaghetti values ya got there CDPR.
                            if (finalValue > 11 && finalValue < 17)
                            {
                                finalValue++;
                            }
                            else if (finalValue == 17)
                            {
                                finalValue = 12;
                            }
                        }
                        finalValue--;
                        singleEntry.TargetName = "h" + finalValue.ToString("00") + fieldNum.ToString();
                    }
                }
            }
        }

        public int GetFacialValue(string fieldName)
        {
            var result = GetValue("first.additional.second." + fieldName);
            if (result == "default")
            {
                return 1;
            }
            else
            {
                var finalValue = int.Parse(result.Substring(1, 2)) + 1;
                if (fieldName == "nose" && BodyGender == AppearanceGender.Female)
                {
                    //Spaghetti for days.
                    if (finalValue > 12 && finalValue < 18)
                    {
                        finalValue--;
                    }
                    else if (finalValue == 12)
                    {
                        finalValue = 17;
                    }
                }
                return finalValue;
            }
        }

        public void SetCustomizationAppearance(string searchString,
            gameuiCustomizationAppearance defaultEntry, CName[] customizationGroupNames,
            CustomizationAppearanceField setValueField = CustomizationAppearanceField.Resource,
            CArray<gameuiCustomizationGroup> groupArray = null, bool createAllMainSections = false,
            bool allFields = false)
        {
            var entries = GetAllEntries(CustomizationGroupType.Customization, searchString);
            if (defaultEntry.Resource.DepotPath == CName.Empty || defaultEntry.Name == CName.Empty || defaultEntry.Definition == CName.Empty)
            {
                RemoveEntries(entries);
            }
            else
            {
                if (entries.Count == 0)
                {
                    if (createAllMainSections)
                    {
                        CreateEntry(defaultEntry, customizationGroupNames, PresetWrapper.Preset.HeadGroups);
                        CreateEntry(defaultEntry, customizationGroupNames, PresetWrapper.Preset.ArmsGroups);
                        CreateEntry(defaultEntry, customizationGroupNames, PresetWrapper.Preset.BodyGroups);
                    } 
                    else
                    {
                        groupArray ??= PresetWrapper.Preset.HeadGroups;
                        CreateEntry(defaultEntry, customizationGroupNames, groupArray);
                    }
                }
                else
                {
                    SetAllEntries(entries, (entry) => 
                    {
                        if (setValueField == CustomizationAppearanceField.Definition || allFields)
                        {
                            ((gameuiCustomizationAppearance)entry).Definition = defaultEntry.Definition;
                        }

                        if (setValueField == CustomizationAppearanceField.Name || allFields)
                        {
                            ((gameuiCustomizationAppearance)entry).Name = defaultEntry.Name;
                        }

                        if(setValueField == CustomizationAppearanceField.Resource || allFields)
                        {
                            ((gameuiCustomizationAppearance)entry).Resource = defaultEntry.Resource;
                        }
                    });
                }
            }
        }

        public void SetAllValues(gameuiCharacterCustomizationPresetWrapper newValues)
        {
            PresetWrapper.Preset.Version = newValues.Preset.Version;
            PresetWrapper.Preset.IsMale = newValues.Preset.IsMale;
            PresetWrapper.IsBrainGenderMale = newValues.IsBrainGenderMale;

            PresetWrapper.Preset.HeadGroups.Clear();
            foreach (var customizationGroup in newValues.Preset.HeadGroups)
            {
                PresetWrapper.Preset.HeadGroups.Add((gameuiCustomizationGroup)customizationGroup.DeepCopy());
            }

            PresetWrapper.Preset.ArmsGroups.Clear();
            foreach (var customizationGroup in newValues.Preset.ArmsGroups)
            {
                PresetWrapper.Preset.ArmsGroups.Add((gameuiCustomizationGroup)customizationGroup.DeepCopy());
            }

            PresetWrapper.Preset.BodyGroups.Clear();
            foreach (var customizationGroup in newValues.Preset.BodyGroups)
            {
                PresetWrapper.Preset.BodyGroups.Add((gameuiCustomizationGroup)customizationGroup.DeepCopy());
            }

            PresetWrapper.Preset.Tags.Tags.Clear();
            foreach (var tag in newValues.Preset.Tags.Tags)
            {
                PresetWrapper.Preset.Tags.Tags.Add(tag);
            }

            PresetWrapper.Preset.PerspectiveInfo.Clear();
            foreach (var perspectiveInfo in newValues.Preset.PerspectiveInfo)
            {
                PresetWrapper.Preset.PerspectiveInfo.Add((gameuiPerspectiveInfo)perspectiveInfo.DeepCopy());
            }
        }

        public CustomizationGroupLocation StringToLocation(string searchString)
        {
            var searchValues = searchString.Split('.');
            if (searchValues.Length < 3 && searchValues.Length > 4)
            {
                return null;
            }

            var result = new CustomizationGroupLocation();
            switch (searchValues[0])
            {
                case "first":
                case "head":
                    result.Groups = PresetWrapper.Preset.HeadGroups;
                    break;

                case "second":
                case "arms":
                    result.Groups = PresetWrapper.Preset.ArmsGroups;
                    break;

                case "third":
                case "body":
                    result.Groups = PresetWrapper.Preset.BodyGroups;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(searchValues[0]);
            }

            switch (searchValues[1])
            {
                case "main":
                case "customization":
                    result.EntryType = CustomizationGroupType.Customization;
                    break;

                case "additional":
                case "morphs":
                    result.EntryType = CustomizationGroupType.Morphs;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(searchValues[1]);
            }

            if (searchValues.Length == 3)
            {
                result.SearchString = searchValues[2];
                result.EntryField = CustomizationField.FirstCName;

                return result;
            }

            switch (searchValues[2])
            {
                case "first":
                    result.EntryField = CustomizationField.FirstCName;
                    break;

                case "hash":
                case "resource":
                    result.EntryField = CustomizationField.Resource;
                    break;

                case "second":
                    result.EntryField = CustomizationField.SecondCName;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(searchValues[2]);
            }

            result.SearchString = searchValues[3];

            return result;
        }

        public bool CompareMainListAppearanceEntries(string entry1, string entry2)
        {
            return Regex.Replace(entry1, @"[\d-]", string.Empty) == Regex.Replace(entry2, @"[\d-]", string.Empty);
        }

        public static CResourceReference<appearanceAppearanceResource> UlongToResource(ulong input)
        {
            return new CResourceReference<appearanceAppearanceResource>(input);
        }
    }

    public enum AppearanceGender
    {
        Female,
        Male
    }

    public class CustomizationGroupLocation
    {
        public CArray<gameuiCustomizationGroup> Groups { get; set; }
        public CustomizationGroupType EntryType { get; set; }
        public CustomizationField EntryField { get; set; }
        public string SearchString { get; set; }

        public CustomizationGroupLocation(){}

        public CustomizationGroupLocation(CArray<gameuiCustomizationGroup> groups, CustomizationGroupType type, string searchString, CustomizationField field = CustomizationField.FirstCName)
        {
            Groups = groups;
            EntryType = type;
            EntryField = field;
            SearchString = searchString;
        }
    }

    public static class AppearanceValueLists
    {
        public static Dictionary<string, ulong> HairStylesDict { get; }
        public static List<string> HairStyles { get; }
        public static List<string> HairColors { get; }
        public static List<string> SkinTones { get; }
        public static List<ulong> SkinTypes { get; }
        public static List<string> EyeColors { get; }
        public static List<ulong> Eyebrows { get; }
        public static List<ulong> LipMakeups { get; }
        public static List<string> EyebrowColors { get; }
        public static List<string> LipMakeupColors { get; }
        public static List<ulong> EyeMakeups { get; }
        public static List<string> EyeMakeupColors { get; }
        public static Dictionary<string, List<ulong>> BodyTattoos { get; }
        public static List<string> Nailss { get; }
        public static List<ulong> FacialTattoos { get; }
        public static List<ulong> Piercings { get; }
        public static List<string> Teeth { get; }
        public static List<string> FacialScars { get; }
        public static List<ulong> BodyScars { get; }
        public static List<string> PiercingColors { get; }
        public static List<ulong> CheekMakeups { get; }
        public static List<string> CheekMakeupColors { get; }
        public static List<ulong> Blemishes { get; }
        public static List<string> Genitals { get; }
        public static List<string> PenisSizes { get; }
        public static List<ulong> PubicHairStyles { get; }
        public static List<string> NailColors { get; }
        public static List<string> Beards { get; }
        public static Dictionary<string, List<string>> BeardStyles { get; }

        static AppearanceValueLists()
        {
            var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Properties.Resources.AppearanceValues);

            HairStylesDict = values["HairStyles"].Deserialize<Dictionary<string, ulong>>();
            HairStyles = HairStylesDict.Keys.ToList();
            HairColors = values["HairColors"].Deserialize<List<string>>();
            SkinTones = values["SkinTones"].Deserialize<List<string>>();
            SkinTypes = values["SkinTypes"].Deserialize<List<ulong>>();
            EyeColors = values["EyeColors"].Deserialize<List<string>>();
            Eyebrows = values["Eyebrows"].Deserialize<List<ulong>>();
            LipMakeups = values["LipMakeups"].Deserialize<List<ulong>>();
            EyebrowColors = values["EyebrowColors"].Deserialize<List<string>>();
            LipMakeupColors = values["LipMakeupColors"].Deserialize<List<string>>();
            EyeMakeups = values["EyeMakeups"].Deserialize<List<ulong>>();
            EyeMakeupColors = values["EyeMakeupColors"].Deserialize<List<string>>();
            BodyTattoos = values["BodyTattoos"].Deserialize<Dictionary<string, List<ulong>>>();
            Nailss = new List<string> { "Short", "Long" };
            FacialTattoos = values["FacialTattoos"].Deserialize<List<ulong>>();
            Piercings = values["Piercings"].Deserialize<List<ulong>>();
            Teeth = values["Teeth"].Deserialize<List<string>>();
            FacialScars = values["FacialScars"].Deserialize<List<string>>();
            BodyScars = values["BodyScars"].Deserialize<List<ulong>>();
            PiercingColors = values["PiercingColors"].Deserialize<List<string>>();
            CheekMakeups = values["CheekMakeups"].Deserialize<List<ulong>>();
            CheekMakeupColors = values["CheekMakeupColors"].Deserialize<List<string>>();
            Blemishes = values["Blemishes"].Deserialize<List<ulong>>();
            Genitals = values["Genitals"].Deserialize<List<string>>();
            PenisSizes = values["PenisSizes"].Deserialize<List<string>>();
            PubicHairStyles = values["PubicHairStyles"].Deserialize<List<ulong>>();
            NailColors = values["NailColors"].Deserialize<List<string>>();
            Beards = values["Beards"].Deserialize<List<string>>();
            BeardStyles = values["BeardStyles"].Deserialize<Dictionary<string, List<string>>>();
        }
    }
}
