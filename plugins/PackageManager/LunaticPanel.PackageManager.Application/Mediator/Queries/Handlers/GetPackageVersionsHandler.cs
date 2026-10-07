using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Services;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetPackageVersionsHandler : IRequestHandler<GetPackageVersionsQuery, ICollection<string>>
{
    private readonly IPackageDownloader _repositorySourceService;

    public GetPackageVersionsHandler(IPackageDownloader repositorySourceService)
    {
        _repositorySourceService = repositorySourceService;
    }
    public async Task<ICollection<string>> Handle(GetPackageVersionsQuery request, CancellationToken ct = default)
    {
        try
        {
            var packageId = request.PackageId;
            // TODO: Validate Package Id
            var repo = request.RepositorySources.ToList().AsReadOnly();
            var result = await _repositorySourceService.GetVersionsAsync(packageId, ct);

            return result.ToList();
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }
    }
}
