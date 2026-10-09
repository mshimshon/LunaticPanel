using LunaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LunaticPanel.Package.Server.Domain.Repositories;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class EndManifestLifeHandler : IRequestHandler<EndManifestLifeCommand>
{
    private readonly IManifestWriteRepository _writeRepository;

    public EndManifestLifeHandler(IManifestWriteRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }
    public async Task HandleAsync(EndManifestLifeCommand data, CancellationToken ct = default)
    {
        PackageId id = new(data.Data.Id);
        PackageEndOfLifeMessage message = new(data.Data.Message);
        await _writeRepository.EndLifeAsync(id, message, ct);
    }
}
