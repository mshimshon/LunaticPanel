using LunaticPanel.Core.Abstraction;
using McMaster.NETCore.Plugins;

namespace LunaticPanel.Engine.Web.Boostrap.Plugin;

public sealed class BootstrapPluginScanner
{
    private readonly string _installedRoot;
    private readonly string[] _specificFiles;

    private static readonly Type[] _sharedTypes =
    {
        typeof(IPlugin)
    };
    public BootstrapPluginScanner(string installedRoot, params string[] specificFiles)
    {
        _installedRoot = installedRoot;
        _specificFiles = specificFiles ?? Array.Empty<string>();
    }

    public IReadOnlyList<BootstrapPluginScanResult> Scan()
    {
        var results = new List<BootstrapPluginScanResult>();
        var processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var dir in Directory.EnumerateDirectories(_installedRoot))
            foreach (var dll in Directory.GetFiles(dir, "*.dll"))
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
                    results.Add(new BootstrapPluginScanResult(
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
