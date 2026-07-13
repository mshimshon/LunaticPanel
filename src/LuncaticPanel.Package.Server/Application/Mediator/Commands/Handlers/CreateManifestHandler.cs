using LuncaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;
using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Enums;
using LuncaticPanel.Package.Server.Application.Payloads.Mapping;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Application.Services;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class CreateManifestHandler : IRequestHandler<CreateManifestCommand>
{
    private readonly IPackageValidatorService _packageValidatorService;
    private readonly IManifestWriteRepository _manifestWriteRepository;

    public CreateManifestHandler(IPackageValidatorService packageValidatorService, IManifestWriteRepository manifestWriteRepository)
    {
        _packageValidatorService = packageValidatorService;
        _manifestWriteRepository = manifestWriteRepository;
    }
    public async Task HandleAsync(CreateManifestCommand data, CancellationToken ct = default)
    {
        PackageValidationResponse? validation = default;
        if (data.Data.LocationType == PackageValidationLocation.Remote)
            validation = await _packageValidatorService.ValidateRemoteAsync(data.Data.Target, ct);
        else
            validation = await _packageValidatorService.ValidateLocalAsync(data.Data.Target, ct);
        if (!validation.HasPassed)
            throw new PackageValidationException(validation.ErrorFound?.Code ?? "Unknown", validation.ErrorFound?.Message ?? "Unknown Validation Error occured.");
        var manifest = validation!.Manifest!.ToDomain();
        await _manifestWriteRepository.CreateAsync(manifest, ct);
    }
}
