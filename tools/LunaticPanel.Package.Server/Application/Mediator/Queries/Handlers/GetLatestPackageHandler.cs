using LunaticPanel.Package.Server.Application.Mediator.Engine;
using LunaticPanel.Package.Server.Application.Mediator.Queries;
using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Mapping;
using LunaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LunaticPanel.Package.Server.Domain.Repositories;
using LunaticPanel.Package.Server.Application.Payloads.Mapping;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

internal sealed class GetLatestPackageHandler : IRequestHandler<GetLatestPackageQuery, ManifestPayload>
{
    private readonly IManifestReadRepository _readRepository;

    public GetLatestPackageHandler(IManifestReadRepository readRepository)
    {
        _readRepository = readRepository;
    }
    public async Task<ManifestPayload> HandleAsync(GetLatestPackageQuery data, CancellationToken ct = default)
    {
        PackageId id = new(data.Id);
        var result = await _readRepository.GetMostRecentAsync(id, ct);
        return result.ToApplication();
    }
}
