using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Utils;

public class ItemRecord
{
    public string Type { get; set; }
    public bool IsSingleInstance { get; set; }
    public int IsItemPlus { get; set; }
    public List<SlotPartRecord> SlotParts { get; set; }
    // Fixed rarity of the record ("Rare", "EpicPlus"...), null when the game rolls it (most weapons and clothing)
    public string Quality { get; set; }
}

public class SlotPartRecord
{
    public ulong ItemPartPreset { get; set; }
    public ulong Slot { get; set; }
}

public class FastTravelRecord
{
    public string PointRecord { get; set; }
    public string MarkerRef { get; set; }
}

public record ModifierGroup(string Name, uint CRC, List<string> StatTypes);

public static class ResourceHelper
{
    public static readonly Dictionary<ulong, ItemRecord> ItemClasses = JsonSerializer.Deserialize<Dictionary<ulong, ItemRecord>>(Properties.Resources.ItemClasses);
    public static readonly List<FastTravelRecord> FastTravelRecords = JsonSerializer.Deserialize<List<FastTravelRecord>>(Properties.Resources.FastTravel);
    public static readonly List<ModifierGroup> ModifierGroups = JsonSerializer.Deserialize<List<ModifierGroup>>(Properties.Resources.ModifierGroups);

    // In-game names of items and vehicles, keyed by TweakDBID hash
    public static readonly Dictionary<ulong, string> DisplayNames = LoadDisplayNames();

    public static string GetDisplayName(TweakDBID id) => DisplayNames.GetValueOrDefault((ulong)id);

    private static Dictionary<ulong, string> LoadDisplayNames()
    {
        var allLanguages = JsonSerializer.Deserialize<Dictionary<string, Dictionary<ulong, string>>>(Properties.Resources.DisplayNames);

        // English as the base, so records without a translation still get a name
        var names = new Dictionary<ulong, string>(allLanguages["en-us"]);

        if (UiLanguage != null && allLanguages.TryGetValue(UiLanguage, out var localized))
        {
            foreach (var (id, name) in localized)
            {
                names[id] = name;
            }
        }

        return names;
    }

    // Game language matching the Windows UI language, null for English
    private static string UiLanguage => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
    {
        "pt" => "pt-br",
        _ => null
    };

    // Equipment area of every cyberware item, e.g. "EyesCW"
    public static readonly Dictionary<ulong, string> CyberwareAreas = JsonSerializer.Deserialize<Dictionary<ulong, string>>(Properties.Resources.Cyberware);

    public static readonly List<PerkRecord> Perks = JsonSerializer.Deserialize<List<PerkRecord>>(Properties.Resources.Perks);

    public static string GetPerkName(PerkRecord perk) =>
        (UiLanguage != null ? perk.Names.GetValueOrDefault(UiLanguage) : null) ?? perk.Names.GetValueOrDefault("en-us") ?? perk.Type;
}

public record PerkRecord(string Type, string Attribute, int MaxLevel, string Tier, Dictionary<string, string> Names);