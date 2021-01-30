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
            var playerEquipAreas = GetEquipAreas();
            var equippedItems = new Dictionary<GameItemID, string>();

            var weaponSlotNum = 1;
            foreach (GameSEquipArea area in playerEquipAreas)
            {
                var areaName = area.AreaType.ToString();
                if (area.EquipSlots != null)
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

        public GameSEquipArea[] GetEquipAreas()
        {
            var equipSystem = (EquipmentSystem)this.GetScriptableContainer().ClassList[Array.FindIndex(this.GetScriptableContainer().ClassList, x => x.GetType().Name == "EquipmentSystem")];
            return equipSystem.OwnerData.Where(x => x.Value.OwnerID.Hash == 1).FirstOrDefault().Value.Equipment.EquipAreas;
        }

        public List<GameSEquipSlot> GetEquipSlotsFromID(GameItemID id)
        {
            var slots = new List<GameSEquipSlot>();
            foreach (GameSEquipArea area in this.GetEquipAreas())
            {
                if (area.EquipSlots != null)
                {
                    foreach (GameSEquipSlot slot in area.EquipSlots)
                    {
                        if (slot.ItemID != null && slot.ItemID.Id.Raw64 == id.Id.Raw64)
                        {
                            slots.Add(slot);
                        }
                    }
                }
            }
            return slots;
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
                            modifierData.Id = (modifierData.Id - 1);
                        }
                    }
                }
            }
        }

        public GameSavedStatsData CreateStatData(ItemData item, Random rand)
        {
            var randBytes = new byte[4];
            rand.NextBytes(randBytes);
            var newSeed = BitConverter.ToUInt32(randBytes);

            while (this.GetStatsMap().Values.Where(x => x.Seed == newSeed).FirstOrDefault() != null)
            {
                rand.NextBytes(randBytes);
                newSeed = BitConverter.ToUInt32(randBytes);
            }

            this.GetStatsMap().Keys = this.GetStatsMap().Keys.Append(new GameStatsObjectID
            {
                IdType = gameStatIDType.ItemID,
                EntityHash = GetItemIdHash(item.ItemTdbId.Raw64, newSeed, 0)
            }).ToArray();

            var statsEntry = new GameSavedStatsData
            {
                RecordID = item.ItemTdbId,
                Seed = newSeed,
                StatModifiers = new Handle<GameStatModifierData>[0]
            };

            this.GetStatsMap().Values = this.GetStatsMap().Values.Append(statsEntry).ToArray();
            item.Header.Seed = newSeed;
            return statsEntry;
        }

        //Credit to Seberoth
        public static ulong GetItemIdHash(ulong tweakDbId, uint seed, ushort unk1 = 0)
        {
            ulong c = 0xC6A4A7935BD1E995;

            ulong tmp;

            if (unk1 == 0)
            {
                tmp = seed * c;
                tmp = tmp >> 0x2F ^ tmp;
            }
            else
            {
                tmp = unk1 * c;
                tmp = ((tmp >> 0x2f ^ tmp) * c ^ seed) * 0x35A98F4D286A90B9;
                tmp = tmp >> 0x2F ^ tmp;
            }

            return (tmp * c ^ tweakDbId) * c;
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
