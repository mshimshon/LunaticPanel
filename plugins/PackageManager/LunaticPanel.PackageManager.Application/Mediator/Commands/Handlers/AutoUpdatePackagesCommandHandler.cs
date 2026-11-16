using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class AutoUpdatePackagesCommandHandler : IRequestHandler<AutoUpdatePackagesCommand>
{
    private readonly IPanelControl _panelControl;
    private readonly IMediator _mediator;
    private readonly ICrazyReport<AutoUpdatePackagesCommandHandler> _crazyReport;

    public AutoUpdatePackagesCommandHandler(
        IPanelControl panelControl,
        IMediator mediator,
        ICrazyReport<AutoUpdatePackagesCommandHandler> crazyReport)
    {
        _panelControl = panelControl;
        _mediator = mediator;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }
    // CreatePackage("panel.notifications.slack", "Slack Notifications", "4.2.4", "2.0.0"),
    public static bool Validation(PackageManifestPayload payload, NuGetVersion currentPanelVersion, ICrazyReport? crazyReport)
    {
        NuGetVersion packagePanelVersion = NuGetVersion.Parse(payload.PanelVersion);
        int packagePanelMajor = packagePanelVersion.Major;
        int panelMajor = currentPanelVersion.Major;
        if (packagePanelMajor != panelMajor)
        {

            crazyReport.SafeReportWarning("Cannot Auto Update for {0} due to Panel Major Mismatch", payload.Id);
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
            var snap = request.FoundUpdates.ToList();
            var currentVersions = request.InstalledPackageVersions.ToDictionary(p => p.Key, p => NuGetVersion.Parse(p.Value));
            var currentPanelVersion = NuGetVersion.Parse(_panelControl.PanelVersion);
            foreach (var item in snap)
            {
                bool skip = !Validation(item, currentPanelVersion, _crazyReport);
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
