using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class EndManifestLifeHandler : IRequestHandler<EndManifestLifeCommand>
{
    private readonly IManifestWriteRepository _writeRepository;

    public EndManifestLifeHandler(IManifestWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }
    public async Task HandleAsync(EndManifestLifeCommand data, CancellationToken ct = default)
    {
        PackageId id = new(data.Id);
        PackageEndOfLifeMessage message = new(data.Message);
        await _writeRepository.EndLifeAsync(id, message, ct);
    }
}
