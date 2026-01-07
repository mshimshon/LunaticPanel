using LunaticPanel.Core.Abstraction;
using McMaster.NETCore.Plugins;

namespace LunaticPanel.Engine.Web.Services.Plugin;

public sealed class PluginScanner
{
    private readonly string _installedRoot;
    private readonly string[] _specificFiles;

    private static readonly Type[] _sharedTypes =
    {
        typeof(IPlugin)
    };
    public PluginScanner(string installedRoot, params string[] specificFiles)
    {
        _installedRoot = installedRoot;
        _specificFiles = specificFiles ?? Array.Empty<string>();
    }

    public IReadOnlyList<PluginScanResult> Scan()
    {
        var results = new List<PluginScanResult>();
        var processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var dll in Directory.GetFiles(_installedRoot, "*.dll"))
        {
            if (!processed.Add(Path.GetFullPath(dll)))
                continue;
            try
            {
                var loader = PluginLoader.CreateFromAssemblyFile(
                    dll,
                    sharedTypes: _sharedTypes,
                    c => c.IsUnloadable = true
                );
                string dllNamespaceGuess = Path.GetFileNameWithoutExtension(dll);

                var assembly = loader.LoadDefaultAssembly();
                var pluginEntryType = assembly.GetType($"{dllNamespaceGuess}.PluginEntry");

                if (pluginEntryType == default)
                    continue;
                bool supports = typeof(IPlugin).IsAssignableFrom(pluginEntryType);
                if (!supports)
                    continue;
                var name = pluginEntryType.Assembly.GetName();
                results.Add(new PluginScanResult(
                    PluginId: name.Name!,
                    Version: name.Version ?? new Version(1, 0, 0, 0),
                    Loader: loader,
                    PluginType: pluginEntryType, Location: dll
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        return results;
    }


}
