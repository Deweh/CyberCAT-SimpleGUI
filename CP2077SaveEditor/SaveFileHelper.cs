using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using CyberCAT.Core.Classes;
using CyberCAT.Core.Classes.Interfaces;
using CyberCAT.Core.Classes.NodeRepresentations;

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

        public Inventory.SubInventory GetInventory(ulong id)
        {
            return this.GetInventoriesContainer().SubInventories[Array.FindIndex(this.GetInventoriesContainer().SubInventories, x => x.InventoryId == id)];
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

        public void SetAllAppearanceValues(CharacterCustomizationAppearances newValues)
        {
            this.GetAppearanceContainer().FirstSection = newValues.FirstSection;
            this.GetAppearanceContainer().SecondSection = newValues.SecondSection;
            this.GetAppearanceContainer().ThirdSection = newValues.ThirdSection;
        }

        private bool CompareMainListAppearanceEntries(string entry1, string entry2)
        {
            return Regex.Replace(entry1, @"[\d-]", string.Empty) == Regex.Replace(entry2, @"[\d-]", string.Empty);
        }

    }
}
