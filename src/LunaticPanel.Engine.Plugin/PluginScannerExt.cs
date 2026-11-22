using LunaticPanel.Core.PluginValidator;

using McMaster.NETCore.Plugins;
using NuGet.Versioning;
using System.ComponentModel;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

namespace LunaticPanel.Engine.Plugin;

public static class PluginScannerExt
{
    internal static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };
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
                if (LibraryValidatorExt.ContainsIPlugin(dll))
                    return dll;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"'{dll}' -> {ex.Message}");
                continue;
            }

        }
        return default;
    }

    /// <summary>
    /// Specify plugin runtime root, it will read manifest and load up the plugin reference safely or throw.
    /// </summary>
    public static IPluginReference LoadPLugin(string pluginRoot, Type[] sharedType, AssemblyName[] sharedAssemblies)
    {
        PluginLoader? permanentLoader = default;
        try
        {

            var manifestFile = Path.Combine(pluginRoot, "manifest.json");
            var manifest = ParseManifest(File.ReadAllText(manifestFile));
            var entryFile = Path.Combine(pluginRoot, manifest.PluginEntryFile);
            var entryClass = manifest.EntryClass;
            permanentLoader = PluginLoader.CreateFromAssemblyFile(entryFile, sharedTypes: sharedType,
               c =>
               {
                   c.IsUnloadable = true;
                   foreach (var item in sharedAssemblies)
                       c.SharedAssemblies.Add(item);
               }
           );
            var pluginLoaderInfo = new PluginReferenceEntity(manifest, permanentLoader);
            pluginLoaderInfo.Load();
            // Perform Internal Blazor Validation
            var validationResult = pluginLoaderInfo.Entry.PerformValidation();
            if (validationResult.Any(p => !p.Passed))
            {
                foreach (var error in validationResult.Where(p => !p.Passed).SelectMany(p => p.Errors!))
                    System.Console.WriteLine($"[ERROR]::{error.Origin} :: {error.Message}");
                throw new Exception("Plugin Internal Validation Failure, See log.");
            }
            return pluginLoaderInfo;
        }
        catch (Exception)
        {
            if (permanentLoader != default && permanentLoader.IsUnloadable)
            {
                permanentLoader.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            throw;
        }
        finally
        {



        }
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
            var version = NuGetVersion.Parse(pluginEntryType!.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
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
                Version: version ?? NuGetVersion.Parse("1.0.0"),
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

    public static PluginManifestEntity ParseManifest(string json)
    {
        var result = JsonSerializer.Deserialize<PluginManifestEntity>(json, _jsonSerializerOptions);
        if (result == null)
            throw new FileNotFoundException($"manifest.json corrupted in '{json}'");
        return result;
    }

    public static PluginManifestEntity ReadManifestFromPackage(string path)
    {
        using var archive = ZipFile.OpenRead(path);

        var entry = archive.GetEntry("manifest.json");
        if (entry == null)
            throw new FileNotFoundException($"manifest.json not found in {path}");

        using (var stream = entry.Open())
        {
            using (var reader = new StreamReader(stream))
            {
                var manifest = ParseManifest(reader.ReadToEnd());
                manifest.PackageFileLocation = path;
                return manifest;
            }
        }

    }

    public static void ExtractPackage(string zipPath, string targetRoot)
    {
        if (!Directory.Exists(targetRoot))
            Directory.CreateDirectory(targetRoot);
        using (var zip = ZipFile.OpenRead(zipPath))
        {
            foreach (var entry in zip.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name))
                    continue;
                string outputPath = Path.Combine(targetRoot, entry.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                using (var input = entry.Open())
                {
                    using (var output = File.Create(outputPath))
                    {
                        input.CopyTo(output);
                    }
                }

            }
        }
    }
}


