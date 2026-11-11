using System.Globalization;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Save;
using WolvenKit.RED4.Save.Classes;
using WolvenKit.RED4.Types;
using WolvenKit.Common.Services;
using static WolvenKit.RED4.Types.Enums;

namespace CP2077SaveExporter;

/// <summary>
/// Read-only projection from <see cref="CyberpunkSaveFile"/> to export DTOs. No mutation of save data.
/// </summary>
public static class ProgressExtractor
{
    private const ulong PlayerOwnerHash = 1;
    private const ulong PlayerInventoryId = 1;

    private static readonly HashSet<gamedataEquipmentArea> CyberwareEquipmentAreas = new()
    {
        gamedataEquipmentArea.AbilityCW,
        gamedataEquipmentArea.ArmsCW,
        gamedataEquipmentArea.CardiovascularSystemCW,
        gamedataEquipmentArea.CyberwareWheel,
        gamedataEquipmentArea.EyesCW,
        gamedataEquipmentArea.FaceCW,
        gamedataEquipmentArea.FrontalCortexCW,
        gamedataEquipmentArea.HandsCW,
        gamedataEquipmentArea.ImmuneSystemCW,
        gamedataEquipmentArea.IntegumentarySystemCW,
        gamedataEquipmentArea.LegsCW,
        gamedataEquipmentArea.MusculoskeletalSystemCW,
        gamedataEquipmentArea.NervousSystemCW,
        gamedataEquipmentArea.PersonalLink,
        gamedataEquipmentArea.SilverhandArm,
        gamedataEquipmentArea.Splinter,
        gamedataEquipmentArea.SystemReplacementCW,
    };

    public static ExportSnapshot Build(
        CyberpunkSaveFile save,
        HashService hashService,
        IReadOnlyDictionary<uint, string>? factNameCatalog = null)
    {
        _ = hashService;

        var header = save.FileHeader;
        var headerDto = new SaveHeaderDto
        {
            SaveVersion = header.SaveVersion,
            GameVersion = header.GameVersion,
            GameDefPath = header.GameDefPath ?? "",
            TimeStamp = header.TimeStamp,
            ArchiveVersion = header.ArchiveVersion,
        };

        var playerDev = TryGetPlayerDevelopmentData(save);
        var level = FindProficiencyLevel(playerDev, gamedataProficiencyType.Level);
        var streetCred = FindProficiencyLevel(playerDev, gamedataProficiencyType.StreetCred);

        var moneyInv = TryGetInventoryMoneyQuantity(save);

        var attrsFlat = ExtractAttributesFlat(playerDev);
        var invSummary = SummarizeInventory(save);
        var invRows = CollectInventoryItemRows(save);

        var equipAreas = TryGetPlayerEquipAreas(save);
        var equippedRaw = CollectEquippedRaw(equipAreas);
        var cyberRaw = CollectCyberwareRaw(equipAreas);

        var factsAll = CollectQuestFactsRaw(save, factNameCatalog);
        var ftRaw = CollectFastTravelRaw(save);

        var raw = new RawSectionDto
        {
            Level = level,
            StreetCred = streetCred,
            Money = new MoneyDto
            {
                InventoryItemsMoneyQuantity = moneyInv,
                StatPoolsSystemCurrency = null,
            },
            AttributesFlat = attrsFlat,
            Inventory = new InventoryRawDto
            {
                Summary = invSummary,
                ItemRows = invRows,
            },
            Equipment = new EquipmentRawDto
            {
                AllSlots = equippedRaw,
                CyberwareSlots = cyberRaw,
            },
            QuestFactsAll = factsAll,
            FastTravelPoints = ftRaw,
        };

        var core = ExportTransforms.SplitCoreAttributes(attrsFlat);
        var otherStats = ExportTransforms.SplitOtherStats(attrsFlat);

        var equippedIdSet = ExportTransforms.CollectEquippedTweakDbIds(equippedRaw, cyberRaw);
        var cyberwareIdSet = ExportTransforms.CollectCyberwareTweakDbIds(cyberRaw);
        var slotsDeduped = ExportTransforms.DedupeEquippedSlots(equippedRaw);
        var cyberDeduped = ExportTransforms.DedupeCyberwareSlots(cyberRaw);
        var invNormalizedRows = ExportTransforms.NormalizeInventoryRows(invRows, equippedIdSet, cyberwareIdSet);
        var slotInstances = ExportTransforms.ToEquippedSlotInstances(equippedRaw);
        var cyberInstances = ExportTransforms.ToCyberwareSlotInstances(cyberRaw);

        var normalized = new NormalizedSectionDto
        {
            CoreAttributes = core,
            OtherStats = otherStats,
            Inventory = new InventoryNormalizedDto
            {
                Summary = invSummary,
                ItemRows = invNormalizedRows,
            },
            Equipment = new EquipmentNormalizedDto
            {
                SlotsDeduped = slotsDeduped,
                CyberwareDeduped = cyberDeduped,
                SlotInstances = slotInstances,
                CyberwareInstances = cyberInstances,
            },
            QuestFactsNamedNonZero = ExportTransforms.FilterNamedNonZero(factsAll),
        };

        var questSignals = ExportTransforms.SummarizeQuestSignals(factsAll);
        var buildTags = ExportTransforms.BuildTags(level, streetCred, core, cyberDeduped.Count);
        var inventoryInsights = ExportTransforms.SummarizeInventoryInsights(invNormalizedRows, level, moneyInv);
        var derived = new DerivedSectionDto
        {
            BuildTags = buildTags,
            ProgressionStage = ExportTransforms.ProgressionStageFromLevel(level),
            QuestFactSummary = ExportTransforms.SummarizeFacts(factsAll),
            QuestSignals = questSignals,
            InventorySummary = ExportTransforms.SummarizeInventoryCategories(invNormalizedRows),
            EquipmentSummary = ExportTransforms.SummarizeEquipmentLoadout(slotsDeduped, cyberInstances),
            Completion = ExportTransforms.SummarizeCompletionMetrics(factsAll),
            Expansion = ExportTransforms.SummarizeExpansion(factsAll, ftRaw, questSignals),
            InventoryInsights = inventoryInsights,
            EquipmentMaturity = ExportTransforms.SummarizeEquipmentMaturity(
                level,
                moneyInv,
                invNormalizedRows,
                slotsDeduped,
                cyberDeduped,
                cyberInstances.Count),
            BuildProfile = ExportTransforms.SummarizeBuildProfile(core, buildTags, cyberInstances, inventoryInsights),
            Exploration = ExportTransforms.SummarizeExploration(ftRaw),
        };

        return new ExportSnapshot
        {
            Header = headerDto,
            Raw = raw,
            Normalized = normalized,
            Derived = derived,
        };
    }

    private static RedPackage? TryGetScriptablePackage(CyberpunkSaveFile save)
    {
        var node = save.Nodes.FirstOrDefault(n => n.Name == Constants.NodeNames.SCRIPTABLE_SYSTEMS_CONTAINER);
        if (node?.Value is not Package { Content: RedPackage pkg })
        {
            return null;
        }

        return pkg;
    }

    private static PlayerDevelopmentData? TryGetPlayerDevelopmentData(CyberpunkSaveFile save)
    {
        var pkg = TryGetScriptablePackage(save);
        if (pkg == null)
        {
            return null;
        }

        var sys = pkg.Chunks.OfType<PlayerDevelopmentSystem>().FirstOrDefault();
        if (sys == null)
        {
            return null;
        }

        foreach (var h in sys.PlayerData)
        {
            if (h.Chunk != null && (ulong)h.Chunk.OwnerID.Hash == PlayerOwnerHash)
            {
                return h.Chunk;
            }
        }

        return null;
    }

    private static int? FindProficiencyLevel(PlayerDevelopmentData? pd, gamedataProficiencyType kind)
    {
        if (pd == null)
        {
            return null;
        }

        foreach (var p in pd.Proficiencies)
        {
            if ((gamedataProficiencyType)p.Type == kind)
            {
                return (int)p.CurrentLevel;
            }
        }

        return null;
    }

    private static ulong? TryGetInventoryMoneyQuantity(CyberpunkSaveFile save)
    {
        var node = save.Nodes.FirstOrDefault(n => n.Name == Constants.NodeNames.INVENTORY);
        if (node?.Value is not Inventory inv)
        {
            return null;
        }

        foreach (var sub in inv.SubInventories)
        {
            if (sub.InventoryId != PlayerInventoryId)
            {
                continue;
            }

            foreach (var item in sub.Items)
            {
                var idText = item.ItemInfo.ItemId.Id.GetResolvedText();
                if (idText == "Items.money")
                {
                    return item.Quantity;
                }
            }
        }

        return null;
    }

    private static IReadOnlyList<AttributeFlatDto> ExtractAttributesFlat(PlayerDevelopmentData? pd)
    {
        if (pd == null)
        {
            return Array.Empty<AttributeFlatDto>();
        }

        var list = new List<AttributeFlatDto>();
        foreach (var a in pd.Attributes)
        {
            var name = ((gamedataStatType)a.AttributeName).ToString();
            list.Add(new AttributeFlatDto { StatType = name, Value = (int)a.Value });
        }

        return list;
    }

    private static InventorySummaryDto SummarizeInventory(CyberpunkSaveFile save)
    {
        var node = save.Nodes.FirstOrDefault(n => n.Name == Constants.NodeNames.INVENTORY);
        if (node?.Value is not Inventory inv)
        {
            return new InventorySummaryDto();
        }

        var subs = new List<SubInventorySummaryDto>();
        var total = 0;
        foreach (var sub in inv.SubInventories)
        {
            var c = sub.Items.Count;
            total += c;
            subs.Add(new SubInventorySummaryDto
            {
                InventoryId = sub.InventoryId.ToString(CultureInfo.InvariantCulture),
                ItemCount = c,
            });
        }

        return new InventorySummaryDto
        {
            SubInventoryCount = inv.SubInventories.Count,
            TotalItemStacks = total,
            SubInventories = subs,
        };
    }

    private static IReadOnlyList<InventoryItemRawDto> CollectInventoryItemRows(CyberpunkSaveFile save)
    {
        var node = save.Nodes.FirstOrDefault(n => n.Name == Constants.NodeNames.INVENTORY);
        if (node?.Value is not Inventory inv)
        {
            return Array.Empty<InventoryItemRawDto>();
        }

        var rows = new List<InventoryItemRawDto>();
        foreach (var sub in inv.SubInventories)
        {
            var invId = sub.InventoryId.ToString(CultureInfo.InvariantCulture);
            foreach (var item in sub.Items)
            {
                var tid = item.ItemInfo.ItemId.Id;
                rows.Add(new InventoryItemRawDto
                {
                    InventoryId = invId,
                    TweakDbIdUlong = ((ulong)tid).ToString(CultureInfo.InvariantCulture),
                    ItemIdResolved = tid.GetResolvedText(),
                    ItemIdPresentation = tid.ToString(),
                    Quantity = item.Quantity,
                    Flags = item.Flags.ToString(),
                    CreationTime = item.CreationTime,
                    ItemStructure = item.ItemInfo.ItemStructure.ToString(),
                });
            }
        }

        return rows;
    }

    private static CArray<gameSEquipArea>? TryGetPlayerEquipAreas(CyberpunkSaveFile save)
    {
        var pkg = TryGetScriptablePackage(save);
        if (pkg == null)
        {
            return null;
        }

        var equip = pkg.Chunks.OfType<EquipmentSystem>().FirstOrDefault();
        if (equip == null)
        {
            return null;
        }

        foreach (var h in equip.OwnerData)
        {
            if (h.Chunk != null && (ulong)h.Chunk.OwnerID.Hash == PlayerOwnerHash)
            {
                return h.Chunk.Equipment.EquipAreas;
            }
        }

        return null;
    }

    private static IReadOnlyList<EquippedSlotRawDto> CollectEquippedRaw(CArray<gameSEquipArea>? areas)
    {
        if (areas == null)
        {
            return Array.Empty<EquippedSlotRawDto>();
        }

        var list = new List<EquippedSlotRawDto>();
        var weaponSlot = 1;
        foreach (var area in areas)
        {
            var gt = (gamedataEquipmentArea)area.AreaType;
            var areaLabel = gt.ToString();
            if (gt == gamedataEquipmentArea.Weapon)
            {
                areaLabel = "Weapon " + weaponSlot;
                weaponSlot++;
            }

            if (area.EquipSlots == null)
            {
                continue;
            }

            foreach (var slot in area.EquipSlots)
            {
                if (slot.ItemID == null || (ulong)slot.ItemID.Id == 0)
                {
                    continue;
                }

                var id = slot.ItemID.Id;
                list.Add(new EquippedSlotRawDto
                {
                    AreaTypeLabel = areaLabel,
                    AreaTypeEnum = gt.ToString(),
                    TweakDbIdUlong = ((ulong)id).ToString(CultureInfo.InvariantCulture),
                    ItemIdResolved = id.GetResolvedText(),
                    ItemIdPresentation = id.ToString(),
                });
            }
        }

        return list;
    }

    private static IReadOnlyList<CyberwareSlotRawDto> CollectCyberwareRaw(CArray<gameSEquipArea>? areas)
    {
        if (areas == null)
        {
            return Array.Empty<CyberwareSlotRawDto>();
        }

        var list = new List<CyberwareSlotRawDto>();
        foreach (var area in areas)
        {
            var gt = (gamedataEquipmentArea)area.AreaType;
            if (!CyberwareEquipmentAreas.Contains(gt))
            {
                continue;
            }

            if (area.EquipSlots == null)
            {
                continue;
            }

            foreach (var slot in area.EquipSlots)
            {
                if (slot.ItemID == null || (ulong)slot.ItemID.Id == 0)
                {
                    continue;
                }

                var id = slot.ItemID.Id;
                list.Add(new CyberwareSlotRawDto
                {
                    AreaTypeEnum = gt.ToString(),
                    TweakDbIdUlong = ((ulong)id).ToString(CultureInfo.InvariantCulture),
                    ItemIdResolved = id.GetResolvedText(),
                    ItemIdPresentation = id.ToString(),
                });
            }
        }

        return list;
    }

    private static IReadOnlyList<QuestFactRawDto> CollectQuestFactsRaw(
        CyberpunkSaveFile save,
        IReadOnlyDictionary<uint, string>? factNameCatalog)
    {
        var list = new List<QuestFactRawDto>();
        var qs = save.Nodes.FirstOrDefault(n => n.Name == Constants.NodeNames.QUEST_SYSTEM);
        if (qs == null)
        {
            return list;
        }

        foreach (var child in qs.Children)
        {
            if (child.Name != Constants.NodeNames.FACTSDB)
            {
                continue;
            }

            if (child.Value is not FactsDB db)
            {
                continue;
            }

            foreach (var table in db.FactsTables)
            {
                foreach (var fe in table.FactEntries)
                {
                    var hash = (uint)fe.FactName;
                    string? name = null;
                    if (factNameCatalog != null && factNameCatalog.TryGetValue(hash, out var n))
                    {
                        name = n;
                    }

                    list.Add(new QuestFactRawDto
                    {
                        FactHash = hash,
                        FactHashHex = "0x" + hash.ToString("X8", CultureInfo.InvariantCulture),
                        Name = name,
                        Value = fe.Value,
                    });
                }
            }
        }

        return list;
    }

    private static IReadOnlyList<FastTravelPointRawDto> CollectFastTravelRaw(CyberpunkSaveFile save)
    {
        var pkg = TryGetScriptablePackage(save);
        if (pkg == null)
        {
            return Array.Empty<FastTravelPointRawDto>();
        }

        var fts = pkg.Chunks.OfType<FastTravelSystem>().FirstOrDefault();
        if (fts == null)
        {
            return Array.Empty<FastTravelPointRawDto>();
        }

        var list = new List<FastTravelPointRawDto>();
        foreach (var h in fts.FastTravelNodes)
        {
            if (h.Chunk == null)
            {
                continue;
            }

            var d = h.Chunk;
            var pr = d.PointRecord;
            var mr = d.MarkerRef;
            list.Add(new FastTravelPointRawDto
            {
                PointRecordResolved = pr.GetResolvedText(),
                PointRecordTweakDbUlong = ((ulong)pr).ToString(CultureInfo.InvariantCulture),
                MarkerRefResolved = mr.GetResolvedText(),
                MarkerRefUlong = ((ulong)mr).ToString(CultureInfo.InvariantCulture),
                IsEp1 = (bool)d.IsEP1,
            });
        }

        return list;
    }
}
