using NuGet.Versioning;

namespace LunaticPanel.Engine.Plugin;

public record PluginScannedEntity(
    string PluginId,
    NuGetVersion Version,
    IPluginLoader Loader,
    string PluginEntryLocationType,
    string Location);
