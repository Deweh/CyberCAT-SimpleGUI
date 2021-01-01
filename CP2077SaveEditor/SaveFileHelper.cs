using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            this.GetAppearanceContainer().FirstSection.AppearanceSections[0].AdditionalList = newValues.FirstSection.AppearanceSections[0].AdditionalList;

            var i = 0;
            foreach (CharacterCustomizationAppearances.AppearanceSection section in this.GetAppearanceContainer().FirstSection.AppearanceSections)
            {
                section.AdditionalList = newValues.FirstSection.AppearanceSections[i].AdditionalList;
                i++;
            }
        }

    }
}
