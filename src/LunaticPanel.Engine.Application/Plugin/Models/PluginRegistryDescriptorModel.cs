using LunaticPanel.Core;
using LunaticPanel.Engine.Domain.Plugin.Entites;

namespace LunaticPanel.Engine.Application.Plugin.Models;

public sealed record PluginRegistryDescriptorModel(
        Type EntryType,
        IPlugin Entry,
        PluginEntity Plugin
    )
{
}
