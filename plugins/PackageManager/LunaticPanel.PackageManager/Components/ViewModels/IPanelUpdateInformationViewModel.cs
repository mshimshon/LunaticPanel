using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Pulses.States;
using NuGet.Versioning;
using System;

namespace LunaticPanel.PackageManager.Components.ViewModels;

public interface IPanelUpdateInformationViewModel : IWidgetViewModel
{
    PanelUpdateState PanelUpdateState { get; }
    NuGetVersion? Version { get; }
    bool HasUpdateAvailable { get; }
    public bool HasDownloaded { get; }
    public bool HasStaged { get; }
}
