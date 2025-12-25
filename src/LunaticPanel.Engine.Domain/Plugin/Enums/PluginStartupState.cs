namespace GameServerManager.Engine.Domain.Plugin.Enums;

public enum PluginStartupState
{
    /// <summary>
    /// Plugin will be loaded at boot up
    /// </summary>
    Enabled,

    /// <summary>
    /// Plugin will not be loaded at boot up
    /// </summary>
    Disabled
}
