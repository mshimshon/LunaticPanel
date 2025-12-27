namespace LunaticPanel.Engine.Domain.Plugin.Enums;

public enum PluginState
{
    /// <summary>
    /// Was previously discovered, now it's no longer available.
    /// </summary>
    Missing,
    /// <summary>
    /// Found the files.
    /// </summary>
    Discovered,
    /// <summary>
    /// The plugin's assembly was loaded.
    /// </summary>
    Validated,
    /// <summary>
    /// Plugin assembly was loaded and its services registered.
    /// </summary>
    Loaded,
    /// <summary>
    /// the initialized was executed, plugin is actively loaded.
    /// </summary>
    Active,
    /// <summary>
    /// Plugin failed during the loading phase.
    /// </summary>
    Failed,
    /// <summary>
    /// The plugin is currently not loaded
    /// </summary>
    Unloaded
}
