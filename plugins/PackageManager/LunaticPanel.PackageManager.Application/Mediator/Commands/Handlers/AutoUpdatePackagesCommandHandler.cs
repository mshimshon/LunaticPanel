using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class AutoUpdatePackagesCommandHandler : IRequestHandler<AutoUpdatePackagesCommand>
{
    private readonly IPanelControl _panelControl;
    private readonly IMedihater _medihater;
    private readonly IStateAccessor<PackageUpdateState> _packageUpdateState;
    private readonly IStateAccessor<PackageManagerDiskState> _packageManagerDiskState;
    private readonly ICrazyReport<AutoUpdatePackagesCommandHandler> _crazyReport;

    public AutoUpdatePackagesCommandHandler(IPanelControl panelControl, IMedihater medihater,
        IStateAccessor<PackageUpdateState> packageUpdateState,
        IStateAccessor<PackageManagerDiskState> packageManagerDiskState,
        ICrazyReport<AutoUpdatePackagesCommandHandler> crazyReport)
    {
        _panelControl = panelControl;
        _medihater = medihater;
        _packageUpdateState = packageUpdateState;
        _packageManagerDiskState = packageManagerDiskState;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }

    public async Task Handle(AutoUpdatePackagesCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            var snap = _packageUpdateState.State.FoundUpdates.ToList();
            var currentVersions = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => new Version(p.Version));
            var UpdateVersion = snap.ToDictionary(p => p.Info.PackageId, p => new Version(p.Version));
            var UpdatePanelVersion = snap.ToDictionary(p => p.Info.PackageId, p => new Version(p.PanelVersion));
            int major = _panelControl.PanelVersion.Major;
            foreach (var item in snap)
            {
                Version packagePanelVersion = new Version(item.PanelVersion);
                Version packageVersion = new Version(item.PanelVersion);
                Version packageCurrentVersion = currentVersions[item.Info.PackageId];
                int currentPackageMajor = packageCurrentVersion.Major;
                int packagePanelMajor = packagePanelVersion.Major;
                int packageVersionMajor = packageVersion.Major;
                if (packagePanelMajor != major)
                {
                    _crazyReport.ReportWarning("Cannot Auto Update for {0} due to Panel Major Mismatch", item.Info.PackageId);
                    continue;
                }
                if (packageVersionMajor != currentPackageMajor)
                {
                    _crazyReport.ReportWarning("Cannot Auto Update for {0} due to Major Version Mismatch", item.Info.PackageId);
                    continue;
                }

                if (packagePanelVersion > _panelControl.PanelVersion)
                {
                    _crazyReport.ReportWarning("Cannot Auto Update for {0} the package was compiled against more recent panel {1}.", item.Info.PackageId, packagePanelVersion);
                    continue;
                }
                await _medihater.Send(new PackageUpdateCommand(item), ct);
            }
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
