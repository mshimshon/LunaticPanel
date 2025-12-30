using LunaticPanel.Core;
using LunaticPanel.Engine.Domain.Plugin.Entites;
using McMaster.NETCore.Plugins;
using System.Text.Json.Serialization;

namespace LunaticPanel.Engine.Presentation.Boostrap;

internal record BootstrapPluginDescriptor
{
    public PluginEntity Entity { get; set; } = default!;

    [JsonIgnore]
    public IPlugin? EntryPoint { get; set; }

    [JsonIgnore]
    public Type EntryPointType { get; set; } = default!;

    [JsonIgnore]
    public PluginLoader? Loader { get; set; }
}
