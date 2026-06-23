using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetPackagesLatestVersionHandler : IRequestHandler<GetPackagesLatestVersionQuery, ICollection<PackagePayload>>
{
    private readonly IRepositorySourceService _repositorySourceService;

    public GetPackagesLatestVersionHandler(IRepositorySourceService repositorySourceService)
    {
        _repositorySourceService = repositorySourceService;
    }
    public async Task<ICollection<PackagePayload>> Handle(GetPackagesLatestVersionQuery request, CancellationToken ct = default)
    {
        try
        {
            var repo = request.RepositorySources.ToList().AsReadOnly();
            var result = await _repositorySourceService.GetLatestVersionAsync(request.Packages, ct);

            return result.ToList();
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }
    }
}
