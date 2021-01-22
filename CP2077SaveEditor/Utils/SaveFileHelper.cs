using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CyberCAT.Core.Classes;
using CyberCAT.Core.Classes.DumpedClasses;
using CyberCAT.Core.Classes.Interfaces;
using CyberCAT.Core.Classes.Mapping;
using CyberCAT.Core.Classes.NodeRepresentations;
using CyberCAT.Core.DumpedEnums;
using Newtonsoft.Json;

namespace CP2077SaveEditor
{
    class SaveFileHelper : SaveFile
    {
        public AppearanceHelper Appearance { get; }

        public enum AppearanceEntryType
        {
            MainListEntry,
            AdditionalListEntry
        }

        public enum AppearanceField
        {
            FirstString,
            Hash,
            SecondString
        }

        public SaveFileHelper(IEnumerable<INodeParser> parsers) : base(parsers) {
            Appearance = new AppearanceHelper(this);
        }

        public CharacterCustomizationAppearances GetAppearanceContainer()
        {
            return (CharacterCustomizationAppearances)this.Nodes[this.Nodes.FindIndex(x => x.Name == "CharacetrCustomization_Appearances")].Value;
        }

        public Inventory GetInventoriesContainer()
        {
            return (Inventory)this.Nodes[this.Nodes.FindIndex(x => x.Name == "inventory")].Value;
        }
        
        public NodeEntry GetFactsContainer()
        {
            var questSystem = this.Nodes[this.Nodes.FindIndex(x => x.Name == "questSystem")]; return questSystem.Children[questSystem.Children.FindIndex(x => x.Name == "FactsDB")];
        }

        public GenericUnknownStruct GetStatsContainer()
        {
            return (GenericUnknownStruct)this.Nodes[this.Nodes.FindIndex(x => x.Name == "StatsSystem")].Value;
        }

        public GenericUnknownStruct GetScriptableContainer()
        {
            return (GenericUnknownStruct)this.Nodes[this.Nodes.FindIndex(x => x.Name == "ScriptableSystemsContainer")].Value;
        }

        public Handle<PlayerDevelopmentData> GetPlayerDevelopmentData()
        {
            var devSystem = (PlayerDevelopmentSystem)this.GetScriptableContainer().ClassList[Array.FindIndex(this.GetScriptableContainer().ClassList, x => x.GetType().Name == "PlayerDevelopmentSystem")];
            return devSystem.PlayerData[Array.FindIndex(devSystem.PlayerData, x => x.Value.OwnerID.Hash == 1)];
        }

        public Dictionary<GameItemID, string> GetEquippedItems()
        {
            var equipSystem = (EquipmentSystem)this.GetScriptableContainer().ClassList[Array.FindIndex(this.GetScriptableContainer().ClassList, x => x.GetType().Name == "EquipmentSystem")];
            var playerEquipAreas = equipSystem.OwnerData.Where(x => x.Value.OwnerID.Hash == 1).FirstOrDefault().Value.Equipment.EquipAreas;

            var equippedItems = new Dictionary<GameItemID, string>();
            var weaponSlotNum = 1;
            foreach (GameSEquipArea area in playerEquipAreas)
            {
                var areaName = area.AreaType.ToString();
                if (area.AreaType == gamedataEquipmentArea.Weapon || area.AreaType == gamedataEquipmentArea.Head || area.AreaType == gamedataEquipmentArea.Face
                    || area.AreaType == gamedataEquipmentArea.OuterChest || area.AreaType == gamedataEquipmentArea.InnerChest || area.AreaType == gamedataEquipmentArea.Legs
                    || area.AreaType == gamedataEquipmentArea.Feet || area.AreaType == gamedataEquipmentArea.QuickSlot || area.AreaType == gamedataEquipmentArea.Consumable
                    || area.AreaType == gamedataEquipmentArea.Outfit)
                {
                    foreach (GameSEquipSlot slot in area.EquipSlots)
                    {
                        if (area.AreaType == gamedataEquipmentArea.Weapon)
                        {
                            areaName = "Weapon " + weaponSlotNum.ToString();
                            weaponSlotNum++;
                        }
                        if (slot.ItemID != null)
                        {
                            equippedItems.Add(slot.ItemID, areaName);
                        }
                    }
                }
                
            }
            return equippedItems;
        }

        public GameStatsStateMapStructure GetStatsMap()
        {
            return (GameStatsStateMapStructure)this.GetStatsContainer().ClassList[Array.FindIndex(this.GetStatsContainer().ClassList, x => x.GetType().FullName.EndsWith("GameStatsStateMapStructure"))];
        }

        public GameSavedStatsData GetItemStatData(ItemData item)
        {
            var result = this.GetStatsFromSeed(item.Header.Seed);

            if (result == null)
            {
                var i = Array.FindIndex(this.GetStatsMap().Values, x => x.RecordID.Id == item.ItemTdbId.Id);
                if (i > -1)
                {
                    return this.GetStatsMap().Values[i];
                }
            }

            return result;
        }

        public GameSavedStatsData GetStatsFromSeed(uint seed)
        {
            var i = Array.FindIndex(this.GetStatsMap().Values, x => x.Seed == seed);
            if (i > -1)
            {
                return this.GetStatsMap().Values[i];
            }
            return null;
        }

        public void SetConstantStat(gamedataStatType stat, float value, GameSavedStatsData statsData)
        {
            var foundStat = false;
            foreach (Handle<GameStatModifierData> modifier in statsData.StatModifiers)
            {
                if (modifier.Value.GetType() == typeof(GameConstantStatModifierData))
                {
                    if (((GameConstantStatModifierData)modifier.Value).StatType == stat)
                    {
                        ((GameConstantStatModifierData)modifier.Value).Value = value;
                        foundStat = true;
                    }
                }
            }

            if (!foundStat)
            {
                var newModifierData = new GameConstantStatModifierData();
                newModifierData.ModifierType = gameStatModifierType.Additive;
                newModifierData.StatType = stat;
                newModifierData.Value = value;

                this.AddStat(typeof(GameConstantStatModifierData), statsData, newModifierData);
            }
        }

        public uint AddStat(Type statType, GameSavedStatsData statsData, GameStatModifierData modifierData = null)
        {
            var newModifierData = Activator.CreateInstance(statType);

            if (statType == typeof(GameCurveStatModifierData))
            {
                ((GameCurveStatModifierData)newModifierData).ColumnName = "<null>";
                ((GameCurveStatModifierData)newModifierData).CurveName = "<null>";

            }

            if (modifierData != null)
            {
                newModifierData = modifierData;
            }

            var newModifier = this.GetStatsContainer().CreateHandle<GameStatModifierData>((GameStatModifierData)newModifierData);
            statsData.StatModifiers = statsData.StatModifiers.Append(newModifier).ToArray();

            return newModifier.Id;
        }

        public void RemoveStat(Handle<GameStatModifierData> statsHandle, GameSavedStatsData statsData)
        {
            var modifiersList = statsData.StatModifiers.ToList();
            modifiersList.Remove(statsHandle);
            statsData.StatModifiers = modifiersList.ToArray();

            this.GetStatsContainer().RemoveHandle(statsHandle.Id);

            foreach (GameSavedStatsData value in this.GetStatsMap().Values)
            {
                if (value.StatModifiers != null)
                {
                    foreach (Handle<GameStatModifierData> modifierData in value.StatModifiers)
                    {
                        if (modifierData.Id > statsHandle.Id)
                        {
                            modifierData.SetId(modifierData.Id - 1);
                        }
                    }
                }
            }
        }

        public List<FactsTable.FactEntry> GetKnownFacts()
        {
            var factsList = new List<FactsTable.FactEntry>();
            foreach (NodeEntry tableEntry in this.GetFactsContainer().Children)
            {
                var tableData = (FactsTable)tableEntry.Value;
                foreach (FactsTable.FactEntry fact in tableData.FactEntries)
                {
                    if (!fact.FactName.StartsWith("Unknown_"))
                    {
                        factsList.Add(fact);
                    }
                }
            }
            return (factsList);
        }

        public bool SetFactByName(string factName, uint factValue)
        {
            var existingFact = this.GetKnownFacts().Where(x => x.FactName == factName).FirstOrDefault();

            if (existingFact != null)
            {
                existingFact.Value = factValue;
                return true;
            }

            return this.AddFactByName(factName, factValue);
        }

        public bool AddFactByName(string factName, uint factValue)
        {
            var factsList = JsonConvert.DeserializeObject<Dictionary<uint, string>>(CP2077SaveEditor.Properties.Resources.Facts);
            if (!factsList.Values.Contains(factName))
            {
                return false;
            }

            var factHash = factsList.FirstOrDefault(x => x.Value == factName).Key;
            AddFactByHash(factHash, factValue);
            return true;
        }

        public void AddFactByHash(uint factHash, uint factValue)
        {
            var newFact = new FactsTable.FactEntry();
            newFact.Hash = factHash;
            newFact.Value = factValue;

            var hashesList = new List<uint>();
            var currentFacts = ((FactsDB)this.GetFactsContainer().Value).FactsTables[0].FactEntries;

            foreach (FactsTable.FactEntry fact in currentFacts)
            {
                hashesList.Add(fact.Hash);
            }
            hashesList.Sort();

            foreach (uint hash in hashesList)
            {
                if (hash > factHash)
                {
                    var i = currentFacts.IndexOf(currentFacts.Where(x => x.Hash == hash).FirstOrDefault());
                    currentFacts.Insert(i, newFact);
                    return;
                }
            }
            currentFacts.Add(newFact);
        }

        public Inventory.SubInventory GetInventory(ulong id)
        {
            return this.GetInventoriesContainer().SubInventories[this.GetInventoriesContainer().SubInventories.IndexOf(this.GetInventoriesContainer().SubInventories.Where(x => x.InventoryId == id).FirstOrDefault())];
        }

        public string GetAppearanceValue(CharacterCustomizationAppearances.Section appearanceSection, AppearanceEntryType entryType, AppearanceField fieldToGet, string searchString)
        {
            foreach (CharacterCustomizationAppearances.AppearanceSection subSection in appearanceSection.AppearanceSections)
            {
                if (entryType == AppearanceEntryType.MainListEntry)
                {
                    foreach (CharacterCustomizationAppearances.HashValueEntry mainListEntry in subSection.MainList)
                    {
                        if (Appearance.CompareMainListAppearanceEntries(mainListEntry.SecondString, searchString) == true)
                        {
                            if (fieldToGet == AppearanceField.FirstString)
                            {
                                return mainListEntry.FirstString;
                            }
                            else if (fieldToGet == AppearanceField.Hash)
                            {
                                return mainListEntry.Hash.ToString();
                            } else {
                                return mainListEntry.SecondString;
                            }
                        }
                    }
                } else {
                    foreach (CharacterCustomizationAppearances.ValueEntry additionalListEntry in subSection.AdditionalList)
                    {
                        if (additionalListEntry.FirstString == searchString)
                        {
                            if (fieldToGet == AppearanceField.FirstString)
                            {
                                return additionalListEntry.FirstString;
                            }
                            else
                            {
                                return additionalListEntry.SecondString;
                            }
                        }
                    }
                }
            }
            return "default";
        }

        public string GetAppearanceValue(string searchString)
        {
            var searchValues = searchString.Split('.');

            if (searchValues.Count() != 4)
            {
                return "default";
            }

            var searchSection = this.GetAppearanceContainer().FirstSection;
            var searchType = AppearanceEntryType.MainListEntry;
            var searchFieldToGet = AppearanceField.FirstString;
            var searchTrueString = searchValues[3];

            if (searchValues[0] == "second")
            {
                searchSection = this.GetAppearanceContainer().SecondSection;
            }
            else if(searchValues[0] == "third")
            {
                searchSection = this.GetAppearanceContainer().ThirdSection;
            }

            if (searchValues[1] == "additional")
            {
                searchType = AppearanceEntryType.AdditionalListEntry;
            }

            if (searchValues[2] == "hash")
            {
                searchFieldToGet = AppearanceField.Hash;
            }
            else if(searchValues[2] == "second")
            {
                searchFieldToGet = AppearanceField.SecondString;
            }
            return GetAppearanceValue(searchSection, searchType, searchFieldToGet, searchTrueString);
        }

        public void SetAllAppearanceValues(CharacterCustomizationAppearances newValues)
        {
            this.GetAppearanceContainer().FirstSection.AppearanceSections.Clear();
            foreach (CharacterCustomizationAppearances.AppearanceSection section in newValues.FirstSection.AppearanceSections)
            {
                this.GetAppearanceContainer().FirstSection.AppearanceSections.Add(section);
            }

            this.GetAppearanceContainer().SecondSection.AppearanceSections.Clear();
            foreach (CharacterCustomizationAppearances.AppearanceSection section in newValues.SecondSection.AppearanceSections)
            {
                this.GetAppearanceContainer().SecondSection.AppearanceSections.Add(section);
            }

            this.GetAppearanceContainer().ThirdSection.AppearanceSections.Clear();
            foreach (CharacterCustomizationAppearances.AppearanceSection section in newValues.ThirdSection.AppearanceSections)
            {
                this.GetAppearanceContainer().ThirdSection.AppearanceSections.Add(section);
            }

            if (newValues.Strings != null)
            {
                this.GetAppearanceContainer().Strings.Clear();
                foreach (string singleString in newValues.Strings)
                {
                    this.GetAppearanceContainer().Strings.Add(singleString);
                }
            }
        }

        public void SetLinearAppearanceValue(string fieldName, int fieldNum, int value)
        {
            var lists = new[] { this.GetAppearanceContainer().FirstSection.AppearanceSections[0], this.GetAppearanceContainer().FirstSection.AppearanceSections[3] };
            var entries = new List<CharacterCustomizationAppearances.ValueEntry>();

            foreach (CharacterCustomizationAppearances.AppearanceSection section in lists)
            {
                entries.Add(section.AdditionalList.Where(x => x.FirstString == fieldName).FirstOrDefault());
            }

            if (entries[0] == null)
            {
                var newEntry = new CharacterCustomizationAppearances.ValueEntry();
                newEntry.FirstString = fieldName;
                newEntry.SecondString = "h000";

                foreach (CharacterCustomizationAppearances.AppearanceSection section in lists)
                {
                    section.AdditionalList.Add(newEntry);
                }
                SetLinearAppearanceValue(fieldName, fieldNum, value);
            } else {
                if (value == 1)
                {
                    var i = 0;
                    foreach (CharacterCustomizationAppearances.AppearanceSection section in lists)
                    {
                        section.AdditionalList.Remove(entries[i]);
                        i++;
                    }
                }
                else
                {
                    foreach (CharacterCustomizationAppearances.ValueEntry entry in entries)
                    {
                        entry.SecondString = "h" + (value - 1).ToString("00") + fieldNum.ToString();
                    }
                }
            }
        }

        public class AppearanceHelper {

            public Dictionary<string, ulong> HairStyles { get; } = JsonConvert.DeserializeObject<Dictionary<string, ulong>>(CP2077SaveEditor.Properties.Resources.HairStyles);
            public List<string> HairColors { get; } = JsonConvert.DeserializeObject<List<string>>(CP2077SaveEditor.Properties.Resources.HairColors);
            public List<string> SkinColors { get; } = JsonConvert.DeserializeObject<List<string>>(CP2077SaveEditor.Properties.Resources.SkinColors);
            public List<string> EyeColors { get; } = JsonConvert.DeserializeObject<List<string>>(CP2077SaveEditor.Properties.Resources.EyeColors);

            private SaveFileHelper activeSave;

            public AppearanceHelper(SaveFileHelper _saveFile)
            {
                activeSave = _saveFile;
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

            public void SetEntryField(AppearanceField field, string searchString, object value)
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

            public void SetHairStyle(string friendlyName)
            {
                if (friendlyName != "Shaved")
                {
                    SetEntryField(AppearanceField.Hash, "first.main.hair_color", HairStyles[friendlyName]);
                }
            }

            public void SetHairColor(string colorString)
            {
                if (colorString != "None")
                {
                    SetEntryField(AppearanceField.FirstString, "first.main.hair_color", colorString);
                    if (activeSave.GetAppearanceContainer().Strings.Count < 1)
                    {
                        activeSave.GetAppearanceContainer().Strings.Add(colorString.Substring(3));
                        activeSave.GetAppearanceContainer().Strings.Add("Short");
                    } else {
                        activeSave.GetAppearanceContainer().Strings[0] = colorString.Substring(3);
                    }
                }
            }

            public void CreateHairEntry(string friendlyName)
            {
                var hairsList = activeSave.GetAppearanceContainer().FirstSection.AppearanceSections.Where(x => x.SectionName == "hairs").FirstOrDefault().MainList;

                var newEntry = new CharacterCustomizationAppearances.HashValueEntry();
                newEntry.FirstString = HairColors[0];
                newEntry.Hash = HairStyles[friendlyName];
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

            public void SetConcatedValue(string searchString, string newValue, int position = -1)
            {
                string currentValue;

                if (position < 0)
                {
                    currentValue = activeSave.GetAppearanceValue(searchString).Split("__", StringSplitOptions.None).Last();
                } else {
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

    }

}
