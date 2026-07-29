using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Package.Tool.Exceptions;
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

    public static PluginManifestPayload GetManifestInformation(string inputFolder)
    {
        if (!Directory.Exists(inputFolder))
            throw new DirectoryNotFoundException(inputFolder);
        Console.Out.WriteLine($"Searching for Plugin in {inputFolder}".Cyan());
        string? dll = PluginScannerExt.FindPluginDllInDirectory(inputFolder, [], DependencySettings.ScanSharedFrameworkNames());
        if (dll == default)
            throw new PluginDllNotFoundException(inputFolder);
        var meta = DotnetInspectorExt.ExtractMetadata(dll);
        foreach (var item in meta)
            Console.Out.WriteLine($"{item.Key}:{item.Value}".Cyan());
        Console.Out.WriteLine($"Extract Manifest".Cyan());
        Console.Out.WriteLine($"Extracting Manifest Information for {dll}".Cyan());
        var pluginId = meta[ManifestMeta.Id];
        if (pluginId == default)
            throw new PluginIdNotFoundException(dll);
        Console.Out.WriteLine($"pluginId:{pluginId}".Magenta());
        var description = meta[ManifestMeta.Description];
        Console.Out.WriteLine($"description:{description}".Magenta());
        if (description == default)
            throw new PluginDescriptionNotFoundException(pluginId);
        var company = meta[ManifestMeta.Company];
        Console.Out.WriteLine($"company:{company}".Magenta());
        if (company == default)
            throw new PluginCompanyNotFoundException();

        var title = meta[ManifestMeta.Title];
        Console.Out.WriteLine($"title:{title}".Magenta());
        var version = meta[ManifestMeta.Version]!.Split('+')[0];
        Console.Out.WriteLine($"version:{version}".Magenta());
        if (version == default)
            throw new PluginVersionNotFoundException(pluginId);

        var copyright = meta[ManifestMeta.Copyright];
        Console.Out.WriteLine($"copyright:{copyright}".Magenta());

        return new PluginManifestPayload()
        {
            Id = pluginId,
            Title = title ?? pluginId,
            Company = company,
            Author = company,
            Copyright = copyright,
            Description = description,
            Version = version,
            PanelVersion = PackSettings.LunaticPanelVersion.ToString(),
            DotnetVersion = PackSettings.DotNetVersion.ToString(),
            PluginEntryFile = Path.GetFileName(dll)
        };

    }

}


