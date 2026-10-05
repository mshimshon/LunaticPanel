using LunaticPanel.Package.Server.Application.Mediator.Engine;
using LunaticPanel.Package.Server.Application.Mediator.Queries;
using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Mapping;
using LunaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LunaticPanel.Package.Server.Domain.Repositories;
using LunaticPanel.Package.Server.Application.Payloads.Mapping;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

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
        var result = await _readRepository.GetAllAsync(id, ct);
        return result.Select(p => p.ToApplication()).ToList();
    }
}
