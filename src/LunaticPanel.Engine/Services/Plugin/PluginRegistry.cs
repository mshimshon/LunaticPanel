namespace LunaticPanel.Engine.Services.Plugin;

internal class PluginRegistry
{
    private static readonly ICollection<PluginItem> _plugins = new List<PluginItem>();

    private static readonly Object _lock = new();

    public void Register(PluginItem item)
    {
        lock (_lock)
        {
            if (!_plugins.Contains(item))
                _plugins.Add(item);
        }
    }


    public PluginItem GetByEntryType(Type plugin)
    {
        lock (_lock)
        {
            return _plugins.Single(p => p.EntryType == plugin);
        }
    }


}
