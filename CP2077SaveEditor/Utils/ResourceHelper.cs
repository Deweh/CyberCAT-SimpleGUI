using System.Collections.Generic;
using System.Text.Json;
using WolvenKit.RED4.Types;

namespace CP2077SaveEditor.Utils;

public class ItemRecord
{
    public string Type { get; set; }
    public bool IsSingleInstance { get; set; }
}

public class FastTravelRecord
{
    public string PointRecord { get; set; }
    public string MarkerRef { get; set; }
}

public static class ResourceHelper
{
    public static readonly Dictionary<ulong, ItemRecord> ItemClasses = JsonSerializer.Deserialize<Dictionary<ulong, ItemRecord>>(Properties.Resources.ItemClasses);
    public static readonly List<FastTravelRecord> FastTravelRecords = JsonSerializer.Deserialize<List<FastTravelRecord>>(Properties.Resources.FastTravel);
}