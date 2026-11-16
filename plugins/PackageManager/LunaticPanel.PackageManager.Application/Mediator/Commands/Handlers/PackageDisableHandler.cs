using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Domain.Respositories;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageDisableHandler : IRequestHandler<PackageDisableCommand>
{
    private readonly IPackageRepository _packageRepository;

    public PackageDisableHandler(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }
    public async Task HandleAsync(PackageDisableCommand command, CancellationToken ct = default)
    {
        try
        {
            await _packageRepository.DisableAsync(new(command.Id), ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {

            throw new HostUnkownException();
        }
    }
}
