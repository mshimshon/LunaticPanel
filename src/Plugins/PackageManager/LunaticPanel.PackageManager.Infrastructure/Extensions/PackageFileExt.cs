using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Services.Payloads;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Extensions;

internal static class PackageFileExt
{
    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
#if DEBUG
        WriteIndented = true,
#endif
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
    };
    public static PackagePayload GetPackageInformation(string file)
    {
        var manifest = ReadManifestFromArchive(file);
        return new()
        {
            Version = manifest.Version,
            Info = new()
            {
                Description = manifest.Description,
                Name = manifest.Title,
                PackageId = manifest.Id,
                State = Application.Payloads.Enums.PackageStatePayload.Unknown
            }
        };
    }
    public static PluginManifestExtPayload ReadManifestFromArchive(string input)
    {
        using var zip = ZipFile.OpenRead(input);
        var entry = zip.GetEntry("manifest.json");
        if (entry == null)
            throw new Exception("manifest.json not found in package");
        using var stream = entry.Open();
        return JsonSerializer.Deserialize<PluginManifestExtPayload>(stream, _jsonSerializerOptions)!;
    }
}
