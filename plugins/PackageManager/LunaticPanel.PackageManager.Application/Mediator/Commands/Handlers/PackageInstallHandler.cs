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
    private readonly IPackageFileReader _packageFileReader;

    public PackageInstallHandler(IPackageDownloader repositorySourceService,
        IPackageRepository packageRepository, IPackageFileReader packageFileReader)
    {
        _repositorySourceService = repositorySourceService;
        _packageRepository = packageRepository;
        _packageFileReader = packageFileReader;
    }
    public async Task Handle(PackageInstallCommand command,
        CancellationToken ct = default)
    {
        try
        {
            //var current = await _repositorySourceService.GetLatestVersionAsync([package.Info.Id.Value], ct);
            var file = await _repositorySourceService.DownloadAsync(command.Data, ct);
            var payload = _packageFileReader.GetPackageInformation(file);
            PackageEntity package = payload.ToDomainEntity();
            await _packageRepository.InstallAsync(package, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            throw new HostUnkownException();
        }
    }
}
