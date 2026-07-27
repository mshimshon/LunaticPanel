using LuncaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;
using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Enums;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal sealed class PackageValidationHandler : IRequestHandler<PackageValidationCommand, PackageValidationResponse>
{
    private readonly IPackageValidatorService _packageValidatorService;
    private readonly IPackageValidationEvents? _packageValidationEvents;
    public PackageValidationHandler(IPackageValidatorService packageValidatorService, IServiceProvider serviceProvider)
    {
        _packageValidatorService = packageValidatorService;
        _packageValidationEvents = serviceProvider.GetService<IPackageValidationEvents>();

    }
    public async Task<PackageValidationResponse> HandleAsync(PackageValidationCommand data, CancellationToken ct = default)
    {
        try
        {
            Console.WriteLine($"Trying to Validate {data.Data.Target}");
            if (data.Data.LocationType == PackageValidationLocation.Remote)
                return await _packageValidatorService.ValidateRemoteAsync(data.Data.Target, ct);
            else
                return await _packageValidatorService.ValidateLocalAsync(data.Data.Target, ct);
        }
        catch (PackageValidationException ex)
        {
            Console.WriteLine($"PackageValidationHandler {ex.Message}");
            _ = _packageValidationEvents?.OnValidationFailure(data.Data, ex);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PackageValidationHandler {ex.Message}");
            _ = _packageValidationEvents?.OnValidationFailure(data.Data, new PackageValidationException("Unknown", "Unknown Internal Error Occured.", null));
            throw;
        }

    }
}
