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

        public string GetFacialFeatureValue(string name)
        {
            var facialFeatures = this.GetAppearanceContainer().FirstSection.AppearanceSections[0].AdditionalList;
            var i = facialFeatures.FindIndex(x => x.FirstString == name);

            if (i > -1)
            {
                return facialFeatures[i].SecondString;
            }
            else
            {
                return "default";
            }
        }

        public void SetAllAppearanceValues(CharacterCustomizationAppearances newValues)
        {
            this.GetAppearanceContainer().FirstSection = newValues.FirstSection;
            this.GetAppearanceContainer().SecondSection = newValues.SecondSection;
            this.GetAppearanceContainer().ThirdSection = newValues.ThirdSection;
        }

        public void SetAppearanceSectionSafely(CharacterCustomizationAppearances.Section currentSection, CharacterCustomizationAppearances.Section newSection)
        {
            foreach (CharacterCustomizationAppearances.AppearanceSection currentSubSection in currentSection.AppearanceSections)
            {
                var i = newSection.AppearanceSections.FindIndex(x => x.SectionName == currentSubSection.SectionName);
                if (i < 0) { continue; }

                var newSubSection = newSection.AppearanceSections[i];
                foreach (CharacterCustomizationAppearances.HashValueEntry mainListEntry in currentSubSection.MainList)
                {
                    i = newSubSection.MainList.FindIndex(x => CompareMainListAppearanceEntries(x, mainListEntry) == true);
                    if (i < 0) { continue; }

                    var newMainListEntry = newSubSection.MainList[i];
                    //mainListEntry.FirstString = newMainListEntry.FirstString; -- Resizing issues.
                    mainListEntry.Hash = newMainListEntry.Hash;
                    //mainListEntry.SecondString = newMainListEntry.SecondString; -- Resizing issues.
                }
                foreach (CharacterCustomizationAppearances.ValueEntry additionalListEntry in currentSubSection.AdditionalList)
                {
                    i = newSubSection.AdditionalList.FindIndex(x => x.FirstString == additionalListEntry.FirstString);
                    if (i < 0) { continue; }

                    var newAdditionalListEntry = newSubSection.AdditionalList[i];
                    additionalListEntry.SecondString = newAdditionalListEntry.SecondString;
                }
            }
        }

        private bool CompareMainListAppearanceEntries(CharacterCustomizationAppearances.HashValueEntry entry1, CharacterCustomizationAppearances.HashValueEntry entry2)
        {
            return Regex.Replace(entry1.SecondString, @"[\d-]", string.Empty) == Regex.Replace(entry2.SecondString, @"[\d-]", string.Empty);
        }

    }
}
