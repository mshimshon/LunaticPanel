using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Engine.Plugin.Entities;
using LunaticPanel.Engine.Plugin.Exceptions;
using McMaster.NETCore.Plugins;
using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

public static class PluginScannerExt
{
    public static bool IsPluginDllValid(string dll)
    {
        try
        {
            RunPluginDllValidator(dll);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }

    private static void MatchReferenceAssemblyVersion(string dll, string master, string match, bool throwOnNotFound)
    {
        Version? coreVersionMaster = DotnetInspectorExt.GetReferencedAssemblyVersion(dll, master);
        if (coreVersionMaster == default)
            throw new HostCodedException("MasterReferenceNotFound", $"{master} not a referenced assembly.");
        Version? corePluginVersion = DotnetInspectorExt.GetReferencedAssemblyVersion(dll, match);
        if (corePluginVersion == default)
        {

            Console.WriteLine($"Plugin does not have references to {match}.");
            if (throwOnNotFound)
                throw new HostCodedException("CoreReferenceNotFound", $"{match} not a referenced assembly but required.");
            else return;
        }
        if (coreVersionMaster != corePluginVersion)
            throw new HostCodedException("CoreReferenceVersionMisMatch", $"{master} v{coreVersionMaster} and {match} v{corePluginVersion} different versions.");

        Console.WriteLine($"{master} v{coreVersionMaster} and {match} v{corePluginVersion} version aligned.");
    }

    public static void RunPluginDllValidator(string dll)
    {
        Console.WriteLine("RunPluginDllValidator");
        var meta = DotnetInspectorExt.ExtractMetadata(dll);

        var pluginId = meta[ManifestMeta.Id];
        if (pluginId == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Id.ToString(), $"'{dll}' no ID found.");
        var description = meta[ManifestMeta.Description];
        if (description == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Description.ToString(), $"'{dll}' no description found.");
        var company = meta[ManifestMeta.Company];
        if (company == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Company.ToString(), $"'{dll}' company not found.");
        var version = meta[ManifestMeta.Version]?.Split('+')[0];
        if (version == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Version.ToString(), $"'{dll}' version tag not found.");
        string[] versionSplit = version.Split('.');
        if (versionSplit.Length != 3)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.Version.ToString(), $"'{dll}' version tag '{version}' doesn't respect strict 'major.minor.patch' format.");

        var asmVersion = meta[ManifestMeta.AssemblyVersion]?.Split('+')[0];
        if (asmVersion == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.AssemblyVersion.ToString(), $"'{dll}' AssemblyVersion tag not found.");
        string[] asmVersionSplit = asmVersion.Split('.');
        if (asmVersionSplit.Length != 4 || asmVersionSplit[3] != "0")
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.AssemblyVersion.ToString(), $"'{dll}' AssemblyVersion tag '{asmVersion}' doesn't respect strict 'major.minor.patch' format.");

        var fileVersion = meta[ManifestMeta.FileVersion]?.Split('+')[0];
        if (fileVersion == default)
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.FileVersion.ToString(), $"'{dll}' AssemblyFileVersion tag not found.");
        string[] fileVersionSplit = fileVersion.Split('.');
        if (fileVersionSplit.Length != 4 || fileVersionSplit[3] != "0")
            throw new PluginMetadataExtractionMalformedException(ManifestMeta.FileVersion.ToString(), $"'{dll}' AssemblyFileVersion tag '{fileVersion}' doesn't respect strict 'major.minor.patch' format.");

        if (versionSplit[0] != asmVersionSplit[0] || versionSplit[1] != asmVersionSplit[1] || versionSplit[2] != asmVersionSplit[2])
            throw new PluginMetadataExtractionMalformedException("", $"'{dll}' {asmVersion} != {version} Assembly Version must equal Version (without +hash).");
        if (versionSplit[0] != fileVersionSplit[0] || versionSplit[1] != fileVersionSplit[1] || versionSplit[2] != fileVersionSplit[2])
            throw new PluginMetadataExtractionMalformedException("", $"'{dll}' {asmVersion} != {version} AssemblyFileVersion must equal Version (without +hash).");
        int pluginEntryImplementations = DotnetInspectorExt.CountIPluginImplementations(dll);
        if (pluginEntryImplementations <= 0)
            throw new PluginEntryViolationException("No Plugin Entry Found.");
        if (pluginEntryImplementations > 1)
            throw new PluginEntryViolationException("Only one plugin entry is allowed.");

        foreach (var item in DependencySettings.OptionalCoreAssemblies)
            MatchReferenceAssemblyVersion(dll, "LunaticPanel.Core.Abstraction", item.Name!, false);

        Version? corePluginVersion = DotnetInspectorExt.GetReferencedAssemblyVersion(dll, "LunaticPanel.Core.Abstraction");
        if (corePluginVersion == default)
            throw new PluginCoreVersionFailedException("Failed to extract core plugin version.");
        var currentCoreVersion = typeof(IPlugin).Assembly.GetName().Version;
        if (currentCoreVersion == default)
            throw new PluginCoreVersionFailedException("Failed to extract core current version.");
        if (currentCoreVersion.Major != corePluginVersion.Major)
            throw new PluginCoreVersionFailedException($"Plugin was compiled with panel v{corePluginVersion.Major} and the current panel version is v{currentCoreVersion.Major}.");
        if (currentCoreVersion < corePluginVersion)
            throw new PluginCoreVersionFailedException("Plugin was compiled with greater core version, panel is outdated for this plugin.");


        Console.WriteLine("Validator Success");

    }
    public static IReadOnlyList<string> ScanAndFindPlugins(string locationForPlugins, Type[] sharedType, AssemblyName[] sharedAssemblies)
    {
        var results = new List<string>();
        var processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Console.WriteLine($"Plugin Root: {locationForPlugins}");
        foreach (var dir in Directory.EnumerateDirectories(locationForPlugins))
        {
            var dll = FindPluginDllInDirectory(dir, sharedType, sharedAssemblies, p => processed.Add(p));
            if (dll == default)
            {
                Console.Out.WriteLine($"No Plugin Found In: {dir}");
                continue;
            }
            Console.Out.WriteLine($"Plugin Folder: {dir}");
            results.Add(dll);
        }

        return results;
    }

    public static string? FindPluginDllInDirectory(string pluginFolder, Type[] sharedType, AssemblyName[] sharedAssemblies, Func<string, bool>? skipDll = default)
    {
        Console.Out.WriteLine($"[Search for Plugin File]");
        foreach (var dll in Directory.GetFiles(pluginFolder, "*.dll", SearchOption.TopDirectoryOnly))
        {
            Console.Out.WriteLine($"{dll}");
            if (skipDll != default && !skipDll(Path.GetFullPath(dll)))
                continue;
            try
            {
                if (DotnetInspectorExt.ContainsIPlugin(dll))
                    return dll;
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine($"'{dll}' not .NET valid.");
                continue;
            }

        }
        return default;
    }

    public static PluginScannedEntity? LoadPluginInformation(string dllFile, Type[] sharedType, AssemblyName[] sharedAssemblies)
    {
        PluginLoader? tmpLoader = default;
        PluginLoader? permanentLoader = default;
        try
        {


            tmpLoader = PluginLoader.CreateFromAssemblyFile(
                dllFile,
                sharedTypes: sharedType,
                c =>
                {
                    c.IsUnloadable = true;
                    foreach (var item in sharedAssemblies)
                        c.SharedAssemblies.Add(item);
                }
            );


            var assembly = tmpLoader.LoadDefaultAssembly();
            var pluginId = assembly.GetName().Name;
            Console.Out.WriteLine($"Testing Dll for {pluginId}");
            var pluginEntryType = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.Namespace == pluginId &&
                    t.GetInterfaces().Any(i => string.Equals(i.FullName, "LunaticPanel.Core.Abstraction.Plugin.IPlugin", StringComparison.Ordinal)));


            bool unloadAndSkip = false;
            if (pluginEntryType == default)
                unloadAndSkip = true;

            if (unloadAndSkip)
            {
                Console.Out.WriteLine($"{pluginId} not a plugin, unloading.");

                tmpLoader.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return default;
            }
            Console.Out.WriteLine($"Found Plugin: {dllFile}");

            var asmName = pluginEntryType!.Assembly.GetName().Name!;
            var entryPointTypeName = pluginEntryType.FullName!;
            var asmVersion = pluginEntryType!.Assembly.GetName().Version;

            Version? version = default;
            if (asmVersion != default)
                version = new(asmVersion.Major, asmVersion.Minor, asmVersion.Build);
            tmpLoader.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            permanentLoader = PluginLoader.CreateFromAssemblyFile(
                dllFile,
                sharedTypes: sharedType,
                c =>
                {
                    c.IsUnloadable = true;
                    foreach (var item in sharedAssemblies)
                        c.SharedAssemblies.Add(item);
                }
            );
            return new PluginScannedEntity(PluginId: asmName,
                Version: asmVersion ?? new Version(1, 0, 0, 0),
                Loader: new PluginLoaderInfo(permanentLoader),
                PluginEntryLocationType: entryPointTypeName, Location: dllFile);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Testing Plugin Failed with {ex.Message}");
        }
        finally
        {
            if (tmpLoader != default && tmpLoader.IsUnloadable)
            {
                tmpLoader.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }


        }
        return default;
    }

}
