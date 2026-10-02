using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageUpdateHandler : IRequestHandler<PackageUpdateCommand>
{
    private readonly IPackageDownloader _repositorySourceService;
    private readonly IPackageRepository _packageRepository;

    public PackageUpdateHandler(IPackageDownloader repositorySourceService, IPackageRepository packageRepository)
    {
        _repositorySourceService = repositorySourceService;
        _packageRepository = packageRepository;
    }

    public async Task Handle(PackageUpdateCommand command, CancellationToken ct = default)
    {
        try
        {
            PackageEntity packageEntity = command.Package.ToDomainEntity();
            await _repositorySourceService.DownloadAsync(command.Package, ct);
            await _packageRepository.UpdateAsync(packageEntity, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
