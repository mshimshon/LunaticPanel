using McMaster.NETCore.Plugins;

namespace LunaticPanel.Engine.Web.Boostrap.Plugin;

public record BootstrapPluginScanResult(string PluginId,
    Version Version,
    PluginLoader Loader,
    Type PluginType, string Location);