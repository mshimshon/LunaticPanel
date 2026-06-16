using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entites;
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
            RepositorySourcePayload source = new()
            {
                SourceType = command.Package.RepositoryType,
                Source = command.Package.RepositorySource,
                Name = "Target",
                State = Payloads.Enums.RepositorySourceStatePayload.Unknown,
            };
            PackageEntity packageEntity = command.Package.ToDomainEntity();
            await _repositorySourceService.DownloadAsync(command.Package, source, ct);
            await _packageRepository.UpdateAsync(packageEntity, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
