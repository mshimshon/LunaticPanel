using McMaster.NETCore.Plugins;

namespace LunaticPanel.Engine.Web.Services.Plugin;

public record PluginScanResult(string PluginId,
    Version Version,
    PluginLoader Loader,
    Type PluginType, string Location);