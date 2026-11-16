using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using StatePulse.Net;
namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class PeriodicCheckPackageUpdatesQueryHandler : IRequestHandler<PeriodicCheckPackageUpdatesQuery, ICollection<PackageManifestPayload>>
{
    private readonly IMediator _mediator;
    private readonly ICrazyReport<PeriodicCheckPackageUpdatesQueryHandler> _crazyReport;

    public PeriodicCheckPackageUpdatesQueryHandler(IMediator mediator,
        ICrazyReport<PeriodicCheckPackageUpdatesQueryHandler> crazyReport)
    {
        _mediator = mediator;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");

    }

    public async Task<ICollection<PackageManifestPayload>> HandleAsync(PeriodicCheckPackageUpdatesQuery request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            var snap = request.InstalledPackages;
            var result = await _mediator.ExecuteAsync(new GetPackagesLatestVersionQuery(request.InstalledPackages.Select(p => p.Key)), ct);
            var updates = result.Where(p => NuGetVersion.Parse(p.Version) > request.InstalledPackages[p.Id]).ToList();
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
