using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Application.Plugin.Models;

namespace LunaticPanel.Engine.Infrastructure.Plugin;

public class PluginRegistry : IPluginRegistry
{
    private static readonly ICollection<PluginRegistryDescriptorModel> _plugins = new List<PluginRegistryDescriptorModel>();

    private static readonly Object _lock = new();

    public void Register(PluginRegistryDescriptorModel item)
    {
        lock (_lock)
        {
            if (!_plugins.Contains(item))
                _plugins.Add(item);
        }
    }


    public PluginRegistryDescriptorModel GetByEntryType(Type plugin)
    {
        lock (_lock)
        {
            return _plugins.Single(p => p.EntryType == plugin);
        }
    }

    public IReadOnlyCollection<PluginRegistryDescriptorModel> GetAll()
    {
        lock (_lock)
        {
            return _plugins.ToList().AsReadOnly();
        }
    }
}
