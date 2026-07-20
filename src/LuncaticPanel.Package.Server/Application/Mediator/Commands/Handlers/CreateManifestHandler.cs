using LuncaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;
using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Mapping;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class CreateManifestHandler : IRequestHandler<CreateManifestCommand>
{
    private readonly IMediator _mediator;
    private readonly IManifestWriteRepository _manifestWriteRepository;
    private readonly IManifestReadRepository _manifestReadRepository;

    public CreateManifestHandler(IMediator mediator, IManifestWriteRepository manifestWriteRepository, IManifestReadRepository manifestReadRepository)
    {
        _mediator = mediator;
        _manifestWriteRepository = manifestWriteRepository;
        _manifestReadRepository = manifestReadRepository;
    }
    public async Task HandleAsync(CreateManifestCommand data, CancellationToken ct = default)
    {
        var validationCmd = new PackageValidationCommand(data.Data);
        PackageValidationResponse validation = await _mediator.ExecuteAsync(validationCmd, ct);
        bool exist = await _manifestReadRepository.ExistAsync(new(validation.Manifest.Id), new(validation.Manifest.Version), ct);
        if (exist)
            throw new PackageVersionExistException(validation.Manifest.Id, validation.Manifest.Version);
        var manifest = validation.Manifest.ToDomain();
        await _manifestWriteRepository.CreateAsync(manifest, ct);
    }
}
