using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

public sealed class GetAllPackageVersionsHandler : IRequestHandler<GetAllPackageVersionsQuery, ICollection<ManifestPayload>>
{
    private readonly IManifestReadRepository _readRepository;

    public GetAllPackageVersionsHandler(IManifestReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<ICollection<ManifestPayload>> HandleAsync(GetAllPackageVersionsQuery data, CancellationToken ct = default)
    {
        PackageId id = new(data.Id);
        await _readRepository.
    }
}
