using LunaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LunaticPanel.Package.Server.Domain.Repositories;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class HideManifestVersionHandler : IRequestHandler<HideManifestVersionCommand>
{
    private readonly IManifestWriteRepository _writeRepository;

    public HideManifestVersionHandler(IManifestWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public async Task HandleAsync(HideManifestVersionCommand data, CancellationToken ct = default)
    {
        PackageId id = new(data.Id);
        PackageVersion version = new(data.Version);
        await _writeRepository.HideAsync(id, version, ct);
    }
}
