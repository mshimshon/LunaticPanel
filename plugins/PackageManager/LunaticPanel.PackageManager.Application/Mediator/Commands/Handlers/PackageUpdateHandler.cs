using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageUpdateHandler : IRequestHandler<PackageUpdateCommand>
{
    private readonly IPackageFileReader _packageFileReader;
    private readonly IPackageDownloader _repositorySourceService;
    private readonly IPackageRepository _packageRepository;

    public PackageUpdateHandler(IPackageFileReader packageFileReader, IPackageDownloader repositorySourceService, IPackageRepository packageRepository)
    {
        _packageFileReader = packageFileReader;
        _repositorySourceService = repositorySourceService;
        _packageRepository = packageRepository;
    }

    public async Task Handle(PackageUpdateCommand command, CancellationToken ct = default)
    {
        try
        {
            var file = await _repositorySourceService.DownloadAsync(command.Package, ct);
            var payload = _packageFileReader.GetPackageInformation(file);
            PackageEntity packageEntity = payload.ToDomainEntity();
            await _packageRepository.UpdateAsync(packageEntity, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            throw new HostUnkownException();
        }
    }
}
