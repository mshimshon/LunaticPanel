using LunaticPanel.Core;
using LunaticPanel.Engine.Domain.Plugin.Entites;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Application.Plugin;

public sealed record PluginRegistryDescriptor(
        Type EntryType,
        IPlugin Entry,
        PluginEntity Plugin,
        ServiceCollection Services
    )
{

}
