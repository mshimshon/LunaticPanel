using LunaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;
using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Mapping;
using LunaticPanel.Package.Server.Domain.Repositories;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class CreateManifestHandler : IRequestHandler<CreateManifestCommand, ManifestPayload>
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
    public async Task<ManifestPayload> HandleAsync(CreateManifestCommand data, CancellationToken ct = default)
    {
        Console.WriteLine("Creating Manifest Request Received");
        bool exist = await _manifestReadRepository.ExistAsync(new(data.Data.Id), new(data.Data.Version), ct);
        if (exist)
            throw new PackageVersionExistException(data.Data.Id, data.Data.Version);
        var manifest = data.Data.ToDomain();
        await _manifestWriteRepository.CreateAsync(manifest, ct);
        return manifest.ToApplication();
    }
}
