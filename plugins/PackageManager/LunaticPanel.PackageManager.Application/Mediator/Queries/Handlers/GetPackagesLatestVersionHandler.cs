using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetPackagesLatestVersionHandler : IRequestHandler<GetPackagesLatestVersionQuery, ICollection<PackageManifestPayload>>
{
    private readonly IPackageDownloader _repositorySourceService;

    public GetPackagesLatestVersionHandler(IPackageDownloader repositorySourceService)
    {
        _repositorySourceService = repositorySourceService;
    }
    public async Task<ICollection<PackageManifestPayload>> HandleAsync(GetPackagesLatestVersionQuery request, CancellationToken ct = default)
    {
        try
        {
            var result = await _repositorySourceService.GetLatestVersionAsync(request.Packages, ct);
            return result.ToList();
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {

            throw new HostUnkownException();
        }
    }
}
