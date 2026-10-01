using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Engine.Domain.Plugin.Entites;

namespace LunaticPanel.Engine.Application.Plugin.Models;

public sealed record PluginRegistryDescriptorModel(
        IPlugin Entry,
        PluginEntity Plugin
    )
{
}
