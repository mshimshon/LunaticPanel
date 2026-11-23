using NuGet.Versioning;
using System;

namespace LunaticPanel.Engine.Bootstraper;

public interface IUpdateDiscovery
{
    string UpgradeMarkerFile { get; }
    string UpgradePanelFile { get; }
    string FailSafePanelFile { get; }
    string FailSafePluginFile { get; }
    string AccessBinMark { get; }
    string AccessUpgradeRootAccessMark { get; }
    string AccessPluginRootMark { get; }
    NuGetVersion? UpgradeTo { get; }
    bool CanUpgrade();
    Task StartUpgrade();
    Task ApplyUpgrade();
    Task BackUpCurrentPanel();
    Task FailSafeRestoring();
    Task ClearStageAsync();
    void LoadVersionFile();
}
