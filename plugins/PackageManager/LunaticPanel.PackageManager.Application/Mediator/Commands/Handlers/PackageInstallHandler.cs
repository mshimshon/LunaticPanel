using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageInstallHandler : IRequestHandler<PackageInstallCommand>
{
    private readonly IPackageDownloader _repositorySourceService;
    private readonly IPackageRepository _packageRepository;

    public PackageInstallHandler(IPackageDownloader repositorySourceService,
        IPackageRepository packageRepository)
    {
        _repositorySourceService = repositorySourceService;
        _packageRepository = packageRepository;
    }
    public async Task Handle(PackageInstallCommand command,
        CancellationToken ct = default)
    {
        try
        {
            PackageEntity package = command.Data.ToDomainEntity();
            //var current = await _repositorySourceService.GetLatestVersionAsync([package.Info.Id.Value], ct);
            await _repositorySourceService.DownloadAsync(command.Data, ct);
            await _packageRepository.InstallAsync(package, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
