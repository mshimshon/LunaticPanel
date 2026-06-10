using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageDisableHandler
{
    public async Task Handle(PackageDisableCommand command, IPackageRepository packageRepository, CancellationToken ct = default)
    {

        await packageRepository.DisableAsync(new(command.Id), ct);
    }
}
