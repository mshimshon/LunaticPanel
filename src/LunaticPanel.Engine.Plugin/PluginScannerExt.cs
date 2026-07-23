using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Engine.Plugin.Entities;
using McMaster.NETCore.Plugins;

namespace LunaticPanel.Engine.Plugin;

public static class PluginScannerExt
{
    private static readonly Type[] _sharedTypes =
    {
        typeof(IPlugin)
    };
    public static IReadOnlyList<PluginScannedEntity> ScanAndFindPlugins(string locationForPlugins, Type[] sharedType)
    {
        var results = new List<PluginScannedEntity>();
        var processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Console.WriteLine($"Plugin Root: {locationForPlugins}");
        foreach (var dir in Directory.EnumerateDirectories(locationForPlugins))
        {
            var entity = FindPluginDllInDirectory(dir, sharedType, p => processed.Add(p));
            if (entity == default)
            {
                Console.Out.WriteLine($"No Plugin Found In: {dir}");
                continue;
            }
            Console.Out.WriteLine($"Plugin Folder: {dir}");
            results.Add(entity);
        }

        return results;
    }

    public static PluginScannedEntity? FindPluginDllInDirectory(string pluginFolder, Type[] sharedType, Func<string, bool>? skipDll = default)
    {
        Console.Out.WriteLine($"[Search for Plugin File]");
        foreach (var dll in Directory.GetFiles(pluginFolder, "*.dll", SearchOption.TopDirectoryOnly))
        {
            Console.Out.WriteLine($"{dll}");
            if (skipDll != default && !skipDll(Path.GetFullPath(dll)))
                continue;

            var entity = LoadPluginInformation(dll, sharedType);
            if (entity == default) continue;
            return entity;
        }
        return default;
    }

    public static PluginScannedEntity? LoadPluginInformation(string dllFile, Type[] sharedType)
    {
        PluginLoader? tmpLoader = default;
        PluginLoader? permanentLoader = default;
        try
        {
            tmpLoader = PluginLoader.CreateFromAssemblyFile(
                dllFile,
                sharedTypes: [.. _sharedTypes, .. sharedType],
                c => c.IsUnloadable = true
            );


            var assembly = tmpLoader.LoadDefaultAssembly();
            var pluginId = assembly.GetName().Name;
            Console.Out.WriteLine($"Testing Dll for {pluginId}");
            var pluginEntryType = assembly.GetTypes()
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract &&
                    typeof(IPlugin).IsAssignableFrom(t) && t.Namespace == pluginId);

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
                sharedTypes: _sharedTypes,
                c => c.IsUnloadable = true
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
