using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Engine.Domain.Plugin.Entites;
using LunaticPanel.Engine.Plugin;
using System.Text.Json.Serialization;

namespace LunaticPanel.Engine.Web.Boostrap.Plugin;

internal record BootstrapPluginDescriptor
{
    public PluginEntity Entity { get; set; } = default!;

    [JsonIgnore]
    public IPlugin? EntryPoint { get; set; }

    [JsonIgnore]
    public IPluginLoader? Loader { get; set; }

    public string PluginDir { get; set; } = default!;
}
