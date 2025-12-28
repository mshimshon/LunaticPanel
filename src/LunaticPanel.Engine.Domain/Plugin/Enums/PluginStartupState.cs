namespace LunaticPanel.Engine.Domain.Plugin.Enums;

public enum PluginStartupState
{
    /// <summary>
    /// Plugin will be loaded at boot up
    /// </summary>
    Enabled = 1,

    /// <summary>
    /// Plugin will not be loaded at boot up
    /// </summary>
    Disabled = 0
}
