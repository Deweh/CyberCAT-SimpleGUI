using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CP2077SaveEditor
{
    public enum AppearanceGender
    {
        Female,
        Male
    }

    public static class AppearanceValueLists
    {
        public static Dictionary<string, ulong> HairStylesDict { get; }
        public static List<string> HairStyles { get; }
        public static List<string> HairColors { get; }
        public static List<string> SkinTones { get; }
        public static List<ulong> SkinTypes { get; }
        public static List<string> EyeColors { get; }
        public static List<ulong> Eyebrows { get; }
        public static List<ulong> LipMakeups { get; }
        public static List<string> EyebrowColors { get; }
        public static List<string> LipMakeupColors { get; }
        public static List<ulong> EyeMakeups { get; }
        public static List<string> EyeMakeupColors { get; }
        public static Dictionary<string, List<ulong>> BodyTattoos { get; }
        public static List<string> Nailss { get; }
        public static List<ulong> FacialTattoos { get; }
        public static List<ulong> Piercings { get; }
        public static List<string> Teeth { get; }
        public static List<string> FacialScars { get; }
        public static List<ulong> BodyScars { get; }
        public static List<string> PiercingColors { get; }
        public static List<ulong> CheekMakeups { get; }
        public static List<string> CheekMakeupColors { get; }
        public static List<ulong> Blemishes { get; }
        public static List<string> Genitals { get; }
        public static List<string> PenisSizes { get; }
        public static List<ulong> PubicHairStyles { get; }
        public static List<string> NailColors { get; }
        public static List<string> Beards { get; }
        public static Dictionary<string, List<string>> BeardStyles { get; }

        static AppearanceValueLists()
        {
            var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Properties.Resources.AppearanceValues);

            HairStylesDict = values["HairStyles"].Deserialize<Dictionary<string, ulong>>();
            HairStyles = HairStylesDict.Keys.ToList();
            HairColors = values["HairColors"].Deserialize<List<string>>();
            SkinTones = values["SkinTones"].Deserialize<List<string>>();
            SkinTypes = values["SkinTypes"].Deserialize<List<ulong>>();
            EyeColors = values["EyeColors"].Deserialize<List<string>>();
            Eyebrows = values["Eyebrows"].Deserialize<List<ulong>>();
            LipMakeups = values["LipMakeups"].Deserialize<List<ulong>>();
            EyebrowColors = values["EyebrowColors"].Deserialize<List<string>>();
            LipMakeupColors = values["LipMakeupColors"].Deserialize<List<string>>();
            EyeMakeups = values["EyeMakeups"].Deserialize<List<ulong>>();
            EyeMakeupColors = values["EyeMakeupColors"].Deserialize<List<string>>();
            BodyTattoos = values["BodyTattoos"].Deserialize<Dictionary<string, List<ulong>>>();
            Nailss = new List<string> { "Short", "Long" };
            FacialTattoos = values["FacialTattoos"].Deserialize<List<ulong>>();
            Piercings = values["Piercings"].Deserialize<List<ulong>>();
            Teeth = values["Teeth"].Deserialize<List<string>>();
            FacialScars = values["FacialScars"].Deserialize<List<string>>();
            BodyScars = values["BodyScars"].Deserialize<List<ulong>>();
            PiercingColors = values["PiercingColors"].Deserialize<List<string>>();
            CheekMakeups = values["CheekMakeups"].Deserialize<List<ulong>>();
            CheekMakeupColors = values["CheekMakeupColors"].Deserialize<List<string>>();
            Blemishes = values["Blemishes"].Deserialize<List<ulong>>();
            Genitals = values["Genitals"].Deserialize<List<string>>();
            PenisSizes = values["PenisSizes"].Deserialize<List<string>>();
            PubicHairStyles = values["PubicHairStyles"].Deserialize<List<ulong>>();
            NailColors = values["NailColors"].Deserialize<List<string>>();
            Beards = values["Beards"].Deserialize<List<string>>();
            BeardStyles = values["BeardStyles"].Deserialize<Dictionary<string, List<string>>>();
        }
    }
}
