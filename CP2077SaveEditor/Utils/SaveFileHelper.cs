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
    public class SaveFileHelper : SaveFile
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

        public SaveFileHelper(IEnumerable<INodeParser> parsers) : base(parsers)
        {
            Appearance = new AppearanceHelper(this);
        }

        public new void Load(System.IO.Stream inputStream)
        {
            base.Load(inputStream);
            Appearance.SetMainSections();
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

            if (result == null && item.Data != null && item.Data is ItemData.SimpleItemData)
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
    }

}
