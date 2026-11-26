using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Components.ViewModels;
using LunaticPanel.PackageManager.Pulses.States;
using NuGet.Versioning;
using StatePulse.Net;
using System;

namespace LunaticPanel.PackageManager.Components;

public class PanelUpdateInformationViewModel : WidgetViewModelBase, IPanelUpdateInformationViewModel
{
    private readonly IStatePulse _statePulse;
    public PanelUpdateState PanelUpdateState => _statePulse.StateOf<PanelUpdateState>(() => this, UpdateChanges);
    public NuGetVersion? UpdateFound => PanelUpdateState.AvailableVersion;
    public NuGetVersion? StagedVersion => PanelUpdateState.StagedVersion;
    public NuGetVersion? DownloadedVersion => PanelUpdateState.DownloadedVersion;
    public string AvailableVersion => UpdateFound?.ToString() ?? StagedVersion?.ToString() ?? DownloadedVersion?.ToString() ?? "No Version Found";
    public bool HasUpdateAvailable => AvailableVersion != default;
    public PanelUpdateInformationViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }

    protected override void OnViewModelBeforeRender()
    {
    }
}
