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
            set
            {
                
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
                SetConcatedValue("third.main.first.body_color", AppearanceValueLists.SkinTones[value - 1]);
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

                SetNullableHashValue("hair_color", new HashValueEntry()
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
                    return 0;
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

                SetNullableHashValue("eyebrows_color", new HashValueEntry()
                {
                    FirstString = "heb_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead__01_black",
                    Hash = AppearanceValueLists.Eyebrows[value],
                    SecondString = "eyebrows_color1"
                },
                new[] { "TPP", "character_customization" }, null, true);
            }
        }

        public int EyebrowColor
        {
            get
            {
                var result = GetConcatedValue("first.main.first.eyebrows_color");
                if (result == "default")
                {
                    return 0;
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

        public object Cyberware { get; set; }

        public object FacialScars { get; set; }

        public object FacialTattoos { get; set; }

        public object Piercings { get; set; }

        public object PiercingColor { get; set; }

        public object Teeth { get; set; }

        public object EyeMakeup { get; set; }

        public object EyeMakeupColor { get; set; }

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

                SetNullableHashValue("makeupLips_", new HashValueEntry()
                {
                    FirstString = "hx_000_p" + (BodyGender == AppearanceGender.Female ? "w" : "m") + "a__basehead__makeup_lips_01__01_black",
                    Hash = AppearanceValueLists.LipMakeups[value],
                    SecondString = "makeupLips_01"
                },
                new[] { "TPP", "character_customization" }, null, true);
            }
        }

        public object LipMakeupColor { get; set; }

        public object CheekMakeup { get; set; }

        public object CheekMakeupColor { get; set; }

        public object Blemishes { get; set; }

        public object Nails { get; set; }

        public object NailColor { get; set; }

        public object Chest { get; set; }

        public object Nipples { get; set; }

        public object BodyTattoos { get; set; }

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
            foreach (CharacterCustomizationAppearances.AppearanceSection subSection in appearanceSection.AppearanceSections)
            {
                if (entryType == AppearanceEntryType.MainListEntry)
                {
                    foreach (CharacterCustomizationAppearances.HashValueEntry mainListEntry in subSection.MainList)
                    {
                        if (CompareMainListAppearanceEntries(mainListEntry.SecondString, searchString) == true)
                        {
                            foundEntries.Add(mainListEntry);
                        }
                    }
                }
                else
                {
                    foreach (CharacterCustomizationAppearances.ValueEntry additionalListEntry in subSection.AdditionalList)
                    {
                        if (additionalListEntry.FirstString == searchString)
                        {
                            foundEntries.Add(additionalListEntry);
                        }
                    }
                }
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

        public void SetConcatedValue(string searchString, string newValue, int position = -1)
        {
            string currentValue;
            if (position < 0)
            {
                currentValue = GetValue(searchString).Split("__", StringSplitOptions.None).Last();
            }
            else
            {
                currentValue = GetValue(searchString).Split("__", StringSplitOptions.None)[position];
            }

            var sections = new[] { activeSave.GetAppearanceContainer().FirstSection, activeSave.GetAppearanceContainer().SecondSection, activeSave.GetAppearanceContainer().ThirdSection };
            foreach (CharacterCustomizationAppearances.Section section in sections)
            {
                foreach (CharacterCustomizationAppearances.AppearanceSection subSection in section.AppearanceSections)
                {
                    foreach (CharacterCustomizationAppearances.HashValueEntry mainEntry in subSection.MainList)
                    {
                        try
                        {
                            var valueParts = mainEntry.FirstString.Split("__", StringSplitOptions.None);
                            var targetPart = valueParts.Last();

                            if (position > -1)
                            {
                                targetPart = valueParts[position];
                            }

                            if (targetPart == currentValue)
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
                            continue;
                        }
                    }
                }
            }
        }

        public string GetConcatedValue(string searchString, int position = -1)
        {
            var result = GetValue(searchString).Split("__", StringSplitOptions.None);
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
                            if (finalValue > 11 || finalValue < 17)
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
                    if (finalValue > 12 || finalValue < 18)
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

        public void SetNullableHashValue(string searchString, HashValueEntry defaultEntry, string[] sectionNames, Section baseMainSection = null, bool createAllMainSections = false)
        {
            var entries = GetAllEntries(AppearanceEntryType.MainListEntry, searchString);
            if (defaultEntry.Hash == 0)
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
                    SetAllEntries(entries, (object entry) => { ((HashValueEntry)entry).Hash = defaultEntry.Hash; });
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
    }
}
