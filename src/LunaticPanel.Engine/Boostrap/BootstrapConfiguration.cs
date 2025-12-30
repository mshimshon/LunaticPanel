using System.Text.Json.Serialization;

namespace LunaticPanel.Engine.Presentation.Boostrap;

internal record BootstrapConfiguration
{
    public List<BootstrapPluginDescriptor> KnownPlugins { get; set; } = new List<BootstrapPluginDescriptor>();

    [JsonIgnore]
    public List<BootstrapPluginDescriptor> ActivePlugins { get; set; } = new List<BootstrapPluginDescriptor>();



}
