using NuGet.Versioning;

namespace LunaticPanel.Engine.Plugin.Entities;

public record PluginScannedEntity(
    string PluginId,
    NuGetVersion Version,
    IPluginLoader Loader,
    string PluginEntryLocationType,
    string Location);
