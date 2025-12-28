using LunaticPanel.Core;
using LunaticPanel.Core.Messaging.Common;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.EventBus.Exceptions;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Core.Messaging.QuerySystem.Exceptions;
using McMaster.NETCore.Plugins;

namespace LunaticPanel.Engine.Services.Plugin;

public sealed class PluginScanner
{
    private readonly string _installedRoot;
    private readonly string[] _specificFiles;

    private static readonly Type[] _sharedTypes =
    {
        typeof(IPlugin),

        typeof(BusMessageData),
        typeof(BusMessageDataType),
        typeof(IBusMessage),

        typeof(IEventBus),
        typeof(EventBusIdAttribute),
        typeof(EventBusMessage),
        typeof(IEventBusHandler),
        typeof(IEventBusMessage),
        typeof(EventBusMessageException),
        typeof(EventBusNotFoundException),

        typeof(IEngineBus),
        typeof(BusIdAttribute),
        typeof(EngineBusMessage),
        typeof(EngineBusResponse),
        typeof(IEngineBusHandler),
        typeof(IEngineBusMessage),

        typeof(IQueryBus),
        typeof(IQueryBusHandler),
        typeof(IQueryBusMessage),
        typeof(QueryBusIdAttribute),
        typeof(QueryBusMessage),
        typeof(QueryBusMessageResponse),
        typeof(QueryBusMessageException),
        typeof(QueryBusMultipleHandlerException),
        typeof(QueryBusNotFoundException),
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

        foreach (var dll in EnumerateScannedDlls().Concat(EnumerateSpecificDlls()))
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

                var assembly = loader.LoadDefaultAssembly();

                var pluginTypes = assembly.GetTypes()
                    .Where(t =>
                        typeof(IPlugin).IsAssignableFrom(t) &&
                        !t.IsAbstract &&
                        !t.IsInterface)
                    .ToList();

                if (pluginTypes.Count == 0)
                    continue;

                if (pluginTypes.Count != 1)
                    throw new InvalidOperationException(
                        $"Assembly {dll} must define exactly one IPlugin");

                var pluginType = pluginTypes[0];
                var name = pluginType.Assembly.GetName();

                results.Add(new PluginScanResult(
                    PluginId: name.Name!,
                    Version: name.Version ?? new Version(1, 0, 0, 0),
                    Loader: loader,
                    PluginType: pluginType
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        return results;
    }

    private IEnumerable<string> EnumerateScannedDlls()
    {
        if (!Directory.Exists(_installedRoot))
            yield break;

        foreach (var pluginDir in Directory.GetDirectories(_installedRoot))
        {
            foreach (var versionDir in Directory.GetDirectories(pluginDir))
            {
                foreach (var dll in Directory.GetFiles(versionDir, "*.dll"))
                    yield return dll;
            }
        }
    }

    private IEnumerable<string> EnumerateSpecificDlls()
    {
        return from file in _specificFiles!
               where File.Exists(file)
               select file;
    }
}
