using CyberCAT.Core.Classes.NodeRepresentations;
using CyberCAT.Core.Classes.DumpedClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CP2077SaveEditor.SaveFileHelper;
using static CyberCAT.Core.Classes.NodeRepresentations.CharacterCustomizationAppearances;
using CP2077SaveEditor.Extensions;
using System.Windows.Forms;

namespace CP2077SaveEditor
{
    public class AppearanceHelper
    {
        private SaveFileHelper activeSave;

        public AppearanceHelper(SaveFileHelper _saveFile)
        {
            activeSave = _saveFile;
        }

        public Section[] MainSections { get; set; }

        public bool SuppressBodyGenderPrompt { get; set; } = false;

        public AppearanceGender BodyGender
        {
            get
            {
                return (AppearanceGender)activeSave.GetAppearanceContainer().UnknownFirstBytes[4];
            }
            set
            {
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

                activeSave.GetAppearanceContainer().UnknownFirstBytes[4] = (byte)value;

                var playerPuppet = (PlayerPuppetPS)activeSave.GetPSDataContainer().ClassList.Where(x => x is PlayerPuppetPS).FirstOrDefault();
                if (value == AppearanceGender.Female)
                {
                    playerPuppet.Gender = string.Empty;
                    if (!SuppressBodyGenderPrompt)
                    {
                        SetAllValues(JsonConvert.DeserializeObject<CharacterCustomizationAppearances>(CP2077SaveEditor.Properties.Resources.FemaleDefaultPreset));
                    }
                }
                else
                {
                    playerPuppet.Gender = "Male";
                    if (!SuppressBodyGenderPrompt)
                    {
                        SetAllValues(JsonConvert.DeserializeObject<CharacterCustomizationAppearances>(CP2077SaveEditor.Properties.Resources.MaleDefaultPreset));
                    }
                }

                if(SuppressBodyGenderPrompt) SuppressBodyGenderPrompt = false;
            }
        }

        public AppearanceGender VoiceTone
        {
            get
            {
                return (AppearanceGender)activeSave.GetAppearanceContainer().UnknownFirstBytes[5];
            }
            set
            {
                activeSave.GetAppearanceContainer().UnknownFirstBytes[5] = (byte)value;
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

                SetAllEntries(AppearanceEntryType.MainListEntry, "skin_type_", (object entry) => { ((HashValueEntry)entry).Hash = AppearanceValueLists.SkinTypes[value - 1]; });
            }
        }

        public string HairStyle
        {
            get
            {
                var hash = GetValue("first.main.hash.hair_color");
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

                SetNullableHashEntry("hair_color", new HashValueEntry()
                {
                    FirstString = AppearanceValueLists.HairColors[0],
                    Hash = AppearanceValueLists.HairStylesDict[value],
                    SecondString = "hair_color1"
                },
                new[] { "hairs" });
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
                    SetValue(AppearanceField.FirstString, "first.main.hair_color", AppearanceValueLists.HairColors[value - 1]);
                    var container = activeSave.GetAppearanceContainer();
                    if (container.Strings.Count < 1)
                    {
                        container.Strings.Add(AppearanceValueLists.HairColors[value - 1].Substring(3));
                        container.Strings.Add("Short");
                    }
                    else
                    {
                        container.Strings[0] = AppearanceValueLists.HairColors[value - 1].Substring(3);
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

                SetNullableHashEntry("eyebrows_color", new HashValueEntry()
                {
                    FirstString = "heb_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead__01_black",
                    Hash = AppearanceValueLists.Eyebrows[value],
                    SecondString = "eyebrows_color1"
                },
                new[] { "TPP", "character_customization" }, AppearanceField.Hash, null, true);
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

                SetNullableHashEntry("cyberware_", new HashValueEntry()
                {
                    FirstString = first,
                    Hash = 6513893019731746558,
                    SecondString = "cyberware_" + value.ToString("00")
                },
                new[] { "TPP", "character_customization" }, AppearanceField.FirstString, null, true);
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

                SetNullableHashEntry("scars", new HashValueEntry
                {
                    FirstString = (value > 0 ? "h0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__scars_01__" + AppearanceValueLists.FacialScars[value - 1] : null),
                    Hash = 5491315604699331944,
                    SecondString = "scars"
                },
                new[] { "TPP", "character_customization" }, AppearanceField.FirstString);
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

                SetNullableHashEntry("facial_tattoo_", new HashValueEntry
                {
                    FirstString = (BodyGender == AppearanceGender.Female ? "w" : "m") + "__" + AppearanceValueLists.SkinTones[SkinTone - 1],
                    Hash = AppearanceValueLists.FacialTattoos[value],
                    SecondString = "facial_tattoo_" + value.ToString("00")
                },
                new[] { "TPP", "character_customization" }, AppearanceField.Hash, null, false, true);

                SetNullableHashEntry("tattoo", new HashValueEntry
                {
                    FirstString = (value == 0 ? null : "h0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__tattoo_" + value.ToString("00")),
                    Hash = 2355758180805363120,
                    SecondString = "tattoo"
                },
                new[] { "TPP", "character_customization" }, AppearanceField.FirstString);
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

                SetNullableHashEntry("piercings_", new HashValueEntry
                {
                    FirstString = "i0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__earring__07_pearl",
                    Hash = AppearanceValueLists.Piercings[value],
                    SecondString = "piercings_01"
                },
                new[] { "TPP", "character_customization" }, AppearanceField.Hash);
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

                SetValue(AppearanceField.FirstString, "first.main.first.teeth", (BodyGender == AppearanceGender.Female ? "female" : "male") + "_ht_000__basehead" + AppearanceValueLists.Teeth[value]);
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

                SetNullableHashEntry("makeupEyes_", new HashValueEntry()
                {
                    FirstString = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead_makeup_eyes__01_black",
                    Hash = AppearanceValueLists.EyeMakeups[value],
                    SecondString = "makeupEyes_01"
                },
                new[] { "TPP", "character_customization" }, AppearanceField.Hash, null, true);

                SetAllEntries(AppearanceEntryType.MainListEntry, "makeupEyes_", (object entry) => { ((HashValueEntry)entry).SecondString = "makeupEyes_" + value.ToString("00"); });
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

                SetNullableHashEntry("makeupLips_", new HashValueEntry()
                {
                    FirstString = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead__makeup_lips_01__01_black",
                    Hash = AppearanceValueLists.LipMakeups[value],
                    SecondString = "makeupLips_01"
                },
                new[] { "TPP", "character_customization" }, AppearanceField.Hash, null, true);
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

                SetNullableHashEntry("makeupCheeks_", new HashValueEntry
                {
                    FirstString = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__morphs_makeup_freckles_01__" + (value == 5 ? "02_pink" : "03_light_brown"),
                    Hash = AppearanceValueLists.CheekMakeups[value],
                    SecondString = "makeupCheeks_01"
                },
                new[] { "TPP", "character_customization" });
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

                SetNullableHashEntry("makeupPimples_", new HashValueEntry
                {
                    FirstString = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead_pimples_01__brown_01",
                    Hash = AppearanceValueLists.Blemishes[value],
                    SecondString = "makeupPimples_01"
                },
                new[] { "TPP", "character_customization" });
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
                        var sectionNames = new[]
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
                            sectionNames = new[]
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
                            CreateEntry(new ValueEntry()
                            {
                                FirstString = "nails_" + side,
                                SecondString = "a0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_base__nails_" + side + (BodyGender == AppearanceGender.Male ? "_001" : string.Empty)
                            }, sectionNames, MainSections[1]);
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

                List<object> entries;
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

                SetAllEntries(entries, (object entry) =>
                {
                    var entryValue = entry as HashValueEntry;

                    entryValue.FirstString = "a0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_" + (entryValue.FirstString.Contains("fpp") ? "fpp" : "base") + "__nails_" + AppearanceValueLists.NailColors[value - 1];
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
                        CreateEntry(new ValueEntry() { FirstString = "breast", SecondString = string.Empty }, new[] { "breast", "character_creation" }, MainSections[2]);
                        entries = GetEntries("third.additional.breast");
                    }

                    foreach (ValueEntry entry in entries)
                    {
                        entry.SecondString = "t0_000_wa_base__" + valueString;
                        entry.TrailingBytes[0] = 1;
                        entry.TrailingBytes[4] = 1;
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

                SetNullableHashEntry("fpp_nipples_", new HashValueEntry
                {
                    FirstString = (first == null ? null : first.Replace("base", "fpp")),
                    Hash = 8383615550749140678,
                    SecondString = "fpp_nipples_01"
                },
                new[] { "FPP_Body" }, AppearanceField.FirstString, MainSections[2]);

                SetNullableHashEntry("nipples_", new HashValueEntry
                {
                    FirstString = first,
                    Hash = 17949477145130904651,
                    SecondString = "nipples_01"
                },
                new[] { "TPP_Body", "character_creation" }, AppearanceField.FirstString, MainSections[2]);
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

                var entries = GetEntries("third.main.body_color");

                foreach (HashValueEntry entry in entries)
                {
                    entry.SetPath("base\\characters\\appearances\\main_npc\\evelyn.app");
                    entry.FirstString = "default";

                    System.Windows.Forms.MessageBox.Show("set.");
                }

                //SetNullableHashEntry("body_tattoo_", new HashValueEntry()
                //{
                //    FirstString = (BodyGender == AppearanceGender.Female ? "w" : "m") + "__" + AppearanceValueLists.SkinTones[SkinTone - 1],
                //    Hash = AppearanceValueLists.BodyTattoos["TPP"][value],
                //    SecondString = "body_tattoo_01"
                //},
                //new[] { "TPP_Body", "character_creation" }, AppearanceField.Hash, MainSections[2]);

                //SetNullableHashEntry("fpp_body_tattoo_", new HashValueEntry()
                //{
                //    FirstString = (BodyGender == AppearanceGender.Female ? "w" : "m") + "__" + AppearanceValueLists.SkinTones[SkinTone - 1],
                //    Hash = AppearanceValueLists.BodyTattoos["FPP"][value],
                //    SecondString = "fpp_body_tattoo_01"
                //},
                //new[] { "FPP_Body" }, AppearanceField.Hash, MainSections[2]);
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

                SetNullableHashEntry("body_scars_", new HashValueEntry
                {
                    FirstString = "scars_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_001__" + AppearanceValueLists.SkinTones[SkinTone - 1],
                    Hash = AppearanceValueLists.BodyScars[value],
                    SecondString = "body_scars_" + value.ToString("00")
                },
                new[] { "FPP_Body", "TPP_Body", "character_creation" }, AppearanceField.Hash, MainSections[2], false, true);
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
                        SetAllEntries(entries, (object entry) =>
                        {
                            var entryValue = entry as HashValueEntry;

                            var parts = entryValue.FirstString.Split("__", StringSplitOptions.None);
                            parts[1] = AppearanceValueLists.Genitals[value - 1] + "_hairstyle";
                            entryValue.FirstString = string.Join("__", parts);

                            entryValue.SecondString = AppearanceValueLists.Genitals[value - 1] + "_hairstyle_01";
                        });
                    }
                }

                SetNullableHashEntry("genitals_", new HashValueEntry
                {
                    FirstString = first,
                    Hash = 3178724759333055970,
                    SecondString = "genitals_" + value.ToString("00")
                },
                new[] { "character_creation", "genitals" }, AppearanceField.FirstString, MainSections[2], false, true);

                
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
                        CreateEntry(new ValueEntry
                        {
                            FirstString = "penis_base",
                            SecondString = newValue
                        },
                        new[] { "character_creation", "genitals" }, MainSections[2]);
                    }
                    else
                    {
                        SetValue(AppearanceField.SecondString, "third.additional.penis_base", newValue);
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

                SetNullableHashEntry(AppearanceValueLists.Genitals[Genitals - 1] + "_hairstyle_", new HashValueEntry
                {
                    FirstString = "i0_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a_base__" + AppearanceValueLists.Genitals[Genitals - 1] + "_hairstyle__01_black",
                    Hash = AppearanceValueLists.PubicHairStyles[value],
                    SecondString = AppearanceValueLists.Genitals[Genitals - 1] + "_hairstyle_01"
                },
                new[] { "character_creation", "genitals" }, AppearanceField.Hash, MainSections[2]);
            }
        }

        public object PubicHairColor { get; set; }

        public void SetMainSections()
        {
            MainSections = new[] { activeSave.GetAppearanceContainer().FirstSection, activeSave.GetAppearanceContainer().SecondSection, activeSave.GetAppearanceContainer().ThirdSection };
        }

        public List<object> GetEntries(CharacterCustomizationAppearances.Section appearanceSection, AppearanceEntryType entryType, string searchString)
        {
            var foundEntries = new List<object>();
            if (entryType == AppearanceEntryType.MainListEntry)
            {
                foundEntries.AddRange(appearanceSection.AppearanceSections.SelectMany(x => x.MainList).Where(x => CompareMainListAppearanceEntries(x.SecondString, searchString)).ToList());
            }
            else
            {
                foundEntries.AddRange(appearanceSection.AppearanceSections.SelectMany(x => x.AdditionalList).Where(x => x.FirstString == searchString).ToList());
            }
            return foundEntries;
        }

        public List<object> GetEntries(string searchString)
        {
            var location = StringToLocation(searchString);
            if (location != null)
            {
                return GetEntries(location.Section, location.EntryType, location.SearchString);
            }
            else
            {
                return new List<object>();
            }
        }

        public void SetAllEntries(AppearanceEntryType entryType, string searchString, Action<object> entriesAction)
        {
            var entries = GetAllEntries(entryType, searchString);
            foreach (object entry in entries)
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

        public List<object> GetAllEntries(AppearanceEntryType entryType, string searchString)
        {
            var foundEntries = new List<object>();
            foreach (CharacterCustomizationAppearances.Section section in MainSections)
            {
                foundEntries.AddRange(GetEntries(section, entryType, searchString));
            }
            return foundEntries;
        }

        public void EnumerateAllEntries(Action<object> entryAction)
        {
            foreach (CharacterCustomizationAppearances.Section section in MainSections)
            {
                foreach (CharacterCustomizationAppearances.AppearanceSection subSection in section.AppearanceSections)
                {
                    foreach (CharacterCustomizationAppearances.HashValueEntry mainEntry in subSection.MainList)
                    {
                        entryAction(mainEntry);
                    }
                    foreach(ValueEntry additionalEntry in subSection.AdditionalList)
                    {
                        entryAction(additionalEntry);
                    }
                }
            }
        }

        public void RemoveEntry(object entry)
        {
            foreach (CharacterCustomizationAppearances.Section section in MainSections)
            {
                foreach (CharacterCustomizationAppearances.AppearanceSection subSection in section.AppearanceSections)
                {
                    if (entry is CharacterCustomizationAppearances.HashValueEntry)
                    {
                        if (subSection.MainList.Contains((CharacterCustomizationAppearances.HashValueEntry)entry))
                        {
                            subSection.MainList.Remove((CharacterCustomizationAppearances.HashValueEntry)entry);
                        }
                    } else {
                        if (subSection.AdditionalList.Contains((CharacterCustomizationAppearances.ValueEntry)entry))
                        {
                            subSection.AdditionalList.Remove((CharacterCustomizationAppearances.ValueEntry)entry);
                        }
                    }
                }
            }
        }

        public void RemoveEntries(List<object> entries)
        {
            foreach (object entry in entries)
            {
                RemoveEntry(entry);
            }
        }

        public void CreateEntry(object entry, string[] sectionNames, CharacterCustomizationAppearances.Section section)
        {
            var subSections = section.AppearanceSections.Where(x => sectionNames.Contains(x.SectionName));
            if (subSections != null)
            {
                foreach (CharacterCustomizationAppearances.AppearanceSection singleSubSection in subSections)
                {
                    if (entry is CharacterCustomizationAppearances.HashValueEntry)
                    {
                        singleSubSection.MainList.Add((CharacterCustomizationAppearances.HashValueEntry)entry);
                    }
                    else
                    {
                        singleSubSection.AdditionalList.Add((CharacterCustomizationAppearances.ValueEntry)entry);
                    }
                    
                }
            }
        }

        public void SetValue(AppearanceField field, string searchString, object value)
        {
            var entries = GetEntries(searchString);
            foreach (object entry in entries)
            {
                if (entry is CharacterCustomizationAppearances.HashValueEntry)
                {
                    switch (field)
                    {
                        case AppearanceField.FirstString:
                            ((CharacterCustomizationAppearances.HashValueEntry)entry).FirstString = (string)value;
                            break;
                        case AppearanceField.Hash:
                            ((CharacterCustomizationAppearances.HashValueEntry)entry).Hash = (ulong)value;
                            break;
                        case AppearanceField.SecondString:
                            ((CharacterCustomizationAppearances.HashValueEntry)entry).SecondString = (string)value;
                            break;
                    }
                }
                else if (entry is CharacterCustomizationAppearances.ValueEntry)
                {
                    switch (field)
                    {
                        case AppearanceField.FirstString:
                            ((CharacterCustomizationAppearances.ValueEntry)entry).FirstString = (string)value;
                            break;
                        case AppearanceField.SecondString:
                            ((CharacterCustomizationAppearances.ValueEntry)entry).SecondString = (string)value;
                            break;
                    }
                }
            }
        }

        public string GetValue(CharacterCustomizationAppearances.Section appearanceSection, AppearanceEntryType entryType, AppearanceField fieldToGet, string searchString)
        {
            var entries = GetEntries(appearanceSection, entryType, searchString);

            if (entries.Count < 1)
            {
                return "default";
            }

            if (entries[0] is CharacterCustomizationAppearances.HashValueEntry)
            {
                var castedEntry = (CharacterCustomizationAppearances.HashValueEntry)entries[0];
                if (fieldToGet == AppearanceField.FirstString)
                {
                    return castedEntry.FirstString;
                }
                else if (fieldToGet == AppearanceField.Hash)
                {
                    return castedEntry.Hash.ToString();
                }
                else
                {
                    return castedEntry.SecondString;
                }
            } else {
                var castedEntry = (CharacterCustomizationAppearances.ValueEntry)entries[0];
                if (fieldToGet == AppearanceField.FirstString)
                {
                    return castedEntry.FirstString;
                }
                else
                {
                    return castedEntry.SecondString;
                }
            }
        }

        public string GetValue(string searchString)
        {
            var location = StringToLocation(searchString);
            if (location != null)
            {
                return GetValue(location.Section, location.EntryType, location.EntryField, location.SearchString);
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

            EnumerateAllEntries((object entry) =>
            {
                if (entry is HashValueEntry mainEntry)
                {
                    try
                    {
                        if (CompareMainListAppearanceEntries(mainEntry.SecondString, searchString.Split(".").Last()) != true && wideSearch == false)
                        {
                            return;
                        }

                        var valueParts = mainEntry.FirstString.Split("__", StringSplitOptions.None);
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

                            mainEntry.FirstString = string.Join("__", valueParts);
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

            if (entries.Count < 1)
            {
                var newEntry = new CharacterCustomizationAppearances.ValueEntry();
                newEntry.FirstString = fieldName;
                newEntry.SecondString = "h000";

                CreateEntry(newEntry, new[] { "TPP", "character_customization" }, MainSections[0]);
                SetFacialValue(fieldName, fieldNum, value);
            }
            else
            {
                if (value == 1)
                {
                    foreach (CharacterCustomizationAppearances.ValueEntry entry in entries)
                    {
                        RemoveEntry(entry);
                    }
                }
                else
                {
                    foreach (CharacterCustomizationAppearances.ValueEntry entry in entries)
                    {
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
                        entry.SecondString = "h" + finalValue.ToString("00") + fieldNum.ToString();
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

        public void SetNullableHashEntry(string searchString, HashValueEntry defaultEntry, string[] sectionNames, AppearanceField setValueField = AppearanceField.Hash, Section baseMainSection = null, bool createAllMainSections = false, bool allFields = false)
        {
            var entries = GetAllEntries(AppearanceEntryType.MainListEntry, searchString);
            if (defaultEntry.Hash == 0 || defaultEntry.FirstString == null || defaultEntry.SecondString == null)
            {
                RemoveEntries(entries);
            }
            else
            {
                if (entries.Count < 1)
                {
                    if (createAllMainSections == true)
                    {
                        foreach (Section mainSection in MainSections)
                        {
                            CreateEntry(defaultEntry, sectionNames, mainSection);
                        }
                    } else {
                        if (baseMainSection == null)
                        {
                            baseMainSection = MainSections[0];
                        }
                        CreateEntry(defaultEntry, sectionNames, baseMainSection);
                    }
                }
                else
                {
                    SetAllEntries(entries, (object entry) => 
                    {
                        if (setValueField == AppearanceField.FirstString || allFields == true)
                        {
                            ((HashValueEntry)entry).FirstString = defaultEntry.FirstString;
                        }

                        if (setValueField == AppearanceField.SecondString || allFields == true)
                        {
                            ((HashValueEntry)entry).SecondString = defaultEntry.SecondString;
                        }

                        if(setValueField == AppearanceField.Hash || allFields == true)
                        {
                            ((HashValueEntry)entry).Hash = defaultEntry.Hash;
                        }
                    });
                }
            }
        }

        public void SetAllValues(CharacterCustomizationAppearances newValues)
        {
            var sections = new[] { activeSave.GetAppearanceContainer().FirstSection, activeSave.GetAppearanceContainer().SecondSection, activeSave.GetAppearanceContainer().ThirdSection };
            var newSections = new[] { newValues.FirstSection, newValues.SecondSection, newValues.ThirdSection };

            var i = 0;
            foreach (CharacterCustomizationAppearances.Section section in sections)
            {
                section.AppearanceSections.Clear();
                foreach (CharacterCustomizationAppearances.AppearanceSection subSection in newSections[i].AppearanceSections)
                {
                    section.AppearanceSections.Add(subSection);
                }
                i++;
            }

            if (newValues.Strings != null)
            {
                activeSave.GetAppearanceContainer().Strings.Clear();
                foreach (string singleString in newValues.Strings)
                {
                    activeSave.GetAppearanceContainer().Strings.Add(singleString);
                }
            }

            if (newValues.StringTriples != null)
            {
                activeSave.GetAppearanceContainer().StringTriples.Clear();
                foreach (var tripleString in newValues.StringTriples)
                {
                    activeSave.GetAppearanceContainer().StringTriples.Add(tripleString);
                }
            }

            if (newValues.UnknownFirstBytes.Length == 6)
            {
                activeSave.GetAppearanceContainer().UnknownFirstBytes = newValues.UnknownFirstBytes;
            }
        }

        public AppearanceEntryLocation StringToLocation(string searchString)
        {
            var searchValues = searchString.Split('.');
            if (searchValues.Length < 3 && searchValues.Length > 4)
            {
                return null;
            }

            var result = new AppearanceEntryLocation(activeSave.GetAppearanceContainer().FirstSection, AppearanceEntryType.MainListEntry, (searchValues.Length == 3) ? searchValues[2] : searchValues[3], AppearanceField.FirstString);
            if (searchValues[0] == "second")
            {
                result.Section = activeSave.GetAppearanceContainer().SecondSection;
            }
            else if (searchValues[0] == "third")
            {
                result.Section = activeSave.GetAppearanceContainer().ThirdSection;
            }

            if (searchValues[1] == "additional")
            {
                result.EntryType = AppearanceEntryType.AdditionalListEntry;
            }

            if (searchValues.Length == 4)
            {
                if (searchValues[2] == "hash")
                {
                    result.EntryField = AppearanceField.Hash;
                }
                else if (searchValues[2] == "second")
                {
                    result.EntryField = AppearanceField.SecondString;
                }
            }
            
            return result;
        }

        public bool CompareMainListAppearanceEntries(string entry1, string entry2)
        {
            return Regex.Replace(entry1, @"[\d-]", string.Empty) == Regex.Replace(entry2, @"[\d-]", string.Empty);
        }
    }

    public enum AppearanceGender
    {
        Female,
        Male
    }

    public class AppearanceEntryLocation
    {
        public CharacterCustomizationAppearances.Section Section { get; set; }
        public AppearanceEntryType EntryType { get; set; }
        public AppearanceField EntryField { get; set; }
        public string SearchString { get; set; }

        public AppearanceEntryLocation(CharacterCustomizationAppearances.Section _sec, AppearanceEntryType _type, string _searchString, AppearanceField _field = AppearanceField.FirstString)
        {
            Section = _sec;
            EntryType = _type;
            EntryField = _field;
            SearchString = _searchString;
        }
    }

    public static class AppearanceValueLists
    {
        private static JObject Values = JsonConvert.DeserializeObject<JObject>(CP2077SaveEditor.Properties.Resources.AppearanceValues);
        public static Dictionary<string, ulong> HairStylesDict { get; } = Values["HairStyles"].ToObject<Dictionary<string, ulong>>();
        public static List<string> HairStyles { get; } = HairStylesDict.Keys.ToList();
        public static List<string> HairColors { get; } = Values["HairColors"].ToObject<List<string>>();
        public static List<string> SkinTones { get; } = Values["SkinTones"].ToObject<List<string>>();
        public static List<ulong> SkinTypes { get; } = Values["SkinTypes"].ToObject<List<ulong>>();
        public static List<string> EyeColors { get; } = Values["EyeColors"].ToObject<List<string>>();
        public static List<ulong> Eyebrows { get; } = Values["Eyebrows"].ToObject<List<ulong>>();
        public static List<ulong> LipMakeups { get; } = Values["LipMakeups"].ToObject<List<ulong>>();
        public static List<string> EyebrowColors { get; } = Values["EyebrowColors"].ToObject<List<string>>();
        public static List<string> LipMakeupColors { get; } = Values["LipMakeupColors"].ToObject<List<string>>();
        public static List<ulong> EyeMakeups { get; } = Values["EyeMakeups"].ToObject<List<ulong>>();
        public static List<string> EyeMakeupColors { get; } = Values["EyeMakeupColors"].ToObject<List<string>>();
        public static Dictionary<string, List<ulong>> BodyTattoos { get; } = Values["BodyTattoos"].ToObject<Dictionary<string, List<ulong>>>();
        public static List<string> Nailss { get; } = new List<string> { "Short", "Long" };
        public static List<ulong> FacialTattoos { get; } = Values["FacialTattoos"].ToObject<List<ulong>>();
        public static List<ulong> Piercings { get; } = Values["Piercings"].ToObject<List<ulong>>();
        public static List<string> Teeth { get; } = Values["Teeth"].ToObject<List<string>>();
        public static List<string> FacialScars { get; } = Values["FacialScars"].ToObject<List<string>>();
        public static List<ulong> BodyScars { get; } = Values["BodyScars"].ToObject<List<ulong>>();
        public static List<string> PiercingColors { get; } = Values["PiercingColors"].ToObject<List<string>>();
        public static List<ulong> CheekMakeups { get; } = Values["CheekMakeups"].ToObject<List<ulong>>();
        public static List<string> CheekMakeupColors { get; } = Values["CheekMakeupColors"].ToObject<List<string>>();
        public static List<ulong> Blemishes { get; } = Values["Blemishes"].ToObject<List<ulong>>();
        public static List<string> Genitals { get; } = Values["Genitals"].ToObject<List<string>>();
        public static List<string> PenisSizes { get; } = Values["PenisSizes"].ToObject<List<string>>();
        public static List<ulong> PubicHairStyles { get; } = Values["PubicHairStyles"].ToObject<List<ulong>>();
        public static List<string> NailColors { get; } = Values["NailColors"].ToObject<List<string>>();
    }
}
