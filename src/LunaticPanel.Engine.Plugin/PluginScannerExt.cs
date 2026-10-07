using LunaticPanel.Core.PluginValidator;
using LunaticPanel.Engine.Plugin.Entities;
using McMaster.NETCore.Plugins;
using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

public static class PluginScannerExt
{

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
