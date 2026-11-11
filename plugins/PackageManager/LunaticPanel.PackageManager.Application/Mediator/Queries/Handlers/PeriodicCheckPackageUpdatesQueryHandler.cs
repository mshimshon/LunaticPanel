using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using StatePulse.Net;
namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class PeriodicCheckPackageUpdatesQueryHandler : IRequestHandler<PeriodicCheckPackageUpdatesQuery, ICollection<PackageManifestPayload>>
{
    private readonly IMediator _mediator;
    private readonly IStateAccessor<PackageManagerDiskState> _packageManagerDiskState;
    private readonly ICrazyReport<PeriodicCheckPackageUpdatesQueryHandler> _crazyReport;

    public PeriodicCheckPackageUpdatesQueryHandler(IMediator mediator, IStateAccessor<PackageManagerDiskState> packageManagerDiskState,
        ICrazyReport<PeriodicCheckPackageUpdatesQueryHandler> crazyReport)
    {
        _mediator = mediator;
        _packageManagerDiskState = packageManagerDiskState;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");

    }

    public async Task<ICollection<PackageManifestPayload>> HandleAsync(PeriodicCheckPackageUpdatesQuery request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            var snap = _packageManagerDiskState.State.Installed.ToDictionary(p => p.Info.PackageId, p => new NuGetVersion(p.Version));
            var result = await _mediator.ExecuteAsync(new GetPackagesLatestVersionQuery(snap.Select(p => p.Key)), ct);
            var updates = result.Where(p => new NuGetVersion(p.Version) > snap[p.Id]).ToList();
            return updates;
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            throw new HostUnkownException();
        }
    }
}
