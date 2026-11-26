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
    public bool HasUpdateAvailable => HasFoundUpdate || HasDownloaded || HasStaged;
    public bool HasDownloaded => DownloadedVersion != default;
    public bool HasStaged => StagedVersion != default;
    public bool HasFoundUpdate => UpdateFound != default;
    public NuGetVersion? Version => UpdateFound ?? StagedVersion ?? DownloadedVersion;

    public PanelUpdateInformationViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }

    protected override void OnViewModelBeforeRender()
    {
    }
}
