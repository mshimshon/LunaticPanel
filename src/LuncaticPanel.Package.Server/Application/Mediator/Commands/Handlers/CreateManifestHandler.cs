using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Mapping;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class CreateManifestHandler : IRequestHandler<CreateManifestCommand>
{
    private readonly IMediator _mediator;
    private readonly IManifestWriteRepository _manifestWriteRepository;

    public CreateManifestHandler(IMediator mediator, IManifestWriteRepository manifestWriteRepository)
    {
        _mediator = mediator;
        _manifestWriteRepository = manifestWriteRepository;
    }
    public async Task HandleAsync(CreateManifestCommand data, CancellationToken ct = default)
    {
        var validationCmd = new PackageValidationCommand(data.Data);
        PackageValidationResponse validation = await _mediator.ExecuteAsync(validationCmd, ct);
        var manifest = validation.Manifest.ToDomain();
        await _manifestWriteRepository.CreateAsync(manifest, ct);
    }
}
