using WolvenKit.RED4.Save;
using WolvenKit.RED4.Save.IO;

namespace CP2077SaveExporter;

/// <summary>
/// Opens <c>sav.dat</c> read-only and returns a parsed <see cref="CyberpunkSaveFile"/>.
/// </summary>
public static class SaveLoader
{
    /// <summary>
    /// Loads the CSAV. The returned <paramref name="file"/> is independent of the reader after this call returns.
    /// </summary>
    public static (EFileReadErrorCodes Code, CyberpunkSaveFile? File) Load(string path)
    {
        using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new CyberpunkSaveReader(fs);
        var code = reader.ReadFile(out var file);
        return (code, file);
    }
}
