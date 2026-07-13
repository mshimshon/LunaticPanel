using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Enums;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Application.Services;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class PackageValidationHandler : IRequestHandler<PackageValidationCommand, PackageValidationResponse>
{
    private readonly IPackageValidatorService _packageValidatorService;

    public PackageValidationHandler(IPackageValidatorService packageValidatorService)
    {
        _packageValidatorService = packageValidatorService;
    }
    public async Task<PackageValidationResponse> HandleAsync(PackageValidationCommand data, CancellationToken ct = default)
    {
        if (data.Data.LocationType == PackageValidationLocation.Remote)
            return await _packageValidatorService.ValidateRemoteAsync(data.Data.Target, ct);
        else
            return await _packageValidatorService.ValidateLocalAsync(data.Data.Target, ct);
    }
}
