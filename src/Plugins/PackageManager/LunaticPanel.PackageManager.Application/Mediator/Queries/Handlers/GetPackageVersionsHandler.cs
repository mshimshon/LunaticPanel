using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Services;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetPackageVersionsHandler : IRequestHandler<GetPackageVersionsQuery, ICollection<string>>
{
    private readonly IRepositorySourceService _repositorySourceService;

    public GetPackageVersionsHandler(IRepositorySourceService repositorySourceService)
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
            var result = await _repositorySourceService.GetVersionsAsync(packageId, repo, ct);

            return result.ToList();
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }
    }
}
