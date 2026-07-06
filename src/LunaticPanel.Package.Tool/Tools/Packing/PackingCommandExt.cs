using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Package.Tool.Exceptions;
using LunaticPanel.Package.Tool.Exceptions.PackExceptions;
using LunaticPanel.Package.Tool.Exceptions.UnpackExceptions;
using LunaticPanel.Package.Tool.Extensions;
using LunaticPanel.Package.Tool.Payloads;
using LunaticPanel.Package.Tool.Tools.Plugin;
using LunaticPanel.Package.Tool.Tools.Validation;
using System.CommandLine;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.Package.Tool.Tools.Packing;

internal static class PackingCommandExt
{


    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
#if DEBUG
        WriteIndented = true,
#endif
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
    };
    public static RootCommand WithPackCommands(this RootCommand root)
    {
        var command = new Command("pack", "pack plugin folder to .lpkg")
            .AddOption<string>("input", "in", "this is the input folder of the plugin.")
            .AddOption<string>("output", "out", "this is the folder where to write the 'pluginid.version.lpkg' file.")
            .SetExecuteCommand(PackingAction);
        return root.WithSubCommand(command);
    }
    public static RootCommand WithUnPackCommands(this RootCommand root)
    {
        var command = new Command("unpack", "pack plugin folder to .lpkg")
            .AddOption<string>("input", "in", "this is the .lpkg file.")
            .AddOption<string>("output", "out", "where to write files.")
            .SetExecuteCommand(UnPackingAction);

        return root.WithSubCommand(command);
    }

    private static async Task UnPackingAction(ParseResult parseResult, CancellationToken ct = default)
    {
        var input = parseResult.GetValue<string>("--input");
        var output = parseResult.GetValue<string>("--output");
        bool missingParams = input == default || output == default;
        if (missingParams)
            throw new MissingParametersException("--input or --output is missing and required for packing.");
        else if (!File.Exists(input))
            throw new UnpackInputFileMissingException(input!);
        else if (!IsDirectoryPath(output!))
            throw new UnpackOutputDirectoryInvalidException(output!);
        else
            await UnpackAsync(input, output!);
    }


    public static async Task UnpackAsync(string input, string output)
    {
        var manifest = PluginUtilitiesExt.ReadManifestFromArchive(input);
        Console.Out.WriteLine($"Trying to Unpack {input}");
        await UnpackToLocation(manifest, input, output);
    }

    public static async Task UnpackToLocation(PluginManifestPayload manifest, string input, string outputFolder)
    {
        var targetExtraction = Path.Combine(outputFolder, manifest.Id);
        if (Directory.Exists(targetExtraction))
        {
            Console.Out.WriteLine($"Removing Existing {targetExtraction}".Cyan());
            Directory.Delete(targetExtraction);
        }

        Console.Out.WriteLine($"Unpacking {manifest.Id} to {targetExtraction}".Cyan());
        using var zip = await ZipFile.OpenReadAsync(input);

        foreach (var entry in zip.Entries)
        {

            // Skip directory entries
            if (string.IsNullOrEmpty(entry.Name))
                continue;
            // Build the full output path
            string outputPath = Path.Combine(targetExtraction, entry.FullName);

            // Ensure subdirectories exist
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            await entry.ExtractToFileAsync(outputPath);
            Console.Out.WriteLine($"Extracted to {outputPath}".Green());

        }
        Console.Out.WriteLine($"Package Created Finished Extracting to {targetExtraction}".Green());


    }

    private static async Task PackingAction(ParseResult parseResult, CancellationToken ct = default)
    {
        var input = parseResult.GetValue<string>("--input");
        var output = parseResult.GetValue<string>("--output");
        bool missingParams = input == default || output == default;
        if (missingParams)
            throw new MissingParametersException("--input or --output is missing and required for packing.");
        else if (!Directory.Exists(input))
            throw new PackInputDirectoryInvalidException(input!);
        else if (!IsDirectoryPath(output!))
            throw new PackOutputDirectoryInvalidException(output!);
        else
            await PackAsync(input, output!);
    }
    public static async Task PackAsync(string input, string output)
    {
        Console.Out.WriteLine($"Trying to Pack {input}");

        await PluginValidatorCommandExt.ValidatePackageAsync(input);
        var files = FilterArchiveFiles(input);
        await PackToFile(files, input, output);
    }

    public static List<string> FilterArchiveFiles(string inputFolder)
    {
        PackSettings.PopulateExclusionDlls();
        if (!Directory.Exists(inputFolder))
            throw new DirectoryNotFoundException(inputFolder);
        var basePath = Path.GetFullPath(inputFolder);
        var files = Directory.GetFiles(basePath, "*", SearchOption.AllDirectories);
        var outputFiles = new List<string>();
        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            // SKIP Excluded DLL those are host supplied.
            if (PackSettings.ExcludedDlls.Contains(name))
            {
                Console.Out.WriteLine($"Removed {file}.".Yellow());
                continue;
            }
            outputFiles.Add(file);
            Console.Out.WriteLine($"Allowed {file}.".Cyan());
        }
        return outputFiles;

    }

    public static PluginManifestPayload GetManifestInformation(string inputFolder)
    {
        if (!Directory.Exists(inputFolder))
            throw new DirectoryNotFoundException(inputFolder);
        LunaticPanel.Engine.Plugin.Entities.PluginScannedEntity? entity = PluginScannerExt.FindPluginDllInDirectory(inputFolder);
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

        var sdkVersion = typeof(PluginManifestPayload).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion!.Split('+')[0];
        Console.Out.WriteLine($"sdkVersion:{sdkVersion}".Magenta());
        Console.Out.WriteLine($"Dotnet Version:{PackSettings.DotNetVersion}".Magenta());
        var sdkVersionObj = new Version(sdkVersion!);

        entity.Loader.Unload();
        if (pluginId == default)
            throw new PluginIdNotFoundException(entity.Location);

        if (version == default)
            throw new PluginVersionNotFoundException(pluginId);

        if (description == default)
            throw new PluginDescriptionNotFoundException(pluginId);

        return new PluginManifestPayload()
        {
            Id = pluginId,
            Title = title ?? pluginId,
            Company = company,
            Copyright = copyright,
            Description = description,
            Version = version,
            PanelVersion = sdkVersionObj.Major.ToString(),
            DotnetVersion = PackSettings.DotNetVersion,
            PluginEntryFile = Path.GetFileName(entity.Location)
        };

    }

    public static string CreateManifestInformationFile(PluginManifestPayload manifest)
    {
        var tmp = Path.GetTempFileName();
        string json = JsonSerializer.Serialize(manifest, _jsonSerializerOptions);
        File.WriteAllText(tmp, json);
        return tmp;
    }

    public static async Task PackToFile(List<string> files, string inputFolder, string outputFolder)
    {
        if (!Directory.Exists(inputFolder))
            throw new DirectoryNotFoundException(inputFolder);
        Console.Out.WriteLine($"Build Manifest Information for {inputFolder}".Cyan());

        var pluginInfo = GetManifestInformation(inputFolder);
        Console.Out.WriteLine($"Manifest is ready for {pluginInfo.Id} v{pluginInfo.Version}".Green());
        string outputPackage = Path.Combine(outputFolder, $"{pluginInfo.Id}.{pluginInfo.Version}.lpkg");
        if (File.Exists(outputPackage))
            File.Delete(outputPackage);
        Console.Out.WriteLine($"Creating Temp Manifest File for {pluginInfo.Id}".Cyan());
        string manifestFileTmp = CreateManifestInformationFile(pluginInfo);
        Console.Out.WriteLine($"Temp Manifest File for {pluginInfo.Id} located {manifestFileTmp} is ready".Green());
        Console.Out.WriteLine($"Packing Archive for {pluginInfo.Id}".Cyan());
        Console.Out.WriteLine($"Output target set to {outputPackage}".Cyan());
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);
        using var zip = ZipFile.Open(outputPackage, ZipArchiveMode.Create);

        foreach (var file in files)
        {
            var fullPath = Path.GetFullPath(file);
            var relative = Path.GetRelativePath(inputFolder, fullPath);
            await zip.CreateEntryFromFileAsync(fullPath, relative, CompressionLevel.NoCompression);
            Console.Out.WriteLine($"Pack File {relative}.");

        }

        await zip.CreateEntryFromFileAsync(manifestFileTmp, "manifest.json", CompressionLevel.NoCompression);
        Console.Out.WriteLine($"Package Created at {outputPackage}".Green());

    }
    private static bool IsDirectoryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        char last = path[path.Length - 1];

        // Explicit directory indicator
        if (last == Path.DirectorySeparatorChar || last == Path.AltDirectorySeparatorChar)
            return true;

        // If last segment has no dot, assume directory
        string name = Path.GetFileName(path);
        return !name.Contains('.');
    }

}
