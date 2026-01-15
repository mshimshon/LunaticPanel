using LunaticPanel.Core.Abstraction;
using LunaticPanel.Engine.Domain.Plugin.Entites;
using McMaster.NETCore.Plugins;
using System.Text.Json.Serialization;

namespace LunaticPanel.Engine.Web.Boostrap.Plugin;

internal record BootstrapPluginDescriptor
{
    public PluginEntity Entity { get; set; } = default!;

    [JsonIgnore]
    public IPlugin? EntryPoint { get; set; }

    [JsonIgnore]
    public PluginLoader? Loader { get; set; }

    public string PluginDir { get; set; } = default!;
}
