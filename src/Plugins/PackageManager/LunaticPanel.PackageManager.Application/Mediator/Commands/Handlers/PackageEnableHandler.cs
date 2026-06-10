using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageEnableHandler
{
    public async Task Handle(PackageEnableCommand command, IPackageRepository packageRepository, CancellationToken ct = default)
    {
        try
        {
            await packageRepository.EnableAsync(new(command.Id), ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }
    }
}
