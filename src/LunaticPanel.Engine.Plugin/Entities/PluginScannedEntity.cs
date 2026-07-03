namespace LunaticPanel.Engine.Plugin.Entities;

public record PluginScannedEntity(
    string PluginId,
    Version Version,
    IPluginLoader Loader,
    string PluginEntryLocationType,
    string Location);