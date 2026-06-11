using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageInstallHandler
{

    public async Task Handle(PackageInstallCommand command,
        IRepositorySourceService repositorySourceService,
        IPackageRepository packageRepository,
        CancellationToken ct = default)
    {
        try
        {
            PackageId id = new(command.Id);
            PackageVersion version = new(command.Version);
            RepositorySourceEntity sourceEntity = command.Source.ToDomainEntity();
            await repositorySourceService.DownloadAsync(command.Id, command.Version, command.Source, ct);
            await packageRepository.InstallAsync(id, version, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
