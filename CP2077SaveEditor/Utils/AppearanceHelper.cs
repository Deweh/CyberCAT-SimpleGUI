using CyberCAT.Core.Classes.NodeRepresentations;
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

        public AppearanceGender BodyGender
        {
            get
            {
                return (AppearanceGender)activeSave.GetAppearanceContainer().UnknownFirstBytes[4];
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
                    throw new Exception();
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
                    throw new Exception();
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

        public object FacialScars { get; set; }

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

        public object PiercingColor { get; set; }

        public object Teeth { get; set; }

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
                    throw new Exception();
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
                    throw new Exception();
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

        public object CheekMakeup { get; set; }

        public object CheekMakeupColor { get; set; }

        public object Blemishes { get; set; }

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

        public object NailColor { get; set; }

        public int Chest
        {
            get
            {
                if (BodyGender == AppearanceGender.Male)
                {
                    throw new Exception();
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

        public object Nipples { get; set; }

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

                SetNullableHashEntry("body_tattoo_", new HashValueEntry()
                {
                    FirstString = (BodyGender == AppearanceGender.Female ? "w" : "m") + "__" + AppearanceValueLists.SkinTones[SkinTone - 1],
                    Hash = AppearanceValueLists.BodyTattoos["TPP"][value],
                    SecondString = "body_tattoo_01"
                },
                new[] { "TPP_Body", "character_creation" }, AppearanceField.Hash, MainSections[2]);

                SetNullableHashEntry("fpp_body_tattoo_", new HashValueEntry()
                {
                    FirstString = (BodyGender == AppearanceGender.Female ? "w" : "m") + "__" + AppearanceValueLists.SkinTones[SkinTone - 1],
                    Hash = AppearanceValueLists.BodyTattoos["FPP"][value],
                    SecondString = "fpp_body_tattoo_01"
                },
                new[] { "FPP_Body" }, AppearanceField.Hash, MainSections[2]);
            }
        }

        public object BodyScars { get; set; }

        public object Genitals { get; set; }

        public object PenisSize { get; set; }

        public object PubicHairStyle { get; set; }

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
    }
}
