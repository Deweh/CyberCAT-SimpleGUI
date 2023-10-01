using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Save;
using static WolvenKit.RED4.Types.Enums;
using WolvenKit.RED4.Types;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Save.Classes;

namespace CP2077SaveEditor
{
    public class SaveFileHelper
    {
        public AppearanceHelper2 Appearance { get; }
        public CyberpunkSaveFile SaveFile { get; set; }
        public Dictionary<uint, string> KnownFacts;

        public byte[] Metadata { get; set; }
        public byte[] ImageData { get; set; }

        public SaveFileHelper() : base()
        {
            Appearance = new AppearanceHelper2(this);
        }

        public gameuiCharacterCustomizationPresetWrapper GetAppearanceContainer()
        {
            return (gameuiCharacterCustomizationPresetWrapper)SaveFile.Nodes[SaveFile.Nodes.FindIndex(x => x.Name == "CharacetrCustomization_Appearances")].Value;
        }

        public void SetAppearanceContainer(gameuiCharacterCustomizationPresetWrapper preset)
        {
            SaveFile.Nodes[SaveFile.Nodes.FindIndex(x => x.Name == "CharacetrCustomization_Appearances")].Value = preset;
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

        public gameSavedStatsData GetItemStatData(ItemData item)
        {
            var result = GetStatsFromItemId(item.ItemInfo.ItemId);

            if (result == null && item.ItemSlotPart != null && item.IsQuantityOnly())
            {
                var i = Array.FindIndex(this.GetStatsMap().Values.ToArray(), x => x.RecordID == item.ItemInfo.ItemId.Id);
                if (i > -1)
                {
                    return this.GetStatsMap().Values[i];
                }
            }

            return result;
        }

        public gameSavedStatsData GetStatsFromItemId(gameItemID itemId)
        {
            var id = InventoryHelper.GetItemIdHash(itemId);

            var statsMap = GetStatsMap();

            var index = -1;
            for (var i = 0; i < statsMap.Keys.Count; i++)
            {
                if (statsMap.Keys[i].EntityHash == id)
                {
                    index = i;
                    break;
                }
            }

            if (index > -1)
            {
                return this.GetStatsMap().Values[index];
            }
            return null;
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
            foreach (gameStatModifierData_Deprecated modifier in ((ModifiersBuffer)statsData.ModifiersBuffer.Data!).Entries)
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

        public gameStatModifierData_Deprecated AddStat(Type statType, gameSavedStatsData statsData, gameStatModifierData_Deprecated modifierData = null)
        {
            if (modifierData == null)
            {
                modifierData = (gameStatModifierData_Deprecated)System.Activator.CreateInstance(statType);

                if (modifierData is gameCurveStatModifierData_Deprecated curveStat)
                {
                    curveStat.ColumnName = "<null>";
                    curveStat.CurveName = "<null>";
                }
            }

            statsData.ModifiersBuffer.Data ??= new ModifiersBuffer();

            ((ModifiersBuffer)statsData.ModifiersBuffer.Data).Entries.Add(modifierData);

            return modifierData;
        }

        public void RemoveStat(gameStatModifierData_Deprecated stats, gameSavedStatsData statsData)
        {
            ((ModifiersBuffer)statsData.ModifiersBuffer.Data)!.Entries.Remove(stats);
        }

        public uint CreateUniqueSeed(gameStatsStateMapStructure statsMap = null)
        {
            statsMap ??= GetStatsContainer();

            var rand = new Random();

            do
            {
                var seed = (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
                if (statsMap.Values.FirstOrDefault(x => x.Seed == seed) == null)
                {
                    return seed;
                }
            } while (true);
        }

        public gameSavedStatsData CreateStatData(gameItemID gameItemId)
        {
            var statsMap = GetStatsContainer();

            if (gameItemId.RngSeed == 0)
            {
                gameItemId.RngSeed = CreateUniqueSeed(statsMap);
            }

            if (gameItemId.UniqueCounter == 0)
            {
                gameItemId.RngSeed = GetNextUniqueCounter();
            }

            statsMap.Keys.Add(new gameStatsObjectID
            {
                IdType = gameStatIDType.ItemID,
                EntityHash = InventoryHelper.GetItemIdHash(gameItemId)
            });

            var statsData = new gameSavedStatsData
            {
                RecordID = gameItemId.Id,
                Seed = gameItemId.RngSeed,
                StatModifiers = new(),
                ModifiersBuffer = new DataBuffer
                {
                    Data = new ModifiersBuffer()
                }
            };

            if (ResourceHelper.ItemClasses.TryGetValue(gameItemId.Id, out var itemRecord))
            {
                if (itemRecord.Type == "WeaponItem")
                {
                    statsData.InactiveStats.Add(Enums.gamedataStatType.IsItemPlus);

                    ((ModifiersBuffer)statsData.ModifiersBuffer.Data).Entries.Add(new gameConstantStatModifierData_Deprecated
                    {
                        StatType = gamedataStatType.IsItemPlus,
                        ModifierType = gameStatModifierType.Additive,
                        Value = 0
                    });
                }

                if (itemRecord.Type == "Clothing")
                {
                    ((ModifiersBuffer)statsData.ModifiersBuffer.Data).Entries.Add(new gameConstantStatModifierData_Deprecated
                    {
                        StatType = gamedataStatType.LootLevel,
                        ModifierType = gameStatModifierType.Additive,
                        Value = 1
                    });
                }
            }

            statsMap.Values.Add(statsData);

            return statsData;
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

        public SubInventory GetInventory(ulong id)
        {
            return this.GetInventoriesContainer().SubInventories.FirstOrDefault(x => x.InventoryId == id);
        }

        public ushort GetNextUniqueCounter()
        {
            var uniqueItemCounterNode = (UniqueItemCounter)SaveFile.Nodes[SaveFile.Nodes.FindIndex(x => x.Name == Constants.NodeNames.UNIQUE_ITEM_COUNTER)].Value;

            return ++uniqueItemCounterNode.Count;
        }
    }

}
