namespace CP2077SaveExporter;

/// <summary>Explicit read-only transforms from raw extraction to normalized/derived DTOs.</summary>
internal static class ExportTransforms
{
    /// <summary>Count of body cyberware equipment areas used by the exporter (matches ProgressExtractor cyberware filter).</summary>
    public const int CyberwareSlotsPossible = 19;

    private static readonly HashSet<string> CoreStatNames = new(StringComparer.Ordinal)
    {
        "Strength",
        "Reflexes",
        "Intelligence",
        "TechnicalAbility",
        "Cool",
    };

    public static CoreAttributesDto SplitCoreAttributes(IReadOnlyList<AttributeFlatDto> flat)
    {
        int? body = null, reflexes = null, intelligence = null, technical = null, cool = null;
        foreach (var a in flat)
        {
            switch (a.StatType)
            {
                case "Strength": body = a.Value; break;
                case "Reflexes": reflexes = a.Value; break;
                case "Intelligence": intelligence = a.Value; break;
                case "TechnicalAbility": technical = a.Value; break;
                case "Cool": cool = a.Value; break;
            }
        }

        return new CoreAttributesDto
        {
            Body = body,
            Reflexes = reflexes,
            Intelligence = intelligence,
            Technical = technical,
            Cool = cool,
        };
    }

    public static Dictionary<string, int> SplitOtherStats(IReadOnlyList<AttributeFlatDto> flat)
    {
        var d = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var a in flat)
        {
            if (!CoreStatNames.Contains(a.StatType))
            {
                d[a.StatType] = a.Value;
            }
        }

        return d;
    }

    public static HashSet<string> CollectEquippedTweakDbIds(
        IReadOnlyList<EquippedSlotRawDto> equipment,
        IReadOnlyList<CyberwareSlotRawDto> cyberware)
    {
        var s = new HashSet<string>(StringComparer.Ordinal);
        foreach (var e in equipment)
        {
            s.Add(e.TweakDbIdUlong);
        }

        foreach (var c in cyberware)
        {
            s.Add(c.TweakDbIdUlong);
        }

        return s;
    }

    public static HashSet<string> CollectCyberwareTweakDbIds(IReadOnlyList<CyberwareSlotRawDto> cyberware)
    {
        var s = new HashSet<string>(StringComparer.Ordinal);
        foreach (var c in cyberware)
        {
            s.Add(c.TweakDbIdUlong);
        }

        return s;
    }

    public static IReadOnlyList<InventoryItemNormalizedDto> NormalizeInventoryRows(
        IReadOnlyList<InventoryItemRawDto> raw,
        IReadOnlySet<string> equippedTweakDbIds,
        IReadOnlySet<string> cyberwareTweakDbIds)
    {
        var list = new List<InventoryItemNormalizedDto>(raw.Count);
        foreach (var row in raw)
        {
            var equipped = equippedTweakDbIds.Contains(row.TweakDbIdUlong);
            var category = InferItemCategory(row.ItemIdResolved, row.ItemIdPresentation, row.Flags);
            if (cyberwareTweakDbIds.Contains(row.TweakDbIdUlong))
            {
                category = "Cyberware";
            }

            list.Add(new InventoryItemNormalizedDto
            {
                InventoryId = row.InventoryId,
                TweakDbIdUlong = row.TweakDbIdUlong,
                ItemIdResolved = row.ItemIdResolved,
                ItemIdPresentation = row.ItemIdPresentation,
                Quantity = row.Quantity,
                InferredCategory = category,
                AppearsEquipped = equipped,
                AppearsEquippedReason = equipped ? "tweakdbid_match" : "not_matched",
            });
        }

        return list;
    }

    public static IReadOnlyList<EquippedSlotInstanceDto> ToEquippedSlotInstances(IReadOnlyList<EquippedSlotRawDto> raw)
    {
        return raw.Select(r => new EquippedSlotInstanceDto
        {
            AreaTypeLabel = r.AreaTypeLabel,
            AreaTypeEnum = r.AreaTypeEnum,
            TweakDbIdUlong = r.TweakDbIdUlong,
            ItemIdResolved = r.ItemIdResolved,
            ItemIdPresentation = r.ItemIdPresentation,
        }).ToList();
    }

    public static IReadOnlyList<CyberwareSlotInstanceDto> ToCyberwareSlotInstances(IReadOnlyList<CyberwareSlotRawDto> raw)
    {
        return raw.Select(r => new CyberwareSlotInstanceDto
        {
            AreaTypeEnum = r.AreaTypeEnum,
            TweakDbIdUlong = r.TweakDbIdUlong,
            ItemIdResolved = r.ItemIdResolved,
            ItemIdPresentation = r.ItemIdPresentation,
        }).ToList();
    }

    public static DerivedInventorySummaryDto SummarizeInventoryCategories(IReadOnlyList<InventoryItemNormalizedDto> rows)
    {
        var w = 0;
        var c = 0;
        var cons = 0;
        var cloth = 0;
        var qh = 0;
        var q = 0;
        var ammo = 0;
        var distinctTweakIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            distinctTweakIds.Add(row.TweakDbIdUlong);
            switch (row.InferredCategory)
            {
                case "Weapon":
                case "WeaponMod":
                    w++;
                    break;
                case "Cyberware":
                    c++;
                    break;
                case "Consumable":
                    cons++;
                    break;
                case "Clothing":
                    cloth++;
                    break;
                case "Quickhack":
                    qh++;
                    break;
                case "Quest":
                    q++;
                    break;
                case "Ammo":
                    ammo++;
                    break;
            }
        }

        return new DerivedInventorySummaryDto
        {
            WeaponCount = w,
            CyberwareCount = c,
            ConsumableCount = cons,
            ClothingCount = cloth,
            QuickhackCount = qh,
            QuestItemCount = q,
            AmmoCount = ammo,
            UniqueItemTypes = distinctTweakIds.Count,
        };
    }

    private static readonly HashSet<string> WeaponSlotAreaEnums = new(StringComparer.Ordinal)
    {
        "Weapon",
        "WeaponLeft",
        "WeaponHeavy",
        "VDefaultHandgun",
        "WeaponWheel",
    };

    public static DerivedEquipmentSummaryDto SummarizeEquipmentLoadout(
        IReadOnlyList<EquippedSlotNormalizedDto> slotsDeduped,
        IReadOnlyList<CyberwareSlotInstanceDto> cyberInstances)
    {
        var weapons = 0;
        foreach (var s in slotsDeduped)
        {
            if (WeaponSlotAreaEnums.Contains(s.AreaTypeEnum))
            {
                weapons++;
            }
        }

        return new DerivedEquipmentSummaryDto
        {
            TotalEquippedWeapons = weapons,
            TotalEquippedCyberware = cyberInstances.Count,
        };
    }

    /// <summary>Specific categories before broad Weapon match; quest flag from save item flags first.</summary>
    public static string InferItemCategory(string? resolved, string presentation, string flags)
    {
        var id = (resolved ?? presentation).ToLowerInvariant();
        if (id.Length == 0)
        {
            return "Unknown";
        }

        if (flags.Contains("IsQuestItem", StringComparison.Ordinal))
        {
            return "Quest";
        }

        if (id.Contains("items.money", StringComparison.Ordinal)
            || id.Contains("gen_moneyshard", StringComparison.Ordinal)
            || id.Contains("money_shard", StringComparison.Ordinal))
        {
            return "Currency";
        }

        // Before broad items.q / "quest" paths so keycards are labeled even under Items.Q*_Keycard.
        if (id.Contains("keycard", StringComparison.Ordinal) || id.Contains("gen_keycard", StringComparison.Ordinal))
        {
            return "Keycard";
        }

        if (id.Contains("quest", StringComparison.Ordinal) || id.StartsWith("items.q", StringComparison.Ordinal))
        {
            return "Quest";
        }

        if ((id.Contains("mod", StringComparison.Ordinal) && id.Contains("weapon", StringComparison.Ordinal))
            || id.Contains("w_mod_", StringComparison.Ordinal)
            || (id.Contains("prt_", StringComparison.Ordinal) && !id.Contains("fabric", StringComparison.Ordinal)))
        {
            return "WeaponMod";
        }

        if (id.Contains("ammo.", StringComparison.Ordinal) || id.Contains("con_ammo", StringComparison.Ordinal))
        {
            return "Ammo";
        }

        if (id.Contains("quickhack", StringComparison.Ordinal) || id.Contains("quick_hack", StringComparison.Ordinal))
        {
            return "Quickhack";
        }

        if (id.Contains("grenade", StringComparison.Ordinal))
        {
            return "Grenade";
        }

        if (id.Contains("cyberwarestatsshard", StringComparison.Ordinal)
            || id.Contains("cyberwareupgradeshard", StringComparison.Ordinal))
        {
            return "Cyberware";
        }

        if (id.Contains("cyberware", StringComparison.Ordinal) || id.Contains("_cw_", StringComparison.Ordinal))
        {
            return "Cyberware";
        }

        if (id.Contains("gen_craftingmaterial", StringComparison.Ordinal)
            || id.Contains("craftingmaterial", StringComparison.Ordinal)
            || id.Contains("items.crafting", StringComparison.Ordinal)
            || id.Contains("upgrade_component", StringComparison.Ordinal))
        {
            return "Crafting";
        }

        if (id.Contains("gen_readable", StringComparison.Ordinal)
            || id.Contains("gen_databank", StringComparison.Ordinal)
            || id.Contains("lore_shard", StringComparison.Ordinal))
        {
            return "Readable";
        }

        if (id.Contains("consumable", StringComparison.Ordinal)
            || id.Contains("food", StringComparison.Ordinal)
            || id.Contains("drink", StringComparison.Ordinal)
            || id.Contains("alcohol", StringComparison.Ordinal)
            || (id.Contains("items.con_", StringComparison.Ordinal) && !id.Contains("con_ammo", StringComparison.Ordinal)))
        {
            return "Consumable";
        }

        if (id.Contains("shirt") || id.Contains("pants") || id.Contains("outer") || id.Contains("face") || id.Contains("boots") || id.Contains("clothing") || id.Contains("outfit"))
        {
            return "Clothing";
        }

        if (id.Contains("junk", StringComparison.Ordinal))
        {
            return "Junk";
        }

        if (id.Contains("w_melee_", StringComparison.Ordinal))
        {
            return "Weapon";
        }

        if (id.Contains("weapon", StringComparison.Ordinal) || id.Contains("rifle") || id.Contains("pistol") || id.Contains("blade") || id.Contains("sword") || id.Contains("shotgun") || id.Contains("smg"))
        {
            return "Weapon";
        }

        return "Unknown";
    }

    /// <summary>
    /// Merge rows that share the same TweakDB id across different equipment views (e.g. Weapon vs WeaponWheel).
    /// Canonical row = lowest priority number; raw list unchanged.
    /// </summary>
    public static IReadOnlyList<EquippedSlotNormalizedDto> DedupeEquippedSlots(IReadOnlyList<EquippedSlotRawDto> raw)
    {
        return raw
            .GroupBy(r => r.TweakDbIdUlong, StringComparer.Ordinal)
            .Select(g =>
            {
                var ordered = g.OrderBy(r => GetCanonicalLoadoutPriority(r.AreaTypeEnum)).ToList();
                var best = ordered[0];
                var suppressed = ordered.Skip(1).Select(r => r.AreaTypeEnum).Distinct(StringComparer.Ordinal).ToList();
                return new EquippedSlotNormalizedDto
                {
                    AreaTypeLabel = best.AreaTypeLabel,
                    AreaTypeEnum = best.AreaTypeEnum,
                    TweakDbIdUlong = best.TweakDbIdUlong,
                    ItemIdResolved = best.ItemIdResolved,
                    DuplicateCount = g.Count(),
                    SuppressedAreaTypeEnums = suppressed,
                };
            })
            .ToList();
    }

    public static IReadOnlyList<CyberwareSlotNormalizedDto> DedupeCyberwareSlots(IReadOnlyList<CyberwareSlotRawDto> raw)
    {
        return raw
            .GroupBy(r => r.TweakDbIdUlong, StringComparer.Ordinal)
            .Select(g =>
            {
                var ordered = g.OrderBy(r => GetCyberwareCanonicalPriority(r.AreaTypeEnum)).ToList();
                var best = ordered[0];
                var suppressed = ordered.Skip(1).Select(r => r.AreaTypeEnum).Distinct(StringComparer.Ordinal).ToList();
                return new CyberwareSlotNormalizedDto
                {
                    AreaTypeEnum = best.AreaTypeEnum,
                    TweakDbIdUlong = best.TweakDbIdUlong,
                    ItemIdResolved = best.ItemIdResolved,
                    DuplicateCount = g.Count(),
                    SuppressedAreaTypeEnums = suppressed,
                };
            })
            .ToList();
    }

    /// <summary>Lower = preferred primary slot when the same id appears under multiple area enums.</summary>
    private static int GetCanonicalLoadoutPriority(string areaEnum)
    {
        return areaEnum switch
        {
            "Weapon" => 10,
            "WeaponLeft" => 11,
            "WeaponHeavy" => 12,
            "VDefaultHandgun" => 13,
            "LeftArm" => 20,
            "RightArm" => 21,
            "HandsCW" => 30,
            "OuterChest" => 40,
            "InnerChest" => 41,
            "Legs" => 42,
            "Feet" => 43,
            "Head" => 44,
            "Face" => 45,
            "QuickSlot" => 100,
            "QuickWheel" => 110,
            "WeaponWheel" => 200,
            "Gadget" => 120,
            "Outfit" => 130,
            _ => 150,
        };
    }

    private static int GetCyberwareCanonicalPriority(string areaEnum)
    {
        return areaEnum switch
        {
            "FrontalCortexCW" => 0,
            "NervousSystemCW" => 2,
            "CardiovascularSystemCW" => 4,
            "ImmuneSystemCW" => 6,
            "IntegumentarySystemCW" => 8,
            "MusculoskeletalSystemCW" => 10,
            "EyesCW" => 20,
            "HandsCW" => 30,
            "ArmsCW" => 32,
            "LegsCW" => 34,
            "AbilityCW" => 40,
            "SystemReplacementCW" => 50,
            "CyberwareWheel" => 200,
            "PersonalLink" => 60,
            "Splinter" => 70,
            "SilverhandArm" => 80,
            _ => 100,
        };
    }

    public static IReadOnlyList<QuestFactRawDto> FilterNamedNonZero(IReadOnlyList<QuestFactRawDto> all) =>
        all.Where(f => f.Name != null && f.Value != 0).ToList();

    public static QuestFactSummaryDto SummarizeFacts(IReadOnlyList<QuestFactRawDto> all)
    {
        var withName = 0;
        var nz = 0;
        var z = 0;
        foreach (var f in all)
        {
            if (f.Name != null)
            {
                withName++;
            }

            if (f.Value != 0)
            {
                nz++;
            }
            else
            {
                z++;
            }
        }

        var nnz = all.Count(f => f.Name != null && f.Value != 0);
        return new QuestFactSummaryDto
        {
            Total = all.Count,
            WithCatalogName = withName,
            NonZeroValue = nz,
            ZeroValue = z,
            NamedNonZero = nnz,
        };
    }

    public static QuestSignalSummaryDto SummarizeQuestSignals(IReadOnlyList<QuestFactRawDto> all)
    {
        var named = all.Where(f => f.Name != null).Select(f => f.Name!).ToList();
        var qPrefix = 0;
        var done = 0;
        var failed = 0;
        var ep1 = 0;
        foreach (var n in named)
        {
            var ln = n.ToLowerInvariant();
            if (ln.StartsWith("q", StringComparison.Ordinal) || ln.StartsWith("#q", StringComparison.Ordinal) || ln.StartsWith("sq_", StringComparison.Ordinal))
            {
                qPrefix++;
            }

            if (ln.Contains("done", StringComparison.Ordinal))
            {
                done++;
            }

            if (ln.Contains("fail", StringComparison.Ordinal))
            {
                failed++;
            }

            if (ln.Contains("ep1", StringComparison.Ordinal) || ln.Contains("phantom", StringComparison.Ordinal))
            {
                ep1++;
            }
        }

        var mainDone = 0;
        var sideDone = 0;
        var gigsDone = 0;
        foreach (var f in all)
        {
            if (f.Name == null)
            {
                continue;
            }

            var key = NormalizeQuestFactNameKey(f.Name);
            if (!QuestNameLooksCompleted(key))
            {
                continue;
            }

            if (key.StartsWith("mq", StringComparison.Ordinal))
            {
                mainDone++;
            }
            else if (key.StartsWith("sq", StringComparison.Ordinal))
            {
                sideDone++;
            }
            else if (key.StartsWith("ma_", StringComparison.Ordinal) || key.StartsWith("sts_", StringComparison.Ordinal))
            {
                gigsDone++;
            }
        }

        return new QuestSignalSummaryDto
        {
            NamedFactsMatchingQuestPrefix = qPrefix,
            NamedFactsContainingDone = done,
            NamedFactsContainingFailed = failed,
            NamedFactsEp1Hint = ep1,
            MainQuestCompleted = mainDone,
            SideQuestCompleted = sideDone,
            GigsCompleted = gigsDone,
        };
    }

    private static string NormalizeQuestFactNameKey(string name)
    {
        var s = name.Trim();
        if (s.Length > 0 && s[0] == '#')
        {
            s = s.Substring(1);
        }

        return s.Trim().ToLowerInvariant();
    }

    private static bool QuestNameLooksCompleted(string normalizedLower)
    {
        return normalizedLower.Contains("_done", StringComparison.Ordinal)
            || normalizedLower.Contains("_finished", StringComparison.Ordinal);
    }

    public static IReadOnlyList<string> BuildTags(int? level, int? streetCred, CoreAttributesDto core, int dedupedCyberwareSlotCount)
    {
        var tags = new List<string>();
        var lv = level ?? 0;
        var sc = streetCred ?? 0;
        if (lv >= 40)
        {
            tags.Add("level_cap_focus");
        }
        else if (lv >= 25)
        {
            tags.Add("mid_level");
        }

        if (sc >= 50)
        {
            tags.Add("high_street_cred");
        }

        if ((core.Intelligence ?? 0) >= 12)
        {
            tags.Add("int_focus");
        }

        if ((core.Cool ?? 0) >= 12)
        {
            tags.Add("cool_focus");
        }

        if ((core.Body ?? 0) >= 12)
        {
            tags.Add("body_focus");
        }

        if ((core.Reflexes ?? 0) >= 12)
        {
            tags.Add("reflex_focus");
        }

        if ((core.Technical ?? 0) >= 12)
        {
            tags.Add("tech_focus");
        }

        if (dedupedCyberwareSlotCount >= 12)
        {
            tags.Add("heavy_cyberware");
        }

        return tags;
    }

    public static string? ProgressionStageFromLevel(int? level)
    {
        if (level == null)
        {
            return null;
        }

        var l = level.Value;
        if (l < 15)
        {
            return "early";
        }

        if (l < 30)
        {
            return "mid";
        }

        if (l < 45)
        {
            return "late";
        }

        return "endgame";
    }

    public static DerivedCompletionDto SummarizeCompletionMetrics(IReadOnlyList<QuestFactRawDto> all)
    {
        var mainC = 0;
        var mainP = 0;
        var sideC = 0;
        var sideP = 0;
        var gigC = 0;
        var gigP = 0;
        foreach (var f in all)
        {
            if (f.Name == null)
            {
                continue;
            }

            var key = NormalizeQuestFactNameKey(f.Name);
            var kind = CompletionQuestKind(key);
            if (kind == 0)
            {
                continue;
            }

            if (QuestNameLooksCompleted(key))
            {
                switch (kind)
                {
                    case 1: mainC++; break;
                    case 2: sideC++; break;
                    case 3: gigC++; break;
                }
            }
            else if (QuestNameLooksInProgress(key))
            {
                switch (kind)
                {
                    case 1: mainP++; break;
                    case 2: sideP++; break;
                    case 3: gigP++; break;
                }
            }
        }

        return new DerivedCompletionDto
        {
            MainQuest = new CompletionQuestBucketDto { Completed = mainC, InProgress = mainP },
            SideQuest = new CompletionQuestBucketDto { Completed = sideC, InProgress = sideP },
            Gigs = new CompletionQuestBucketDto { Completed = gigC, InProgress = gigP },
            InProgressInterpretation = "approximate",
            InProgressConfidence = "low",
            CompletionInterpretation = "counts_reflect_detected_flags_not_full_quest_set",
            CompletionConfidence = "low",
            HasActiveQuestSignals = mainP + sideP + gigP > 0,
        };
    }

    public static DerivedExplorationDto SummarizeExploration(IReadOnlyList<FastTravelPointRawDto> fastTravelPoints)
    {
        // Real saves often expose more fast-travel rows than the old fixed denominator, which saturated a literal
        // ratio at 1.0 and read like map completion. Use a soft bounded curve so the field stays a progression proxy.
        var n = fastTravelPoints.Count;
        const int proxyReferenceCount = 200;
        var proxy = Math.Sqrt(Math.Min(1.0, n / (double)proxyReferenceCount));
        var progressProxy = Math.Round(proxy, 3);
        return new DerivedExplorationDto
        {
            ProgressProxy = progressProxy,
            FastTravelPointCount = n,
            ProxyReferenceCount = proxyReferenceCount,
            Interpretation = "fast_travel_unlock_progress_proxy",
            Confidence = "low",
        };
    }

    private static int CompletionQuestKind(string normalizedLower)
    {
        if (normalizedLower.StartsWith("mq", StringComparison.Ordinal))
        {
            return 1;
        }

        if (normalizedLower.StartsWith("sq", StringComparison.Ordinal))
        {
            return 2;
        }

        if (normalizedLower.StartsWith("ma_", StringComparison.Ordinal)
            || normalizedLower.StartsWith("sts_", StringComparison.Ordinal))
        {
            return 3;
        }

        return 0;
    }

    private static bool QuestNameLooksInProgress(string normalizedLower)
    {
        return normalizedLower.Contains("_active", StringComparison.Ordinal)
            || normalizedLower.Contains("_start", StringComparison.Ordinal);
    }

    public static PhantomLibertySignalsDto SummarizePhantomLiberty(
        IReadOnlyList<QuestFactRawDto> all,
        IReadOnlyList<FastTravelPointRawDto> fastTravel,
        QuestSignalSummaryDto questSignals)
    {
        var progressionSignals = 0;
        foreach (var f in all)
        {
            if (f.Name == null)
            {
                continue;
            }

            var ln = f.Name.ToLowerInvariant();
            if (ln.Contains("ep1", StringComparison.Ordinal)
                || ln.Contains("phantom", StringComparison.Ordinal)
                || ln.Contains("dogtown", StringComparison.Ordinal))
            {
                progressionSignals++;
            }
        }

        var ftEp1 = 0;
        foreach (var p in fastTravel)
        {
            if (p.IsEp1)
            {
                ftEp1++;
            }
        }

        var detected = progressionSignals > 0 || ftEp1 > 0 || questSignals.NamedFactsEp1Hint > 0;
        var likelyStarted = detected;
        var level = "none";
        if (detected)
        {
            if (ftEp1 == 0)
            {
                if (progressionSignals >= 18)
                {
                    level = "mid";
                }
                else if (progressionSignals >= 5 || questSignals.NamedFactsEp1Hint >= 3)
                {
                    level = "early";
                }
                else
                {
                    level = "unknown";
                }
            }
            else if (ftEp1 >= 3 || progressionSignals >= 12)
            {
                level = "mid";
            }
            else
            {
                level = "early";
            }
        }

        string confidence;
        if (!detected)
        {
            confidence = "low";
        }
        else if (ftEp1 > 0 && progressionSignals >= 5)
        {
            confidence = "high";
        }
        else if (ftEp1 > 0 || progressionSignals >= 5 || questSignals.NamedFactsEp1Hint >= 3)
        {
            confidence = "medium";
        }
        else
        {
            confidence = "low";
        }

        return new PhantomLibertySignalsDto
        {
            Detected = detected,
            ProgressionSignals = progressionSignals,
            FastTravelUnlocked = ftEp1,
            LikelyStarted = likelyStarted,
            LikelyProgressLevel = level,
            Interpretation = "heuristic_signal_based",
            Confidence = confidence,
        };
    }

    public static DerivedExpansionDto SummarizeExpansion(
        IReadOnlyList<QuestFactRawDto> all,
        IReadOnlyList<FastTravelPointRawDto> fastTravel,
        QuestSignalSummaryDto questSignals)
    {
        return new DerivedExpansionDto
        {
            PhantomLiberty = SummarizePhantomLiberty(all, fastTravel, questSignals),
        };
    }

    public static DerivedInventoryInsightsDto SummarizeInventoryInsights(
        IReadOnlyList<InventoryItemNormalizedDto> rows,
        int? level,
        ulong? moneyQuantity)
    {
        // Capability-style counts use first row per TweakDbIdUlong so duplicate stacks do not inflate programs/OS/iconic tallies.
        var dedupedForCapability = FirstRowPerTweakDbId(rows);

        var qhProg = 0;
        var qhMat = 0;
        var osCount = 0;
        var iconic = 0;
        var craftingStacks = 0;
        foreach (var row in dedupedForCapability)
        {
            var id = InventoryItemIdLower(row);
            if (id.Contains("iconic", StringComparison.Ordinal))
            {
                iconic++;
            }

            if (LooksLikeOperatingSystemItem(id))
            {
                osCount++;
            }

            var mat = LooksLikeQuickhackMaterial(id);
            if (mat)
            {
                qhMat++;
            }
            else if (LooksLikeQuickhackProgram(id, row.InferredCategory))
            {
                qhProg++;
            }
        }

        // Wealth thresholds: still count crafting category rows (stack-level), not distinct item types.
        foreach (var row in rows)
        {
            if (string.Equals(row.InferredCategory, "Crafting", StringComparison.Ordinal))
            {
                craftingStacks++;
            }
        }

        var hasQh = qhProg > 0 || qhMat > 0;
        var money = moneyQuantity ?? 0UL;
        var lv = level ?? 0;
        var materialWealth = "low";
        if (craftingStacks >= 22 || money > 180000UL)
        {
            materialWealth = "high";
        }
        else if (craftingStacks >= 9 || money > 45000UL)
        {
            materialWealth = "medium";
        }

        var upgradeReadiness = "low";
        if (lv >= 32 && craftingStacks >= 14 && money > 90000UL)
        {
            upgradeReadiness = "high";
        }
        else if (lv >= 18 && (craftingStacks >= 7 || money > 35000UL))
        {
            upgradeReadiness = "medium";
        }

        var buildFocus = "generalist";
        if (qhProg >= 3 && osCount >= 1)
        {
            buildFocus = "netrunner_capable";
        }
        else if (craftingStacks >= 10 && money > 35000UL)
        {
            buildFocus = "crafting_ready";
        }
        else if (iconic >= 3)
        {
            buildFocus = "gear_progression_focus";
        }

        return new DerivedInventoryInsightsDto
        {
            HasQuickhacks = hasQh,
            QuickhackProgramCount = qhProg,
            QuickhackMaterialCount = qhMat,
            HasMultipleOperatingSystems = osCount >= 2,
            OperatingSystemCount = osCount,
            IconicItemCount = iconic,
            MaterialWealth = materialWealth,
            UpgradeReadiness = upgradeReadiness,
            Interpretation = "inventory_signal_based",
            Confidence = "medium",
            BuildCapabilityFocus = buildFocus,
        };
    }

    /// <summary>First occurrence per <see cref="InventoryItemNormalizedDto.TweakDbIdUlong"/> (order-preserving).</summary>
    private static List<InventoryItemNormalizedDto> FirstRowPerTweakDbId(IReadOnlyList<InventoryItemNormalizedDto> rows)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var list = new List<InventoryItemNormalizedDto>(rows.Count);
        foreach (var row in rows)
        {
            if (seen.Add(row.TweakDbIdUlong))
            {
                list.Add(row);
            }
        }

        return list;
    }

    private static string InventoryItemIdLower(InventoryItemNormalizedDto row)
    {
        return (row.ItemIdResolved ?? row.ItemIdPresentation).ToLowerInvariant();
    }

    private static bool LooksLikeQuickhackMaterial(string idLower)
    {
        if (!idLower.Contains("quickhack", StringComparison.Ordinal))
        {
            return false;
        }

        return idLower.Contains("fragment", StringComparison.Ordinal)
            || idLower.Contains("crafting", StringComparison.Ordinal)
            || (idLower.Contains("shard", StringComparison.Ordinal) && !idLower.Contains("program", StringComparison.Ordinal));
    }

    /// <summary>Quickhack daemon programs (inventory), including *Program records that may not use InferredCategory Quickhack.</summary>
    private static bool LooksLikeQuickhackProgram(string idLower, string inferredCategory)
    {
        if (LooksLikeQuickhackMaterial(idLower))
        {
            return false;
        }

        if (string.Equals(inferredCategory, "Quickhack", StringComparison.Ordinal))
        {
            return true;
        }

        if (!idLower.Contains("program", StringComparison.Ordinal))
        {
            return false;
        }

        if (idLower.EndsWith("program", StringComparison.Ordinal))
        {
            return true;
        }

        if (idLower.Contains("lvl", StringComparison.Ordinal) && idLower.Contains("program", StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static bool LooksLikeOperatingSystemItem(string idLower)
    {
        if (idLower.Contains("fragment", StringComparison.Ordinal)
            || idLower.Contains("shard", StringComparison.Ordinal)
            || idLower.Contains("cyberwarestatsshard", StringComparison.Ordinal)
            || idLower.Contains("cyberwareupgradeshard", StringComparison.Ordinal)
            || idLower.Contains("upgradeshard", StringComparison.Ordinal)
            || idLower.Contains("upgrade_shard", StringComparison.Ordinal))
        {
            return false;
        }

        if (idLower.Contains("sandevistan", StringComparison.Ordinal)
            || idLower.Contains("berserk", StringComparison.Ordinal)
            || idLower.Contains("cyberdeck", StringComparison.Ordinal))
        {
            return true;
        }

        return idLower.Contains("operating_system", StringComparison.Ordinal)
            || idLower.Contains("operatingsystem", StringComparison.Ordinal);
    }

    public static DerivedEquipmentMaturityDto SummarizeEquipmentMaturity(
        int? level,
        ulong? moneyQuantity,
        IReadOnlyList<InventoryItemNormalizedDto> invRows,
        IReadOnlyList<EquippedSlotNormalizedDto> slotsDeduped,
        IReadOnlyList<CyberwareSlotNormalizedDto> cyberDeduped,
        int cyberwareSlotRowCount)
    {
        var scores = new List<int>();
        foreach (var s in slotsDeduped)
        {
            var sc = TierScoreFromItemText(s.ItemIdResolved ?? "");
            if (sc != null)
            {
                scores.Add(sc.Value);
            }
        }

        foreach (var c in cyberDeduped)
        {
            var sc = TierScoreFromItemText(c.ItemIdResolved ?? "");
            if (sc != null)
            {
                scores.Add(sc.Value);
            }
        }

        var avgTier = "unknown";
        if (scores.Count > 0)
        {
            var a = scores.Average();
            avgTier = a >= 4.5 ? "legendary" : a >= 3.5 ? "epic" : a >= 2.5 ? "rare" : a >= 1.5 ? "uncommon" : "common";
        }

        var craftingLike = 0;
        foreach (var r in invRows)
        {
            if (string.Equals(r.InferredCategory, "Crafting", StringComparison.Ordinal))
            {
                craftingLike++;
            }
        }

        var lv = level ?? 0;
        var money = moneyQuantity ?? 0;
        var epicPlus = scores.Count(s => s >= 4);
        var potential = "low";
        if (lv >= 35 && money > 150000UL && craftingLike >= 12 && epicPlus >= 2)
        {
            potential = "high";
        }
        else if (lv >= 18 && (craftingLike >= 6 || money > 40000UL || epicPlus >= 1))
        {
            potential = "medium";
        }

        var possible = CyberwareSlotsPossible;
        var util = possible <= 0
            ? 0.0
            : Math.Min(1.0, cyberwareSlotRowCount / (double)possible);
        var utilRounded = Math.Round(util, 3);
        var investment = utilRounded < 0.30 ? "low" : utilRounded < 0.70 ? "medium" : "high";

        return new DerivedEquipmentMaturityDto
        {
            AverageTier = avgTier,
            CyberwareSlotsUsed = cyberwareSlotRowCount,
            CyberwareSlotsPossible = possible,
            CyberwareUtilizationRatio = utilRounded,
            UpgradePotential = potential,
            CyberwareInvestmentLevel = investment,
            Interpretation = "slot_utilization_based",
            Confidence = "medium",
        };
    }

    private static int? TierScoreFromItemText(string text)
    {
        var t = text.ToLowerInvariant();
        if (t.Contains("legendary", StringComparison.Ordinal))
        {
            return 5;
        }

        if (t.Contains("epic", StringComparison.Ordinal))
        {
            return 4;
        }

        if (t.Contains("uncommon", StringComparison.Ordinal))
        {
            return 2;
        }

        if (t.Contains("rare", StringComparison.Ordinal))
        {
            return 3;
        }

        if (t.Contains("common", StringComparison.Ordinal))
        {
            return 1;
        }

        return null;
    }

    public static DerivedBuildProfileDto SummarizeBuildProfile(
        CoreAttributesDto core,
        IReadOnlyList<string> buildTags,
        IReadOnlyList<CyberwareSlotInstanceDto> cyberInstances,
        DerivedInventoryInsightsDto inventoryInsights)
    {
        var signals = new List<string>();
        var intel = core.Intelligence ?? 0;
        var body = core.Body ?? 0;
        var cool = core.Cool ?? 0;
        var tech = core.Technical ?? 0;
        var reflex = core.Reflexes ?? 0;

        if (intel >= 12)
        {
            signals.Add("high intelligence");
        }

        if (body >= 12)
        {
            signals.Add("body >= 12");
        }

        if (cool >= 12)
        {
            signals.Add("cool >= 12");
        }

        if (tech >= 12)
        {
            signals.Add("technical >= 12");
        }

        if (inventoryInsights.HasQuickhacks)
        {
            signals.Add("quickhack inventory present");
        }

        var cyberdeckEquipped = false;
        foreach (var c in cyberInstances)
        {
            var id = (c.ItemIdResolved ?? c.ItemIdPresentation).ToLowerInvariant();
            if (id.Contains("cyberdeck", StringComparison.Ordinal)
                || id.Contains("frontalcortex", StringComparison.Ordinal) && id.Contains("deck", StringComparison.Ordinal))
            {
                cyberdeckEquipped = true;
                break;
            }
        }

        if (cyberdeckEquipped)
        {
            signals.Add("cyberdeck equipped");
        }

        foreach (var t in buildTags.Take(4))
        {
            signals.Add("tag: " + t);
        }

        var primary = "hybrid-generalist";
        if (intel >= 14 && tech >= 11 && (inventoryInsights.HasQuickhacks || cyberdeckEquipped))
        {
            primary = "netrunner-tech";
        }
        else if (body >= 14 && tech >= 11)
        {
            primary = "body-tech";
        }
        else if (cool >= 14 && reflex >= 10)
        {
            primary = "stealth-cool";
        }

        var secondary = PickSecondaryArchetype(primary, intel, body, cool, tech, reflex, inventoryInsights.HasQuickhacks, cyberdeckEquipped);

        var confidence = "low";
        if (signals.Count >= 5)
        {
            confidence = "high";
        }
        else if (signals.Count >= 2)
        {
            confidence = "medium";
        }

        return new DerivedBuildProfileDto
        {
            Primary = primary,
            Secondary = secondary,
            Confidence = confidence,
            Signals = signals,
        };
    }

    private static string PickSecondaryArchetype(
        string primary,
        int intel,
        int body,
        int cool,
        int tech,
        int reflex,
        bool hasQuickhacks,
        bool cyberdeckEquipped)
    {
        if (primary == "netrunner-tech")
        {
            if (body >= 12 && body >= intel)
            {
                return "body-tech";
            }

            return cool >= 12 ? "stealth-cool" : "hybrid-generalist";
        }

        if (primary == "body-tech")
        {
            if (intel >= 12 && (hasQuickhacks || cyberdeckEquipped))
            {
                return "netrunner-tech";
            }

            return cool >= 12 ? "stealth-cool" : "hybrid-generalist";
        }

        if (primary == "stealth-cool")
        {
            if (intel >= 12 && (hasQuickhacks || cyberdeckEquipped))
            {
                return "netrunner-tech";
            }

            return body >= 12 ? "body-tech" : "hybrid-generalist";
        }

        if (intel >= 12 && tech >= 10 && (hasQuickhacks || cyberdeckEquipped))
        {
            return "netrunner-tech";
        }

        if (body >= 12 && tech >= 10)
        {
            return "body-tech";
        }

        if (cool >= 12 && reflex >= 9)
        {
            return "stealth-cool";
        }

        return "hybrid-generalist";
    }
}
