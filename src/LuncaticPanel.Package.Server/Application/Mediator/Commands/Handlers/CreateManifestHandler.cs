using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Mapping;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class CreateManifestHandler : IRequestHandler<CreateManifestCommand>
{
    private readonly IManifestWriteRepository _manifestWriteRepository;

    public CreateManifestHandler(IManifestWriteRepository manifestWriteRepository)
    {
        _manifestWriteRepository = manifestWriteRepository;
    }
    public async Task HandleAsync(CreateManifestCommand data, CancellationToken ct = default)
    {
        var manifest = data.Data.ToDomain();
        await _manifestWriteRepository.CreateAsync(manifest, ct);
    }
}
