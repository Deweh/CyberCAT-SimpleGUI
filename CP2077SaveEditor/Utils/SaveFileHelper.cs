using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using CP2077SaveEditor.Utils;
using WolvenKit.RED4.Save;
using static WolvenKit.RED4.Types.Enums;
using WolvenKit.RED4.Types;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Save.IO;

namespace CP2077SaveEditor
{
    public class SaveFileHelper
    {
        public AppearanceHelper2 Appearance { get; }
        public CyberpunkSaveFile SaveFile { get; set; }
        public Dictionary<uint, string> KnownFacts;

        public byte[] Metadata { get; set; }
        public byte[] ImageData { get; set; }

        private OptimizedStatsHelper _optimizedStats;
        private bool _optimizedStatsChecked;

        /// <summary>
        /// Non-null for saves from patch 2.5+, which store stats in the optimized format.
        /// </summary>
        private OptimizedStatsHelper OptimizedStats
        {
            get
            {
                if (!_optimizedStatsChecked)
                {
                    var statsMap = GetStatsContainer();
                    _optimizedStats = OptimizedStatsHelper.IsOptimized(statsMap) ? new OptimizedStatsHelper(statsMap) : null;
                    _optimizedStatsChecked = true;
                }

                return _optimizedStats;
            }
        }

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

        public RedPackage GetGodModeSystem()
        {
            return (RedPackage)((Package)SaveFile.Nodes[SaveFile.Nodes.FindIndex(x => x.Name == Constants.NodeNames.GOD_MODE_SYSTEM)].Value).Content;
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

        public gameSavedStatsData GetStatsFromEntityId(ulong hash)
        {
            return GetStatsFromStatsObjectId(new gameStatsObjectID { IdType = gameStatIDType.EntityID, EntityHash = hash });
        }

        public gameSavedStatsData GetStatsFromItemId(gameItemID itemId)
        {
            return GetStatsFromStatsObjectId(new gameStatsObjectID { IdType = gameStatIDType.ItemID, EntityHash = InventoryHelper.GetItemIdHash(itemId) });
        }

        public gameSavedStatsData GetStatsFromStatsObjectId(gameStatsObjectID id)
        {
            var index = GetStatsContainer().Keys.IndexOf(id);
            if (index == -1)
            {
                return OptimizedStats?.Get(id);
            }
            return GetStatsContainer().Values[index];
        }

        public void AddStatData(gameStatsObjectID id, gameSavedStatsData statsData)
        {
            if (OptimizedStats != null)
            {
                OptimizedStats.Add(id, statsData);
                return;
            }

            var statsMap = GetStatsContainer();
            statsMap.Keys.Add(id);
            statsMap.Values.Add(statsData);
        }

        /// <summary>
        /// Creates a new item the way the game structures it (quantity, slot parts, stats). It isn't added to any inventory.
        /// </summary>
        public ItemData CreateItem(TweakDBID id, ItemRecord itemRecordToCreate, uint quantity)
        {
            var item = new ItemData
            {
                ItemInfo = new ItemInfo
                {
                    ItemId = new gameItemID
                    {
                        Id = id,
                        UniqueCounter = GetNextUniqueCounter()
                    }
                }
            };

            if (itemRecordToCreate.Type == "Grenade")
            {
                item.ItemInfo.ItemStructure = ItemStructure.Quantity | ItemStructure.Extended;
                item.ItemInfo.ItemId.RngSeed = 2;

                item.Quantity = quantity;
                item.ItemAdditionalInfo = new ItemAdditionalInfo();
                item.ItemSlotPart = new ItemSlotPart
                {
                    ItemInfo = new ItemInfo
                    {
                        ItemId = new gameItemID()
                    },
                    AppearanceName = "None",
                    ItemAdditionalInfo = new ItemAdditionalInfo()
                };
            }
            else
            {
                if (itemRecordToCreate.IsSingleInstance)
                {
                    item.ItemInfo.ItemStructure = ItemStructure.None; // or 2 with any seed
                    item.ItemInfo.ItemId.RngSeed = 2;

                    item.Quantity = quantity;
                }
                else
                {
                    item.ItemInfo.ItemStructure = ItemStructure.None; // or 1 with any seed
                    item.ItemInfo.ItemId.RngSeed = CreateUniqueSeed();

                    item.ItemAdditionalInfo = new ItemAdditionalInfo();
                    item.ItemSlotPart = new ItemSlotPart
                    {
                        ItemInfo = new ItemInfo
                        {
                            ItemId = new gameItemID()
                        },
                        AppearanceName = "None",
                        ItemAdditionalInfo = new ItemAdditionalInfo()
                    };
                }
            }

            if (item.HasExtendedData())
            {
                if (ResourceHelper.ItemClasses.TryGetValue(item.ItemInfo.ItemId.Id, out var itemRecord))
                {
                    if (itemRecord.Type == "Clothing" || itemRecord.Type == "Weapon" || itemRecord.Type == "WeaponItem")
                    {
                        CreateStatData(item.ItemInfo.ItemId);
                    }

                    if (itemRecord.SlotParts is { Count: > 0 })
                    {
                        item.ItemSlotPart = CreateSlotPart(itemRecord.SlotParts[0].ItemPartPreset, itemRecord.SlotParts[0].Slot);

                        if (itemRecord.SlotParts.Count > 1)
                        {
                            var childMods = new List<ItemSlotPart>();
                            for (int i = 1; i < itemRecord.SlotParts.Count; i++)
                            {
                                var subMod = CreateSlotPart(itemRecord.SlotParts[i].ItemPartPreset, itemRecord.SlotParts[i].Slot);

                                childMods.Add(subMod);
                            }

                            item.ItemSlotPart!.Children = childMods;
                        }
                    }
                }
            }

            return item;

            ItemSlotPart CreateSlotPart(TweakDBID id, TweakDBID slot)
            {
                return new ItemSlotPart
                {
                    ItemInfo = new ItemInfo
                    {
                        ItemId = new gameItemID
                        {
                            Id = id,
                            RngSeed = CreateUniqueSeed(),
                            UniqueCounter = GetNextUniqueCounter()
                        }
                    },
                    AppearanceName = "None",
                    AttachmentSlotTdbId = slot,
                    ItemAdditionalInfo = new ItemAdditionalInfo
                    {
                        RequiredLevel = float.MaxValue
                    }
                };
            }
        }

        /// <summary>
        /// Gives targetItemId its own copy of sourceItemId's stats (modifiers, groups...), e.g. for a duplicated item.
        /// </summary>
        /// <summary>
        /// Copy of an item with new ids (seed and unique counter) and its own copy of the stats, including the mods attached to it.
        /// The copy is not added to any inventory.
        /// </summary>
        public ItemData CloneItem(ItemData source)
        {
            var newSeed = source.ItemInfo.ItemId.RngSeed == 2 ? 2u : CreateUniqueSeed();
            var newItemId = new gameItemID
            {
                Id = source.ItemInfo.ItemId.Id,
                RngSeed = newSeed,
                UniqueCounter = GetNextUniqueCounter(),
                Flags = source.ItemInfo.ItemId.Flags
            };

            var clone = new ItemData
            {
                ItemInfo = new ItemInfo
                {
                    ItemId = newItemId,
                    ItemStructure = source.ItemInfo.ItemStructure
                },
                Flags = source.Flags,
                CreationTime = source.CreationTime,
                Quantity = source.Quantity,
                ItemAdditionalInfo = CloneAdditionalInfo(source.ItemAdditionalInfo),
                ItemSlotPart = CloneSlotPart(source.ItemSlotPart)
            };

            CloneStats(source.ItemInfo.ItemId, newItemId);
            return clone;
        }

        private static ItemAdditionalInfo CloneAdditionalInfo(ItemAdditionalInfo source)
        {
            if (source == null)
            {
                return null;
            }

            return new ItemAdditionalInfo
            {
                LootItemPoolId = source.LootItemPoolId,
                Unknown2 = source.Unknown2,
                RequiredLevel = source.RequiredLevel
            };
        }

        private ItemSlotPart CloneSlotPart(ItemSlotPart source)
        {
            if (source == null)
            {
                return null;
            }

            var newSeed = source.ItemInfo.ItemId.RngSeed == 0
                ? 0u
                : (source.ItemInfo.ItemId.RngSeed == 2 ? 2u : CreateUniqueSeed());
            var newCounter = source.ItemInfo.ItemId.Id == 0 ? (ushort)0 : GetNextUniqueCounter();

            var newItemId = new gameItemID
            {
                Id = source.ItemInfo.ItemId.Id,
                RngSeed = newSeed,
                UniqueCounter = newCounter,
                Flags = source.ItemInfo.ItemId.Flags
            };

            var clonedPart = new ItemSlotPart
            {
                ItemInfo = new ItemInfo
                {
                    ItemId = newItemId,
                    ItemStructure = source.ItemInfo.ItemStructure
                },
                AppearanceName = source.AppearanceName,
                AttachmentSlotTdbId = source.AttachmentSlotTdbId,
                Unknown2 = source.Unknown2,
                ItemAdditionalInfo = CloneAdditionalInfo(source.ItemAdditionalInfo),
                Children = source.Children?.Select(CloneSlotPart).ToList() ?? new List<ItemSlotPart>()
            };

            if (source.ItemInfo.ItemId.Id != 0)
            {
                CloneStats(source.ItemInfo.ItemId, newItemId);
            }

            return clonedPart;
        }

        public void CloneStats(gameItemID sourceItemId, gameItemID targetItemId)
        {
            var sourceStats = GetStatsFromItemId(sourceItemId);
            if (sourceStats == null)
            {
                return;
            }

            var clonedStats = (gameSavedStatsData)sourceStats.DeepCopy();
            clonedStats.Seed = targetItemId.RngSeed;

            if (sourceStats.ModifiersBuffer?.Data is WolvenKit.RED4.Archive.Buffer.ModifiersBuffer srcModBuf)
            {
                clonedStats.ModifiersBuffer = new DataBuffer
                {
                    Data = new WolvenKit.RED4.Archive.Buffer.ModifiersBuffer
                    {
                        Entries = srcModBuf.Entries.Select(x => (gameStatModifierData_Deprecated)x.DeepCopy()).ToList()
                    }
                };
            }

            if (sourceStats.ForcedModifiersBuffer?.Data is WolvenKit.RED4.Archive.Buffer.ModifiersBuffer srcForcedBuf)
            {
                clonedStats.ForcedModifiersBuffer = new DataBuffer
                {
                    Data = new WolvenKit.RED4.Archive.Buffer.ModifiersBuffer
                    {
                        Entries = srcForcedBuf.Entries.Select(x => (gameStatModifierData_Deprecated)x.DeepCopy()).ToList()
                    }
                };
            }

            if (sourceStats.SavedModifierGroupStatTypesBuffer?.Data is WolvenKit.RED4.Archive.Buffer.SavedModifierGroupStatTypesBuffer srcGroupBuf)
            {
                clonedStats.SavedModifierGroupStatTypesBuffer = new DataBuffer
                {
                    Data = new WolvenKit.RED4.Archive.Buffer.SavedModifierGroupStatTypesBuffer
                    {
                        Entries = srcGroupBuf.Entries.Select(x => new WolvenKit.RED4.Archive.Buffer.SavedModifierGroupStatTypesBuffer_Entry
                        {
                            ModifierGroupHash = x.ModifierGroupHash,
                            StatTypes = new List<gamedataStatType>(x.StatTypes)
                        }).ToList()
                    }
                };
            }

            AddStatData(new gameStatsObjectID
            {
                IdType = gameStatIDType.ItemID,
                EntityHash = InventoryHelper.GetItemIdHash(targetItemId)
            }, clonedStats);
        }

        /// <summary>
        /// Returns the save as it would be written to sav.dat.
        /// </summary>
        public byte[] Serialize()
        {
            FlushStats();

            using var ms = new MemoryStream();
            using (var writer = new CyberpunkSaveWriter(ms, Encoding.UTF8, true))
            {
                writer.WriteFile(SaveFile, true);
            }

            return ms.ToArray();
        }

        /// <summary>
        /// Writes pending stat changes into the save structure. Must be called before serializing the save.
        /// </summary>
        public void FlushStats()
        {
            OptimizedStats?.Flush();
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

        /// <summary>
        /// Makes an item Tier 5++. The game caps Quality at 4 and IsItemPlus at 2, so raising their additive modifiers
        /// far enough reaches the cap whatever the item's base quality (which comes from its TweakDB record) is.
        /// Returns false if the item already had enough of both, i.e. nothing changed.
        /// </summary>
        public bool SetMaxTier(gameSavedStatsData statsData)
        {
            var qualityChanged = RaiseConstantStat(gamedataStatType.Quality, 4, statsData);
            var plusChanged = RaiseConstantStat(gamedataStatType.IsItemPlus, 2, statsData);
            return qualityChanged || plusChanged;
        }

        // Adds an additive modifier so the stat's constant additive modifiers sum up to at least minTotal
        private bool RaiseConstantStat(gamedataStatType stat, float minTotal, gameSavedStatsData statsData)
        {
            var total = (statsData.ModifiersBuffer.Data as ModifiersBuffer)?.Entries
                .OfType<gameConstantStatModifierData_Deprecated>()
                .Where(x => x.StatType == stat && x.ModifierType == gameStatModifierType.Additive)
                .Sum(x => (float)x.Value) ?? 0;

            if (total >= minTotal)
            {
                return false;
            }

            AddStat(typeof(gameConstantStatModifierData_Deprecated), statsData, new gameConstantStatModifierData_Deprecated
            {
                ModifierType = gameStatModifierType.Additive,
                StatType = stat,
                Value = minTotal - total
            });

            return true;
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

        public uint CreateUniqueSeed(gameStatsStateMapStructure statsMap = null)
        {
            statsMap ??= GetStatsContainer();

            var rand = new Random();

            do
            {
                var seed = (uint)(rand.Next(1 << 30)) << 2 | (uint)(rand.Next(1 << 2));
                if (statsMap.Values.FirstOrDefault(x => x.Seed == seed) == null && !(OptimizedStats?.ContainsSeed(seed) ?? false))
                {
                    return seed;
                }
            } while (true);
        }

        public gameSavedStatsData CreateStatDataFromEntityId(ulong id)
        {
            var statsMap = GetStatsContainer();

            var inactiveStats = new CArray<CEnum<gamedataStatType>>
            {
                gamedataStatType.Level,
                gamedataStatType.QuickHackUpload
            };

            var modifierBuffer = new ModifiersBuffer();
            modifierBuffer.Entries.Add(new gameConstantStatModifierData_Deprecated
            {
                StatType = gamedataStatType.Humanity,
                ModifierType = gameStatModifierType.Additive,
                Value = 2
            });

            var statsData = new gameSavedStatsData
            {
                RecordID = "Character.Player_Puppet_Base",
                Seed = CreateUniqueSeed(statsMap)
            };

            AddStatData(new gameStatsObjectID
            {
                IdType = gameStatIDType.EntityID,
                EntityHash = id
            }, statsData);

            return statsData;
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
                gameItemId.UniqueCounter = GetNextUniqueCounter();
            }

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
                if (itemRecord.Type == "Weapon" || itemRecord.Type == "WeaponItem")
                {
                    statsData.InactiveStats.Add(Enums.gamedataStatType.IsItemPlus);

                    ((ModifiersBuffer)statsData.ModifiersBuffer.Data).Entries.Add(new gameConstantStatModifierData_Deprecated
                    {
                        StatType = gamedataStatType.IsItemPlus,
                        ModifierType = gameStatModifierType.Additive,
                        Value = itemRecord.IsItemPlus
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

            AddStatData(new gameStatsObjectID
            {
                IdType = gameStatIDType.ItemID,
                EntityHash = InventoryHelper.GetItemIdHash(gameItemId)
            }, statsData);

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
