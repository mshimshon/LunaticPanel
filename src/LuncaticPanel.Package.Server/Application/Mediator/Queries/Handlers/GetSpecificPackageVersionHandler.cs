using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Mediator.Queries.Exceptions;
using LuncaticPanel.Package.Server.Application.Payloads;
using LuncaticPanel.Package.Server.Application.Payloads.Mapping;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

internal sealed class GetSpecificPackageVersionHandler : IRequestHandler<GetSpecificPackageVersionQuery, ManifestPayload>
{
    private readonly IManifestReadRepository _readRepository;

    public GetSpecificPackageVersionHandler(IManifestReadRepository readRepository)
    {
        _readRepository = readRepository;
    }
    public async Task<ManifestPayload> HandleAsync(GetSpecificPackageVersionQuery data, CancellationToken ct = default)
    {
        PackageId id = new(data.Id);
        PackageVersion version = new(data.Version);
        var result = await _readRepository.GetAllAsync(id, ct);
        var targetResult = result.FirstOrDefault(p => p.Version.Value == version.Value);
        if (targetResult == default)
            throw new PackageVersionNotFoundException(id.Value, version.Value);
        return targetResult.ToApplication();
    }
}
