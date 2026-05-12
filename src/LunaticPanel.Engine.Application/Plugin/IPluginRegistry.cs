using LunaticPanel.Engine.Application.Plugin.Models;

namespace LunaticPanel.Engine.Application.Plugin;

public interface IPluginRegistry
{
    void Register(PluginRegistryDescriptorModel item);
    PluginRegistryDescriptorModel GetById(string Id);
    IReadOnlyCollection<PluginRegistryDescriptorModel> GetAll();
}
