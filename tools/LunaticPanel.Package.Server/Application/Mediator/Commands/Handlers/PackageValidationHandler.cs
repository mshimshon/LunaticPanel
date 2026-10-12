using LunaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;
using LunaticPanel.Package.Server.Application.Payloads.Enums;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using LunaticPanel.Package.Server.Application.Services;
using MaksimShimshon.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

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
