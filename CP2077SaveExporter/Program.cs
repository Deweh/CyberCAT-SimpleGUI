using System.Text.Json;
using WolvenKit.Common.Services;
using WolvenKit.Core.Compression;
using WolvenKit.RED4.Save.IO;
using WolvenKit.RED4.TweakDB.Helper;

namespace CP2077SaveExporter;

internal static class Program
{
    private const int ExitOk = 0;
    private const int ExitUsage = 1;
    private const int ExitLoadError = 2;
    private const int ExitInitError = 3;

    private const string RawFileName = "save.raw.json";
    private const string FullFileName = "save.full.json";
    private const string InsightsFileName = "save.insights.json";

    private enum ExportOutputMode
    {
        Raw,
        Full,
        Insights,
        Split,
    }

    private static async Task<int> Main(string[] args)
    {
        Console.Error.WriteLine("CP2077 read-only save exporter (WolvenKit-based).");

        if (args.Length == 0)
        {
            PrintUsage();
            return ExitUsage;
        }

        if (args.Length == 1 && args[0] is "-h" or "--help")
        {
            PrintUsage();
            return ExitOk;
        }

        if (!TryParseArgs(
                args,
                out var savPath,
                out var mode,
                out var outputDirectory,
                out var factsPathOverride,
                out var parseError))
        {
            if (parseError != null)
            {
                Console.Error.WriteLine(parseError);
            }

            return ExitUsage;
        }

        // Match CP2077SaveEditor Form2: Oodle off for embedded kark decompression used by HashService / streams.
        CompressionSettings.Get().UseOodle = false;

        // Editor also calls ModManager.LoadTypes() (separate assembly) to register modded RED classes for modded saves.
        // That path is not available here without referencing the editor; modded ScriptableSystems may fail to parse.

        savPath = Path.GetFullPath(savPath);

        if (!File.Exists(savPath))
        {
            Console.Error.WriteLine($"File not found: {savPath}");
            return ExitLoadError;
        }

        HashService? hashService = null;
        TweakDBStringHelper? tweakDbStrings = null;

        try
        {
            // Older WolvenKit: HashService loads embedded pools synchronously in the constructor (no Task Loaded).
            Console.Error.WriteLine("Initializing hash / string resolution (WolvenKit.Common)...");
            hashService = new HashService();

            Console.Error.WriteLine("Loading TweakDB string sidecar (optional CRC-based map; parallel to HashService pools)...");
            using (var tweakStream = typeof(HashService).Assembly.GetManifestResourceStream("WolvenKit.Common.Resources.tweakdbstr.kark"))
            {
                if (tweakStream != null)
                {
                    tweakDbStrings = new TweakDBStringHelper();
                    tweakDbStrings.LoadFromStream(tweakStream);
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Initialization failed: {ex.Message}");
            return ExitInitError;
        }

        Console.Error.WriteLine($"Reading save: {savPath}");
        var (code, save) = SaveLoader.Load(savPath);
        if (code != EFileReadErrorCodes.NoError || save == null)
        {
            Console.Error.WriteLine($"ReadFile failed: {code}");
            return ExitLoadError;
        }

        IReadOnlyDictionary<uint, string>? knownFacts = null;
        var factsPath = factsPathOverride != null
            ? factsPathOverride
            : Path.Combine(AppContext.BaseDirectory, "Facts.json");
        if (File.Exists(factsPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(factsPath).ConfigureAwait(false);
                knownFacts = TryParseFactsJsonObject(json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Warning: could not parse Facts.json; quest fact names will be omitted. " + ex.Message);
            }
        }
        else if (factsPathOverride != null)
        {
            Console.Error.WriteLine(
                $"Warning: Facts.json not found at '{factsPath}'; quest fact names will be hash-only.");
        }
        else
        {
            Console.Error.WriteLine("Warning: Facts.json not beside executable; quest fact names will be hash-only.");
        }

        _ = tweakDbStrings;

        var snapshot = ProgressExtractor.Build(save, hashService!, knownFacts);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        var outDir = outputDirectory == null
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(outputDirectory);
        var rawPath = Path.Combine(outDir, RawFileName);
        var fullPath = Path.Combine(outDir, FullFileName);
        var insightsPath = Path.Combine(outDir, InsightsFileName);

        var rawSnapshot = new RawExportSnapshot
        {
            Header = snapshot.Header,
            Raw = snapshot.Raw,
        };
        var insightsSnapshot = new InsightsExportSnapshot
        {
            Normalized = snapshot.Normalized,
            Derived = snapshot.Derived,
        };

        switch (mode)
        {
            case ExportOutputMode.Raw:
                await JsonExporter.WriteAsync(rawPath, rawSnapshot, jsonOptions).ConfigureAwait(false);
                Console.Error.WriteLine($"Wrote: {rawPath}");
                break;
            case ExportOutputMode.Full:
                await JsonExporter.WriteAsync(fullPath, snapshot, jsonOptions).ConfigureAwait(false);
                Console.Error.WriteLine($"Wrote: {fullPath}");
                break;
            case ExportOutputMode.Insights:
                await JsonExporter.WriteAsync(insightsPath, insightsSnapshot, jsonOptions).ConfigureAwait(false);
                Console.Error.WriteLine($"Wrote: {insightsPath}");
                break;
            case ExportOutputMode.Split:
                await JsonExporter.WriteAsync(rawPath, rawSnapshot, jsonOptions).ConfigureAwait(false);
                Console.Error.WriteLine($"Wrote: {rawPath}");
                await JsonExporter.WriteAsync(insightsPath, insightsSnapshot, jsonOptions).ConfigureAwait(false);
                Console.Error.WriteLine($"Wrote: {insightsPath}");
                break;
        }

        return ExitOk;
    }

    private static void PrintUsage()
    {
        Console.Error.WriteLine(
            "Usage:");
        Console.Error.WriteLine(
            "  CP2077SaveExporter [--mode raw|full|insights|split] [--facts <path-to-Facts.json>] <path-to-sav.dat> [output-directory]");
        Console.Error.WriteLine();
        Console.Error.WriteLine("Default mode: full");
        Console.Error.WriteLine("Output directory defaults to the current working directory when omitted.");
        Console.Error.WriteLine();
        Console.Error.WriteLine("Modes:");
        Console.Error.WriteLine($"  raw       — raw decoded data → {RawFileName}");
        Console.Error.WriteLine($"  full      — raw + normalized + derived → {FullFileName}");
        Console.Error.WriteLine($"  insights  — normalized + derived only → {InsightsFileName}");
        Console.Error.WriteLine($"  split     — {RawFileName} + {InsightsFileName} (two files)");
        Console.Error.WriteLine();
        Console.Error.WriteLine(
            "--facts overrides automatic Facts.json discovery; when omitted, Facts.json beside the running executable is used if present.");
    }

    private static bool TryParseArgs(
        string[] args,
        out string savPath,
        out ExportOutputMode mode,
        out string? outputDirectory,
        out string? factsPathOverride,
        out string? error)
    {
        savPath = "";
        mode = ExportOutputMode.Full;
        outputDirectory = null;
        factsPathOverride = null;
        error = null;

        var positionals = new List<string>();

        for (var i = 0; i < args.Length;)
        {
            var a = args[i];
            if (a == "--mode")
            {
                if (i + 1 >= args.Length)
                {
                    error = "--mode requires a value (raw, full, insights, or split).";
                    return false;
                }

                var m = args[i + 1].ToLowerInvariant();
                switch (m)
                {
                    case "raw":
                        mode = ExportOutputMode.Raw;
                        break;
                    case "full":
                        mode = ExportOutputMode.Full;
                        break;
                    case "insights":
                        mode = ExportOutputMode.Insights;
                        break;
                    case "split":
                        mode = ExportOutputMode.Split;
                        break;
                    default:
                        error = $"Invalid --mode value: {args[i + 1]} (expected raw, full, insights, or split)";
                        return false;
                }

                i += 2;
                continue;
            }

            if (a == "--facts")
            {
                if (i + 1 >= args.Length)
                {
                    error = "--facts requires a path to Facts.json.";
                    return false;
                }

                factsPathOverride = args[i + 1];
                i += 2;
                continue;
            }

            if (a.Length > 0 && a[0] == '-')
            {
                error = $"Unknown option: {a}";
                return false;
            }

            positionals.Add(a);
            i++;
        }

        if (positionals.Count == 0)
        {
            error = "Missing save path.";
            return false;
        }

        if (positionals.Count > 2)
        {
            error = "Unexpected extra arguments.";
            return false;
        }

        savPath = positionals[0];
        if (string.IsNullOrWhiteSpace(savPath))
        {
            error = "Missing save path.";
            return false;
        }

        if (savPath.Length > 0 && savPath[0] == '-')
        {
            error = $"Unknown option: {savPath}";
            return false;
        }

        if (positionals.Count == 2)
        {
            var od = positionals[1];
            if (string.IsNullOrWhiteSpace(od))
            {
                error = "Invalid output directory.";
                return false;
            }

            if (od.Length > 0 && od[0] == '-')
            {
                error = $"Unknown option: {od}";
                return false;
            }

            outputDirectory = od;
        }

        return true;
    }

    /// <summary>
    /// Facts.json is a single JSON object whose keys are decimal fact hashes (as JSON strings) and values are fact names (strings).
    /// Uses <see cref="JsonDocument"/> so deserialization does not depend on Dictionary key typing quirks.
    /// </summary>
    private static Dictionary<uint, string>? TryParseFactsJsonObject(string json)
    {
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var dict = new Dictionary<uint, string>();
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            if (!uint.TryParse(prop.Name, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var key))
            {
                continue;
            }

            if (prop.Value.ValueKind == JsonValueKind.String)
            {
                dict[key] = prop.Value.GetString() ?? "";
            }
        }

        return dict;
    }
}
