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
    public static IReadOnlyList<PluginScannedEntity> ScanAndFindPlugins(string locationForPlugins)
    {
        var results = new List<PluginScannedEntity>();
        var processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Console.WriteLine($"Plugin Root: {locationForPlugins}");
        foreach (var dir in Directory.EnumerateDirectories(locationForPlugins))
        {
            Console.Out.WriteLine($"Plugin Folder: {dir}");
            var entity = FindPluginDllInDirectory(dir, p => processed.Add(p));
            if (entity == default)
            {
                Console.Out.WriteLine($"No Plugin Found In: {dir}");
                continue;
            }
            results.Add(entity);
        }

        return results;
    }




    public static PluginScannedEntity? FindPluginDllInDirectory(string pluginFolder, Func<string, bool>? skipDll = default)
    {
        foreach (var dll in Directory.GetFiles(pluginFolder, "*.dll"))
        {

            if (skipDll != default && !skipDll(Path.GetFullPath(dll)))
                continue;
            var entity = LoadPluginInformation(dll);
            if (entity == default) continue;
            return entity;
        }
        return default;
    }

    public static PluginScannedEntity? LoadPluginInformation(string dllFile)
    {
        try
        {
            var tmpLoader = PluginLoader.CreateFromAssemblyFile(
                dllFile,
                sharedTypes: _sharedTypes,
                c => c.IsUnloadable = true
            );


            var assembly = tmpLoader.LoadDefaultAssembly();
            var pluginId = assembly.GetName().Name;
            var pluginEntryType = assembly.GetTypes()
                .SingleOrDefault(t => t.IsClass && !t.IsAbstract &&
                    typeof(IPlugin).IsAssignableFrom(t) && t.Namespace == pluginId);

            bool unloadAndSkip = false;
            if (pluginEntryType == default)
                unloadAndSkip = true;
            else if (typeof(IPlugin).IsAssignableFrom(pluginEntryType))
                unloadAndSkip = true;

            if (unloadAndSkip)
            {
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
            var permanentLoader = PluginLoader.CreateFromAssemblyFile(
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
            Console.Error.WriteLine(ex.Message);
        }
        return default;
    }

}
