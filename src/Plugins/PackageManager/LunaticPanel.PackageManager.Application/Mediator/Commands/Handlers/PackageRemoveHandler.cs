using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageRemoveHandler
{
    public async Task Handle(PackageRemoveCommand command, IPackageRepository packageRepository, CancellationToken ct = default)
    {
        try
        {
            PackageId id = new(command.Id);
            await packageRepository.DisableAsync(id, ct);
            await packageRepository.DeleteAsync(id, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
