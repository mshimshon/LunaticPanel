using LunaticPanel.Package.Server.Application.Mediator.Queries.Exceptions;
using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Mapping;
using LunaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LunaticPanel.Package.Server.Domain.Repositories;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

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
