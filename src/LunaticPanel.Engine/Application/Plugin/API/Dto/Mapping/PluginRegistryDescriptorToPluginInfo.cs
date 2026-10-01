using CoreMap;
using LunaticPanel.Engine.Application.Plugin.Models;

namespace LunaticPanel.Engine.Application.Plugin.API.Dto.Mapping;

internal class PluginRegistryDescriptorToPluginInfo : ICoreMapHandler<PluginRegistryDescriptorModel, PluginInfoResponse>
{
    public PluginInfoResponse Handler(PluginRegistryDescriptorModel data, ICoreMap alsoMap)
    => new(data.Plugin.Identity.PackageId, data.Plugin.Identity.PakageVersion, data.Plugin.Lifecycle.State, data.Plugin.Lifecycle.StartupState);
}
