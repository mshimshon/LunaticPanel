using LunaticPanel.Core.Extensions;
using LunaticPanel.Package.Tool.Payloads;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.Package.Tool.Tools.Plugin;

internal static class PluginUtilitiesExt
{

    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
#if DEBUG
        WriteIndented = true,
#endif
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
    };
    public static PluginManifestPayload ReadManifestFromArchive(string input)
    {
        Console.Out.WriteLine($"Trying Read Manifest From {input}".Cyan());

        using var zip = ZipFile.OpenRead(input);

        // Find the manifest entry
        var entry = zip.GetEntry("manifest.json");
        if (entry == null)
            throw new FileNotFoundException("manifest.json not found in package");
        using var stream = entry.Open();
        return JsonSerializer.Deserialize<PluginManifestPayload>(stream, _jsonSerializerOptions)!;
    }
}


