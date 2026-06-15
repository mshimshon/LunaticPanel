using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetRollbackPackagesHandler : IRequestHandler<GetRollbackPackagesQuery, ICollection<PackagePayload>>
{
    private readonly IPackageService _packageService;

    public GetRollbackPackagesHandler(IPackageService packageService)
    {
        _packageService = packageService;
    }
    public async Task<ICollection<PackagePayload>> Handle(GetRollbackPackagesQuery request, CancellationToken ct = default)
    {
        try
        {
            var result = await _packageService.GetAvailableRollbackAsync();

            return result;
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }

    }
}
