using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads;
using LuncaticPanel.Package.Server.Application.Payloads.Mapping;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

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
