namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;

public enum ExternalPluginEntityLifecycleStartupState
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
