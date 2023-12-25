using System.Collections.Generic;
using System.Text.Json;

namespace CP2077SaveEditor.Utils;

public class ItemRecord
{
    public string Type { get; set; }
    public bool IsSingleInstance { get; set; }
    public int IsItemPlus { get; set; }
    public List<SlotPartRecord> SlotParts { get; set; }
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
}