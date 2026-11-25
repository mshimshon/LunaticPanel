using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Pulses.States;
using NuGet.Versioning;
using System;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface IPanelUpdateInformationViewModel : IWidgetViewModel
{
    PanelUpdateState PanelUpdateState { get; }
    NuGetVersion? UpdateFound { get; }
    NuGetVersion? StagedVersion { get; }
    NuGetVersion? DownloadedVersion { get; }
    string AvailableVersion { get; }
    bool HasUpdateAvailable { get; }

}
