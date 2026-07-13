using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

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
