using LunaticPanel.Core;
using LunaticPanel.Engine.Domain.Plugin.Entites;

namespace LunaticPanel.Engine.Services.Plugin;

public sealed record PluginRegistryDescriptor(
        Type EntryType,
        IPlugin Entry,
        PluginEntity Plugin,
        ServiceCollection Services
    )
{

}
