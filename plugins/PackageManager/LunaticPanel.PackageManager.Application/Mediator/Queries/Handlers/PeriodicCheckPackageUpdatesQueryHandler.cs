using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;
namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class PeriodicCheckPackageUpdatesQueryHandler : IRequestHandler<PeriodicCheckPackageUpdatesQuery, ICollection<PackagePayload>>
{
    private readonly IMedihater _medihater;
    private readonly IStateAccessor<PackageManagerDiskState> _packageManagerDiskState;
    private readonly ICrazyReport<PeriodicCheckPackageUpdatesQueryHandler> _crazyReport;

    public PeriodicCheckPackageUpdatesQueryHandler(IMedihater medihater, IStateAccessor<PackageManagerDiskState> packageManagerDiskState,
        ICrazyReport<PeriodicCheckPackageUpdatesQueryHandler> crazyReport)
    {
        _medihater = medihater;
        _packageManagerDiskState = packageManagerDiskState;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");

    }

    public async Task<ICollection<PackagePayload>> Handle(PeriodicCheckPackageUpdatesQuery request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            var snap = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => new Version(p.Version));
            var result = await _medihater.Send(new GetPackagesLatestVersionQuery(snap.Select(p => p.Key)), ct);
            var updates = result.Where(p => new Version(p.Version) > snap[p.Info.PackageId]).ToList();
            return updates;
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
