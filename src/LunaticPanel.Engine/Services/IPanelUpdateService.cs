using LunaticPanel.Engine.Plugin;

using NuGet.Versioning;
using System;

namespace LunaticPanel.Engine.Web.Services;

public interface IPanelUpdateService
{
    NuGetVersion? Downloaded { get; }

    NuGetVersion? Install { get; }

    NuGetVersion? Installing { get; }
    NuGetVersion? Staged { get; }
    Dictionary<string, PluginManifestEntity>? PluginUpgradeDownloaded { get; }
    List<string> PreInstalled { get; }
    bool BlockedByPlugin { get; }

    /// <summary>
    /// True when apply folder does not contain all plugin panel compatible with the next update...
    /// </summary>
    bool AwaitingForPluginCompatibilityUpdate { get; }

    Task<NuGetVersion> GetLatestVersion(CancellationToken ct = default);
    Task InstallUpdateAsync(CancellationToken ct = default);
    Task DownloadUpdateAsync(CancellationToken ct = default);
    Task CheckPluginsAsync(CancellationToken ct = default);

}
