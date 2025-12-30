using LunaticPanel.Engine.Application.Plugin.Contracts;

namespace LunaticPanel.Engine.Application.Plugin;

public interface IPluginRegistry
{
    void Register(PluginRegistryDescriptor item);
    PluginRegistryDescriptor GetByEntryType(Type plugin);
    IReadOnlyCollection<PluginRegistryDescriptor> GetAll();
}
