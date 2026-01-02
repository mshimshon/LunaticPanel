using LunaticPanel.Engine.Application.Plugin.Models;

namespace LunaticPanel.Engine.Application.Plugin;

public interface IPluginRegistry
{
    void Register(PluginRegistryDescriptorModel item);
    PluginRegistryDescriptorModel GetByEntryType(Type plugin);
    IReadOnlyCollection<PluginRegistryDescriptorModel> GetAll();
}
