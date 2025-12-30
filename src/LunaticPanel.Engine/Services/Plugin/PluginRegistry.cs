namespace LunaticPanel.Engine.Services.Plugin;

internal class PluginRegistry
{
    private static readonly ICollection<PluginRegistryDescriptor> _plugins = new List<PluginRegistryDescriptor>();

    private static readonly Object _lock = new();

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
