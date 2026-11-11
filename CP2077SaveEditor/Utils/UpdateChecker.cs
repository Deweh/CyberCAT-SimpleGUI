using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Compares the app version (Version in the .csproj) with the latest GitHub release.
    /// </summary>
    public static class UpdateChecker
    {
        public const string ReleasesPage = "https://github.com/bernardomcma/CyberCAT-SimpleGUI/releases/latest";
        private const string LatestReleaseApi = "https://api.github.com/repos/bernardomcma/CyberCAT-SimpleGUI/releases/latest";

        public static Version CurrentVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0);

        public static string CurrentVersionText => Format(CurrentVersion);

        /// <summary>
        /// Returns the tag of the latest release if it is newer than this build, otherwise null.
        /// Never throws: being offline or rate limited just means no update is shown.
        /// </summary>
        public static async Task<string> GetNewerReleaseAsync()
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                client.DefaultRequestHeaders.UserAgent.ParseAdd("CP2077SaveEditor/" + CurrentVersionText);

                // 404 while the repository has no release yet
                using var response = await client.GetAsync(LatestReleaseApi);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var tag = json.RootElement.GetProperty("tag_name").GetString();

                return Version.TryParse(tag?.TrimStart('v', 'V'), out var latest) && latest > CurrentVersion ? tag : null;
            }
            catch (Exception e)
            {
                Logger.Error("Update check failed", e);
                return null;
            }
        }

        // 0.29.0.0 -> v0.29, 0.29.1.0 -> v0.29.1
        private static string Format(Version version) =>
            version.Build > 0 ? $"v{version.Major}.{version.Minor}.{version.Build}" : $"v{version.Major}.{version.Minor}";
    }
}
