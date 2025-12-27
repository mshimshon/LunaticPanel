using LunaticPanel.Core;
using LunaticPanel.Engine.Domain.Plugin.Entites;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Services.Plugin;

public sealed record PluginItem(
        Type EntryType,
        IPlugin Entry,
        PluginEntity Plugin,
        ServiceCollection Services
    )
{

}
