using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using StatePulse.Net;
namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetPanelUpdateCompatiblePluginsHandler : IRequestHandler<GetPanelUpdateCompatiblePluginsQuery, ICollection<PackageManifestPayload>>
{
    private readonly IPackageDownloader _packageDownloader;
    private readonly ICrazyReport<GetPanelUpdateCompatiblePluginsHandler> _crazyReport;

    public GetPanelUpdateCompatiblePluginsHandler(IPackageDownloader packageDownloader, ICrazyReport<GetPanelUpdateCompatiblePluginsHandler> crazyReport)
    {
        _packageDownloader = packageDownloader;

        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");

    }

    public async Task<ICollection<PackageManifestPayload>> HandleAsync(GetPanelUpdateCompatiblePluginsQuery request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            var result = await _packageDownloader.GetLatestVersionAsync(request.Packages.ToList(), request.PanelVersion, ct);
            return result.ToArray();
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            throw new HostUnkownException();
        }
    }
}
