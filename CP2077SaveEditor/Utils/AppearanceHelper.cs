using CyberCAT.Core.Classes.NodeRepresentations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CP2077SaveEditor.SaveFileHelper;

namespace CP2077SaveEditor
{
    public class AppearanceHelper
    {
        private SaveFileHelper activeSave;

        public AppearanceHelper(SaveFileHelper _saveFile)
        {
            activeSave = _saveFile;
        }

        public string HairStyle {
            get {
                return "";
            }
            set
            {

            }
        }

        public object GetEntry(CharacterCustomizationAppearances.Section appearanceSection, AppearanceEntryType entryType, string searchString)
        {
            foreach (CharacterCustomizationAppearances.AppearanceSection subSection in appearanceSection.AppearanceSections)
            {
                if (entryType == AppearanceEntryType.MainListEntry)
                {
                    foreach (CharacterCustomizationAppearances.HashValueEntry mainListEntry in subSection.MainList)
                    {
                        if (CompareMainListAppearanceEntries(mainListEntry.SecondString, searchString) == true)
                        {
                            return mainListEntry;
                        }
                    }
                }
                else
                {
                    foreach (CharacterCustomizationAppearances.ValueEntry additionalListEntry in subSection.AdditionalList)
                    {
                        if (additionalListEntry.FirstString == searchString)
                        {
                            return additionalListEntry;
                        }
                    }
                }
            }
            return null;
        }

        public object GetEntry(string searchString)
        {
            var searchValues = searchString.Split('.');

            if (searchValues.Count() != 3)
            {
                return "default";
            }

            var searchSection = activeSave.GetAppearanceContainer().FirstSection;
            var searchType = AppearanceEntryType.MainListEntry;
            var searchTrueString = searchValues[2];

            if (searchValues[0] == "second")
            {
                searchSection = activeSave.GetAppearanceContainer().SecondSection;
            }
            else if (searchValues[0] == "third")
            {
                searchSection = activeSave.GetAppearanceContainer().ThirdSection;
            }

            if (searchValues[1] == "additional")
            {
                searchType = AppearanceEntryType.AdditionalListEntry;
            }

            return GetEntry(searchSection, searchType, searchTrueString);
        }

        public void SetValue(AppearanceField field, string searchString, object value)
        {
            var entry = GetEntry(searchString);
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

        public void SetConcatedValue(string searchString, string newValue, int position = -1)
        {
            string currentValue;

            if (position < 0)
            {
                currentValue = activeSave.GetAppearanceValue(searchString).Split("__", StringSplitOptions.None).Last();
            }
            else
            {
                currentValue = activeSave.GetAppearanceValue(searchString).Split("__", StringSplitOptions.None)[position];
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

        public void SetHairStyle(string friendlyName)
        {
            if (friendlyName != "Shaved")
            {
                SetValue(AppearanceField.Hash, "first.main.hair_color", AppearanceValueLists.HairStyles[friendlyName]);
            }
        }

        public void SetHairColor(string colorString)
        {
            if (colorString != "None")
            {
                SetValue(AppearanceField.FirstString, "first.main.hair_color", colorString);
                if (activeSave.GetAppearanceContainer().Strings.Count < 1)
                {
                    activeSave.GetAppearanceContainer().Strings.Add(colorString.Substring(3));
                    activeSave.GetAppearanceContainer().Strings.Add("Short");
                }
                else
                {
                    activeSave.GetAppearanceContainer().Strings[0] = colorString.Substring(3);
                }
            }
        }

        public void CreateHairEntry(string friendlyName)
        {
            var hairsList = activeSave.GetAppearanceContainer().FirstSection.AppearanceSections.Where(x => x.SectionName == "hairs").FirstOrDefault().MainList;

            var newEntry = new CharacterCustomizationAppearances.HashValueEntry();
            newEntry.FirstString = AppearanceValueLists.HairColors[0];
            newEntry.Hash = AppearanceValueLists.HairStyles[friendlyName];
            newEntry.SecondString = "hair_color1";

            hairsList.Add(newEntry);
        }

        public void DeleteHairEntry()
        {
            var hairsList = activeSave.GetAppearanceContainer().FirstSection.AppearanceSections.Where(x => x.SectionName == "hairs").FirstOrDefault().MainList;
            hairsList.Remove(hairsList[0]);

            var hairsCreationList = activeSave.GetAppearanceContainer().FirstSection.AppearanceSections.Where(x => x.SectionName == "character_customization").FirstOrDefault().MainList;
            var creationEntry = hairsCreationList.Where(x => CompareMainListAppearanceEntries("hair_color", x.SecondString)).FirstOrDefault();

            if (creationEntry != null)
            {
                hairsCreationList.Remove(creationEntry);
            }
        }

        public void SetSkinColor(string colorString)
        {
            SetConcatedValue("third.main.first.body_color", colorString);
        }

        public void SetEyeColor(string colorString)
        {
            SetConcatedValue("first.main.first.eyes_color", colorString);
        }

        public bool CompareMainListAppearanceEntries(string entry1, string entry2)
        {
            return Regex.Replace(entry1, @"[\d-]", string.Empty) == Regex.Replace(entry2, @"[\d-]", string.Empty);
        }
    }

    public static class AppearanceValueLists
    {
        public static Dictionary<string, ulong> HairStyles { get; } = JsonConvert.DeserializeObject<Dictionary<string, ulong>>(CP2077SaveEditor.Properties.Resources.HairStyles);
        public static List<string> HairColors { get; } = JsonConvert.DeserializeObject<List<string>>(CP2077SaveEditor.Properties.Resources.HairColors);
        public static List<string> SkinColors { get; } = JsonConvert.DeserializeObject<List<string>>(CP2077SaveEditor.Properties.Resources.SkinColors);
        public static List<string> EyeColors { get; } = JsonConvert.DeserializeObject<List<string>>(CP2077SaveEditor.Properties.Resources.EyeColors);
    }
}
