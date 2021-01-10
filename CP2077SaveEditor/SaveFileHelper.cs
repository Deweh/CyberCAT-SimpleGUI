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
using Newtonsoft.Json;

namespace CP2077SaveEditor
{
    class SaveFileHelper : SaveFile
    {

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

        public SaveFileHelper(IEnumerable<INodeParser> parsers) : base(parsers) {}

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

        public GameStatsStateMapStructure GetStatsMap()
        {
            return (GameStatsStateMapStructure)this.GetStatsContainer().ClassList[Array.FindIndex(this.GetStatsContainer().ClassList, x => x.GetType().FullName.EndsWith("GameStatsStateMapStructure"))];
        }

        public GameSavedStatsData GetItemStatData(ItemData item)
        {
            var i = Array.FindIndex(this.GetStatsMap().Values, x => x.Seed == item.Header.Seed);
            if (i > -1)
            {
                return this.GetStatsMap().Values[i];
            }
            return null;
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

        public void AddFactByName(string factName, uint factValue)
        {
            var factsList = JsonConvert.DeserializeObject<Dictionary<uint, string>>(CP2077SaveEditor.Properties.Resources.Facts);
            if (!factsList.Values.Contains(factName))
            {
                MessageBox.Show("Fact name '" + factName + "' could not be found on the known facts list.");
            }

            var factHash = factsList.FirstOrDefault(x => x.Value == factName).Key;
            AddFactByHash(factHash, factValue);
        }

        public void AddFactByHash(uint factHash, uint factValue)
        {
            var newFact = new FactsTable.FactEntry();
            newFact.Hash = factHash;
            newFact.Value = factValue;

            ((FactsTable)this.GetFactsContainer().Children[0].Value).FactEntries.Add(newFact);
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
                        if (CompareMainListAppearanceEntries(mainListEntry.SecondString, searchString) == true)
                        {
                            if (fieldToGet == AppearanceField.FirstString)
                            {
                                return mainListEntry.FirstString;
                            }
                            else if (fieldToGet == AppearanceField.Hash)
                            {
                                return "hash(" + mainListEntry.Hash.ToString() + ")";
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

        private bool CompareMainListAppearanceEntries(string entry1, string entry2)
        {
            return Regex.Replace(entry1, @"[\d-]", string.Empty) == Regex.Replace(entry2, @"[\d-]", string.Empty);
        }

    }
}
