using CoreMap;
using LunaticPanel.Engine.Application.Plugin.Contracts;

namespace LunaticPanel.Engine.Application.Plugin.API.Dto.Mapping;

internal class PluginRegistryDescriptorToPluginInfo : ICoreMapHandler<PluginRegistryDescriptor, PluginInfoDto>
{
    public PluginInfoDto Handler(PluginRegistryDescriptor data, ICoreMap alsoMap)
    => new(data.Plugin.Identity.PackageId, data.Plugin.Identity.PakageVersion, data.Plugin.Lifecycle.State, data.Plugin.Lifecycle.StartupState);
}
