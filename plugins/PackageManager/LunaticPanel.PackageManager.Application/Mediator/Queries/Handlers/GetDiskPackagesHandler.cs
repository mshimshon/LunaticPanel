using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetDiskPackagesHandler : IRequestHandler<GetDiskPackagesQuery, DiskPackagesPayload>
{
    private readonly IHostDiskPackageService _hostDiskPackageService;
    private readonly ICrazyReport<SearchRepositoryHandler> _crazyReport;

    public GetDiskPackagesHandler(IHostDiskPackageService hostDiskPackageService, ICrazyReport<SearchRepositoryHandler> crazyReport)
    {
        _hostDiskPackageService = hostDiskPackageService;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }
    //TODO: Implement
    public async Task<DiskPackagesPayload> Handle(GetDiskPackagesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var pendingUpdates = await _hostDiskPackageService.GetPendingUpdates(cancellationToken);
            var pendingDelete = await _hostDiskPackageService.GetPendingDelete(cancellationToken);
            var rollbacks = await _hostDiskPackageService.GetRollbacks(cancellationToken);
            var installed = await _hostDiskPackageService.GetIntalled(cancellationToken);
            var preInstalled = await _hostDiskPackageService.GetPreInstalled(cancellationToken);
            var runtimePackages = await _hostDiskPackageService.GetAll(cancellationToken);

            return new DiskPackagesPayload()
            {
                AvailableRollbacks = rollbacks.ToList(),
                Installed = installed.ToList(),
                PendingDelete = pendingDelete.ToList(),
                PendingUpdates = pendingUpdates.ToList(),
                PreInstalled = preInstalled.ToList(),
                RuntimePackages = runtimePackages.Select(p => p.ToApplicationPayload()).ToList()
            };
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
