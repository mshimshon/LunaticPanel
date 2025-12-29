using LunaticPanel.Core;

namespace LunaticPanel.Engine.Services.Plugin;

internal class PluginRegistry
{
    private static readonly ICollection<PluginRegistryDescriptor> _plugins = new List<PluginRegistryDescriptor>();
    private static readonly IDictionary<IPlugin, IServiceProvider> _rootProviders = new Dictionary<IPlugin, IServiceProvider>();

    private static readonly Object _lock = new();
    public void AddRootProvider(IPlugin plugin, IServiceProvider serviceProvider)
    {
        lock (_lock)
        {
            if (!_rootProviders.ContainsKey(plugin))
                _rootProviders.Add(plugin, serviceProvider);
        }
    }

    public IServiceProvider? GetRootProvider(IPlugin plugin)
    {
        lock (_lock)
        {
            if (!_rootProviders.ContainsKey(plugin)) return default;
            return _rootProviders[plugin];
        }
    }
    public void Register(PluginRegistryDescriptor item)
    {
        lock (_lock)
        {
            if (!_plugins.Contains(item))
                _plugins.Add(item);
        }
    }


    public PluginRegistryDescriptor GetByEntryType(Type plugin)
    {
        lock (_lock)
        {
            return _plugins.Single(p => p.EntryType == plugin);
        }
    }


}
