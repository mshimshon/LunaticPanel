using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageUpdateHandler : IRequestHandler<PackageUpdateCommand>
{
    private readonly IRepositorySourceService _repositorySourceService;
    private readonly IPackageRepository _packageRepository;

    public PackageUpdateHandler(IRepositorySourceService repositorySourceService, IPackageRepository packageRepository)
    {
        _repositorySourceService = repositorySourceService;
        _packageRepository = packageRepository;
    }
    public async Task Handle(PackageUpdateCommand command, CancellationToken ct = default)
    {
        try
        {
            PackageId id = new(command.Id);
            PackageVersion fromVersion = new(command.FromVersion);
            PackageVersion toVersion = new(command.ToVersion);
            RepositorySourceEntity sourceEntity = command.Source.ToDomainEntity();
            await _repositorySourceService.DownloadAsync(id.Value, toVersion.Value, command.Source, ct);
            await _packageRepository.UpdateAsync(id, fromVersion, toVersion, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
