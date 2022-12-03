using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WolvenKit.RED4.Save;
using static WolvenKit.RED4.Types.Enums;
using WolvenKit.RED4.Types;
using WolvenKit.RED4.Archive.Buffer;

namespace CP2077SaveEditor
{
    public class SaveFileHelper
    {
        public AppearanceHelper Appearance { get; }
        public CyberpunkSaveFile SaveFile { get; set; }
        public Dictionary<uint, string> KnownFacts;

        public SaveFileHelper() : base()
        {
            Appearance = new AppearanceHelper(this);
        }

        public gameuiCharacterCustomizationPresetWrapper GetAppearanceContainer()
        {
            return (gameuiCharacterCustomizationPresetWrapper)SaveFile.Nodes[SaveFile.Nodes.FindIndex(x => x.Name == "CharacetrCustomization_Appearances")].Value;
        }

        public Inventory GetInventoriesContainer()
        {
            return (Inventory)SaveFile.Nodes[SaveFile.Nodes.FindIndex(x => x.Name == "inventory")].Value;
        }

        public NodeEntry GetFactsContainer()
        {
            var questSystem = SaveFile.Nodes[SaveFile.Nodes.FindIndex(x => x.Name == "questSystem")]; return questSystem.Children[questSystem.Children.FindIndex(x => x.Name == "FactsDB")];
        }

        public gameStatsStateMapStructure GetStatsContainer()
        {
            return (gameStatsStateMapStructure)((RedPackage)((Package)SaveFile.Nodes[SaveFile.Nodes.FindIndex(x => x.Name == "StatsSystem")].Value).Content).RootChunk;
        }

        public RedPackage GetScriptableContainer()
        {
            return (RedPackage)((Package)SaveFile.Nodes[SaveFile.Nodes.FindIndex(x => x.Name == "ScriptableSystemsContainer")].Value).Content;
        }

        public T GetScriptableSystem<T>() where T : gameScriptableSystem
        {
            return (T)GetScriptableContainer().Chunks.FirstOrDefault(x => x is T);
        }

        public PlayerDevelopmentData GetPlayerDevelopmentData()
        {
            var devSystem = (PlayerDevelopmentSystem)this.GetScriptableContainer().Chunks.FirstOrDefault(x => x is PlayerDevelopmentSystem);
            return devSystem.PlayerData.FirstOrDefault(x => x.Chunk.OwnerID.Hash == 1).Chunk;
        }

        public PersistencySystem2 GetPSDataContainer()
        {
            return GetSystem<PersistencySystem2>();
        }

        public T GetSystem<T>()
        {
            return (T)SaveFile.Nodes.FirstOrDefault(x => x.Value.GetType() == typeof(T))?.Value;
        }

        public Dictionary<gameItemID, string> GetEquippedItems()
        {
            var playerEquipAreas = GetEquipAreas();
            var equippedItems = new Dictionary<gameItemID, string>();

            var weaponSlotNum = 1;
            foreach (gameSEquipArea area in playerEquipAreas)
            {
                var areaName = area.AreaType.ToString();
                if (area.EquipSlots != null)
                {
                    foreach (gameSEquipSlot slot in area.EquipSlots)
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

        public CArray<gameSEquipArea> GetEquipAreas()
        {
            var equipSystem = (EquipmentSystem)this.GetScriptableContainer().Chunks.FirstOrDefault(x => x is EquipmentSystem);
            return equipSystem.OwnerData.FirstOrDefault(x => x.Chunk.OwnerID.Hash == 1).Chunk.Equipment.EquipAreas;
        }

        public List<gameSEquipSlot> GetEquipSlotsFromID(gameItemID id)
        {
            var slots = new List<gameSEquipSlot>();
            foreach (gameSEquipArea area in this.GetEquipAreas())
            {
                if (area.EquipSlots != null)
                {
                    foreach (gameSEquipSlot slot in area.EquipSlots)
                    {
                        if (slot.ItemID != null && slot.ItemID.Id == id.Id)
                        {
                            slots.Add(slot);
                        }
                    }
                }
            }
            return slots;
        }

        /// <summary>
        /// Deprecated function. Use GetStatsContainer() instead.
        /// </summary>
        /// <returns></returns>
        public gameStatsStateMapStructure GetStatsMap()
        {
            return GetStatsContainer();
        }

        public gameSavedStatsData GetItemStatData(InventoryHelper.ItemData item)
        {
            var result = this.GetStatsFromSeed(item.Header.ItemId.RngSeed);

            if (result == null && item.Data != null && item.Data is InventoryHelper.SimpleItemData)
            {
                var i = Array.FindIndex(this.GetStatsMap().Values.ToArray(), x => x.RecordID == item.Header.ItemId.Id);
                if (i > -1)
                {
                    return this.GetStatsMap().Values[i];
                }
            }

            return result;
        }

        public gameSavedStatsData GetStatsFromSeed(uint seed)
        {
            var i = Array.FindIndex(this.GetStatsMap().Values.ToArray(), x => x.Seed == seed);
            if (i > -1)
            {
                return this.GetStatsMap().Values[i];
            }
            return null;
        }

        public void SetConstantStat(gamedataStatType stat, float value, gameSavedStatsData statsData, gameStatModifierType mod = gameStatModifierType.Additive)
        {
            var foundStat = false;
            foreach (gameStatModifierData_Deprecated modifier in statsData.StatModifiers)
            {
                if (modifier is gameConstantStatModifierData_Deprecated constantModifier)
                {
                    if (constantModifier.StatType == stat && constantModifier.ModifierType == mod)
                    {
                        constantModifier.Value = value;
                        foundStat = true;
                    }
                }
            }

            if (!foundStat)
            {
                var newModifierData = new gameConstantStatModifierData_Deprecated
                {
                    ModifierType = mod,
                    StatType = stat,
                    Value = value
                };

                this.AddStat(typeof(gameConstantStatModifierData_Deprecated), statsData, newModifierData);
            }
        }

        public CHandle<gameStatModifierData_Deprecated> AddStat(Type statType, gameSavedStatsData statsData, gameStatModifierData_Deprecated modifierData = null)
        {
            var newModifierData = System.Activator.CreateInstance(statType);

            if (statType == typeof(gameCurveStatModifierData_Deprecated))
            {
                ((gameCurveStatModifierData_Deprecated)newModifierData).ColumnName = "<null>";
                ((gameCurveStatModifierData_Deprecated)newModifierData).CurveName = "<null>";

            }

            if (modifierData != null)
            {
                newModifierData = modifierData;
            }

            var final = new CHandle<gameStatModifierData_Deprecated>((gameStatModifierData_Deprecated)newModifierData);
            statsData.StatModifiers.Add(final);

            return final;
        }

        public void RemoveStat(CHandle<gameStatModifierData_Deprecated> statsHandle, gameSavedStatsData statsData)
        {
            statsData.StatModifiers.Remove(statsHandle);
        }

        public gameSavedStatsData CreateStatData(InventoryHelper.ItemData item, Random rand)
        {
            var randBytes = new byte[4];
            rand.NextBytes(randBytes);
            var newSeed = BitConverter.ToUInt32(randBytes);

            while (this.GetStatsMap().Values.Where(x => x.Seed == newSeed).FirstOrDefault() != null)
            {
                rand.NextBytes(randBytes);
                newSeed = BitConverter.ToUInt32(randBytes);
            }

            if (item.Data.GetType() != typeof(InventoryHelper.SimpleItemData))
            {
                item.Header.ItemId.RngSeed = newSeed;
            }
            else
            {
                newSeed = 2;
            }

            this.GetStatsMap().Keys.Add(new gameStatsObjectID
            {
                IdType = gameStatIDType.ItemID,
                EntityHash = GetItemIdHash(item.Header.ItemId.Id, newSeed, 0)
            });

            var statsEntry = new gameSavedStatsData
            {
                RecordID = item.Header.ItemId.Id,
                Seed = newSeed,
                StatModifiers = new()
            };

            this.GetStatsMap().Values.Add(statsEntry);
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
            if (KnownFacts == null)
            {
                KnownFacts = JsonSerializer.Deserialize<Dictionary<uint, string>>(CP2077SaveEditor.Properties.Resources.Facts);
            }

            var factsList = new List<FactsTable.FactEntry>();
            foreach (NodeEntry tableEntry in this.GetFactsContainer().Children)
            {
                var tableData = (FactsTable)tableEntry.Value;
                foreach (FactsTable.FactEntry fact in tableData.FactEntries)
                {
                    var hash = (uint)fact.FactName;

                    if (KnownFacts.ContainsKey(hash))
                    {
                        factsList.Add(fact);
                    }
                }
            }
            return (factsList);
        }

        public bool SetFactByName(string factName, uint factValue)
        {
            var facts = GetKnownFacts();

            if (KnownFacts.ContainsValue(factName) && facts.Any(x => KnownFacts[x.FactName] == factName))
            {
                facts.First(x => KnownFacts[x.FactName] == factName).Value = factValue;
                return true;
            }
            else
            {
                return AddFactByName(factName, factValue);
            }
        }

        public bool AddFactByName(string factName, uint factValue)
        {
            if (!KnownFacts.Values.Contains(factName))
            {
                return false;
            }

            var factHash = KnownFacts.FirstOrDefault(x => x.Value == factName).Key;
            AddFactByHash(factHash, factValue);
            return true;
        }

        public void AddFactByHash(uint factHash, uint factValue)
        {
            var newFact = new FactsTable.FactEntry();
            newFact.FactName = factHash;
            newFact.Value = factValue;

            ((FactsDB)GetFactsContainer().Value).FactsTables[0].FactEntries.Add(newFact);
        }

        public InventoryHelper.SubInventory GetInventory(ulong id)
        {
            return this.GetInventoriesContainer().SubInventories.FirstOrDefault(x => x.InventoryId == id);
        }
    }

}
