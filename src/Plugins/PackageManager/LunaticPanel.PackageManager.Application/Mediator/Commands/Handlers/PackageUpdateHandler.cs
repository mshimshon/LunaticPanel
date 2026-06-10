using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageUpdateHandler
{
    public async Task Handle(PackageUpdateCommand command, IRepositorySourceService repositorySourceService, IPackageRepository packageRepository, CancellationToken ct = default)
    {
        try
        {
            PackageId id = new(command.Id);
            PackageVersion fromVersion = new(command.FromVersion);
            PackageVersion toVersion = new(command.ToVersion);
            RepositorySourceEntity sourceEntity = command.Source.ToDomainEntity();
            await repositorySourceService.DownloadAsync(id.Value, toVersion.Value, command.Source, ct);
            await packageRepository.UpdateAsync(id, fromVersion, toVersion, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
