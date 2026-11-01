using System;

namespace LunaticPanel.Engine.Web.Services.UpdateService;

internal class PanelUpdateService : IPanelUpdateService
{
    public Version? Available { get; set; }

    public Version? Downloaded { get; set; }

    public Version? Install { get; set; }

    public bool AwaitingForPluginCompatibilityUpdate { get; set; }

    public Task CheckPluginsAsync(Version panelVersion, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task DownloadUpdateAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task InstallUpdateAsync(Version panelVersion, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
