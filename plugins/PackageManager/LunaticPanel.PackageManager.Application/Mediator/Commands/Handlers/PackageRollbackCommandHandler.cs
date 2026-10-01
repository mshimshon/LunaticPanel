using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageRollbackCommandHandler : IRequestHandler<PackageRollbackCommand>
{
    private readonly IHostDiskPackageService _hostDiskPackageService;
    private readonly ICrazyReport<PackageRollbackCommandHandler> _crazyReport;

    public PackageRollbackCommandHandler(IHostDiskPackageService hostDiskPackageService, ICrazyReport<PackageRollbackCommandHandler> crazyReport)
    {
        _hostDiskPackageService = hostDiskPackageService;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }

    public async Task Handle(PackageRollbackCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            await _hostDiskPackageService.RollbackTo(request.Package, ct);
            // TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
