using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
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

    public AutoUpdatePackagesCommandHandler(
        IPanelControl panelControl,
        IMediator mediator,
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
    public static bool Validation(PackageManifestPayload payload, NuGetVersion currentVersion, NuGetVersion currentPanelVersion, ICrazyReport? crazyReport)
    {
        NuGetVersion packagePanelVersion = new NuGetVersion(payload.PanelVersion);
        NuGetVersion packageVersion = new NuGetVersion(payload.Version);
        NuGetVersion packageCurrentVersion = currentVersion;
        int currentPackageMajor = packageCurrentVersion.Major;
        int packagePanelMajor = packagePanelVersion.Major;
        int packageVersionMajor = packageVersion.Major;
        int panelMajor = currentPanelVersion.Major;
        if (packagePanelMajor != panelMajor)
        {

            crazyReport.SafeReportWarning("Cannot Auto Update for {0} due to Panel Major Mismatch", payload.Id);
            return false;
        }
        if (packageVersionMajor != currentPackageMajor)
        {
            crazyReport.SafeReportWarning("Cannot Auto Update for {0} due to Major Version Mismatch", payload.Id);
            return false;
        }

        if (packagePanelVersion > currentPanelVersion)
        {
            crazyReport.SafeReportWarning("Cannot Auto Update for {0} the package was compiled against more recent panel {1}.", payload.Id, packagePanelVersion);
            return false;
        }
        return true;
    }
    public async Task HandleAsync(AutoUpdatePackagesCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            var snap = _packageUpdateState.State.FoundUpdates.ToList();
            var currentVersions = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => new NuGetVersion(p.Version));
            var currentPanelVersion = new NuGetVersion(_panelControl.PanelVersion);
            foreach (var item in snap)
            {
                bool skip = !Validation(item, currentVersions[item.Id], currentPanelVersion, _crazyReport);
                if (skip) continue;
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
