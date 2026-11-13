using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Optional config.json in the working directory or next to the executable, e.g. { "Theme": 2 }.
    /// </summary>
    public static class AppConfig
    {
        private static readonly Dictionary<string, int> s_values = Load();

        /// <summary>
        /// Shows work in progress features. "EnableWIPFeatures": 1
        /// </summary>
        public static bool EnableWipFeatures => s_values.GetValueOrDefault("EnableWIPFeatures") == 1;

        /// <summary>
        /// 0 follows Windows, 1 light, 2 dark (default: 2 for Cyberpunk 2077 dark HUD). "Theme": 1 forces light mode.
        /// </summary>
        public static int Theme => s_values.GetValueOrDefault("Theme", 2);

        /// <summary>
        /// Looks for a newer GitHub release on startup, on by default. "CheckForUpdates": 0 turns it off
        /// </summary>
        public static bool CheckForUpdates => s_values.GetValueOrDefault("CheckForUpdates", 1) == 1;

        /// <summary>
        /// How many edits Ctrl+Z can undo, 10 by default. Each step keeps a copy of the save in memory (~4 MB). "UndoSteps": 0 turns it off
        /// </summary>
        public static int UndoSteps => s_values.GetValueOrDefault("UndoSteps", 10);

        private static Dictionary<string, int> Load()
        {
            var path = new[] { Environment.CurrentDirectory, AppContext.BaseDirectory }
                .Select(x => Path.Combine(x, "config.json"))
                .FirstOrDefault(File.Exists);
            if (path == null)
            {
                return new();
            }

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText(path)) ?? new();
            }
            catch (Exception e)
            {
                Logger.Error($"Unable to read {path}", e);
                return new();
            }
        }
    }
}
