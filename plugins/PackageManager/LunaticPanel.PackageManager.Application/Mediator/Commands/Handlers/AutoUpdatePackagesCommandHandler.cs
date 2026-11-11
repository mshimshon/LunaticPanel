using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class AutoUpdatePackagesCommandHandler : IRequestHandler<AutoUpdatePackagesCommand>
{
    private readonly IPanelControl _panelControl;
    private readonly IMediator _mediator;
    private readonly IStateAccessor<PackageUpdateState> _packageUpdateState;
    private readonly IStateAccessor<PackageManagerDiskState> _packageManagerDiskState;
    private readonly ICrazyReport<AutoUpdatePackagesCommandHandler> _crazyReport;

    public AutoUpdatePackagesCommandHandler(IPanelControl panelControl, IMediator mediator,
        IStateAccessor<PackageUpdateState> packageUpdateState,
        IStateAccessor<PackageManagerDiskState> packageManagerDiskState,
        ICrazyReport<AutoUpdatePackagesCommandHandler> crazyReport)
    {
        _panelControl = panelControl;
        _mediator = mediator;
        _packageUpdateState = packageUpdateState;
        _packageManagerDiskState = packageManagerDiskState;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }

    public async Task HandleAsync(AutoUpdatePackagesCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            var snap = _packageUpdateState.State.FoundUpdates.ToList();
            var currentVersions = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => new NuGetVersion(p.Version));
            var UpdateVersion = snap.ToDictionary(p => p.Id, p => new NuGetVersion(p.Version));
            var UpdatePanelVersion = snap.ToDictionary(p => p.Id, p => new NuGetVersion(p.PanelVersion));
            var currentPanelVersion = new NuGetVersion(_panelControl.PanelVersion);
            int major = currentPanelVersion.Major;
            foreach (var item in snap)
            {
                NuGetVersion packagePanelVersion = new NuGetVersion(item.PanelVersion);
                NuGetVersion packageVersion = new NuGetVersion(item.PanelVersion);
                NuGetVersion packageCurrentVersion = currentVersions[item.Id];
                int currentPackageMajor = packageCurrentVersion.Major;
                int packagePanelMajor = packagePanelVersion.Major;
                int packageVersionMajor = packageVersion.Major;
                if (packagePanelMajor != major)
                {
                    _crazyReport.ReportWarning("Cannot Auto Update for {0} due to Panel Major Mismatch", item.Id);
                    continue;
                }
                if (packageVersionMajor != currentPackageMajor)
                {
                    _crazyReport.ReportWarning("Cannot Auto Update for {0} due to Major Version Mismatch", item.Id);
                    continue;
                }

                if (packagePanelVersion > currentPanelVersion)
                {
                    _crazyReport.ReportWarning("Cannot Auto Update for {0} the package was compiled against more recent panel {1}.", item.Id, packagePanelVersion);
                    continue;
                }
                await _mediator.ExecuteAsync(new PackageUpdateCommand(item), ct);
            }
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            throw new HostUnkownException();
        }
    }

}
