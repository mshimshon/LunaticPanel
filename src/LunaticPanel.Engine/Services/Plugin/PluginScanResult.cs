using McMaster.NETCore.Plugins;

namespace LunaticPanel.Engine.Services.Plugin;

public record PluginScanResult(string PluginId,
    Version Version,
    PluginLoader Loader,
    Type PluginType);