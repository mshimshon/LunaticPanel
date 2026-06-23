using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageInstallHandler : IRequestHandler<PackageInstallCommand>
{
    private readonly IRepositorySourceService _repositorySourceService;
    private readonly IPackageService _packageService;
    private readonly IPackageRepository _packageRepository;

    public PackageInstallHandler(IRepositorySourceService repositorySourceService, IPackageService packageService,
        IPackageRepository packageRepository)
    {
        _repositorySourceService = repositorySourceService;
        _packageService = packageService;
        _packageRepository = packageRepository;
    }
    public async Task Handle(PackageInstallCommand command,
        CancellationToken ct = default)
    {
        try
        {
            PackageEntity package = command.Data.ToDomainEntity();
            RepositorySourceEntity sourceEntity = command.Source.ToDomainEntity();
            var current = await _packageRepository.GetByIdAsync(package.Info.Id, ct);
            await _repositorySourceService.DownloadAsync(command.Data, command.Source, ct);
            await _packageRepository.InstallAsync(package, ct);
            await _packageService.CreateRollbackAsync(current.ToApplicationPayload(), ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
