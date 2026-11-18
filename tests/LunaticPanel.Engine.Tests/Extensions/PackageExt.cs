using LunaticPanel.Engine.Plugin;
using System;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace LunaticPanel.Engine.Tests.Extensions;

public static class PackageExt
{
    internal static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };
    public static PluginManifestEntity CreateManifest(string id,
    string title,
    string version,
    string panelVersion) => new()
    {
        Id = id,
        Title = title,
        Company = "Contoso",
        Version = version,
        Description = "Package test description.",
        Author = "Whatever",
        Copyright = "© Contoso",
        PanelVersion = panelVersion,
        DotnetVersion = "8.0.12",
        PluginEntryFile = "Package.dll"
    };
    public static PluginManifestEntity CreateFakePackage(string id,
    string title,
    string version,
    string panelVersion, string location)
    {
        var toPack = CreateManifest(id, title, version, panelVersion);
        string filePath = Path.Combine(location, toPack.PackageFileName);
        if (File.Exists(filePath)) File.Delete(filePath);
        using (var archive = ZipFile.Open(filePath, ZipArchiveMode.Create))
        {
            var entry = archive.CreateEntry("manifest.json", CompressionLevel.Optimal);

            using (var stream = entry.Open())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(JsonSerializer.Serialize(toPack, _jsonSerializerOptions));
            }
        }
        toPack.PackageFileLocation = filePath;
        return toPack;
    }
    public static void CreateFakePackage(PluginManifestEntity toPack, string location)
    {
        string filePath = Path.Combine(location, toPack.PackageFileName);
        if (File.Exists(filePath)) File.Delete(filePath);
        using (var archive = ZipFile.Open(filePath, ZipArchiveMode.Create))
        {
            var entry = archive.CreateEntry("manifest.json", CompressionLevel.Optimal);

            using (var stream = entry.Open())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(JsonSerializer.Serialize(toPack, _jsonSerializerOptions));
            }
        }
    }
}
