using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Package.Tool.Exceptions;
using LunaticPanel.Package.Tool.Payloads;
using System.IO.Compression;
using System.Reflection;
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
        LunaticPanel.Engine.Plugin.Entities.PluginScannedEntity? entity = PluginScannerExt.FindPluginDllInDirectory(inputFolder, [], DependencySettings.ScanSharedFrameworkNames());
        if (entity == default)
            throw new PluginDllNotFoundException(inputFolder);

        Console.Out.WriteLine($"Load Plugin to Extract Manifest".Cyan());
        var asm = entity.Loader.Load();
        Console.Out.WriteLine($"Extracting Manifest Information for {entity.PluginId}".Cyan());
        var pluginId = entity.PluginId;
        Console.Out.WriteLine($"pluginId:{pluginId}".Magenta());
        var description = asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        Console.Out.WriteLine($"description:{description}".Magenta());
        var company = asm.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;

        Console.Out.WriteLine($"company:{company}".Magenta());
        var title = asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        Console.Out.WriteLine($"title:{title}".Magenta());
        var version = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion!.Split('+')[0];
        Console.Out.WriteLine($"version:{version}".Magenta());
        var copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        Console.Out.WriteLine($"copyright:{copyright}".Magenta());

        entity.Loader.Unload();
        if (pluginId == default)
            throw new PluginIdNotFoundException(entity.Location);

        if (version == default)
            throw new PluginVersionNotFoundException(pluginId);

        if (description == default)
            throw new PluginDescriptionNotFoundException(pluginId);
        if (company == default)
            throw new PluginCompanyNotFoundException();


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
            PluginEntryFile = Path.GetFileName(entity.Location)
        };

    }

}


