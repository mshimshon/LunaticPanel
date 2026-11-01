using System;

namespace LunaticPanel.Engine.Web.Services;

public interface IPanelUpdateService
{
    Version? Available { get; }
    Version? Downloaded { get; }
    Version? Install { get; }

    /// <summary>
    /// True when apply folder does not contain all plugin panel compatible with the next update...
    /// </summary>
    bool AwaitingForPluginCompatibilityUpdate { get; }

    Task InstallUpdateAsync(CancellationToken ct = default);
    Task DownloadUpdateAsync(CancellationToken ct = default);
    Task CheckPluginsAsync(CancellationToken ct = default);

}
