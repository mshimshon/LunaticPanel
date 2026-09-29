using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageCancelInstallCommandHandler : IRequestHandler<PackageCancelInstallCommand>
{
    private readonly IHostDiskPackageService _hostDiskPackageService;
    private readonly ICrazyReport<PackageCancelInstallCommandHandler> _crazyReport;

    public PackageCancelInstallCommandHandler(IHostDiskPackageService hostDiskPackageService,
        ICrazyReport<PackageCancelInstallCommandHandler> crazyReport)
    {
        _hostDiskPackageService = hostDiskPackageService;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }

    public async Task Handle(PackageCancelInstallCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            await _hostDiskPackageService.CancelPendingUpdate(request.Package, ct);

            // TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
