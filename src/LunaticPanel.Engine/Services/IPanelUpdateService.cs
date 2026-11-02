using LunaticPanel.Engine.Web.Boostrap.Plugin;
using System;

namespace LunaticPanel.Engine.Web.Services;

public interface IPanelUpdateService
{
    Version? Downloaded { get; }

    Version? Install { get; }

    Version? Installing { get; }
    Version? Staged { get; }
    Dictionary<string, BootstrapPluginManifest>? PluginUpgradeDownloaded { get; }
    List<string> PreInstalled { get; }
    bool BlockedByPlugin { get; }

    /// <summary>
    /// True when apply folder does not contain all plugin panel compatible with the next update...
    /// </summary>
    bool AwaitingForPluginCompatibilityUpdate { get; }

    Task InstallUpdateAsync(CancellationToken ct = default);
    Task DownloadUpdateAsync(CancellationToken ct = default);
    Task CheckPluginsAsync(CancellationToken ct = default);

}
