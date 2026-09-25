using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class MoveUpSourceCommandHandler : IRequestHandler<MoveUpSourceCommand>
{
    private readonly ISourceOrderingService _sourceOrderingService;
    private readonly ICrazyReport<MoveUpSourceCommandHandler> _crazyReport;

    public MoveUpSourceCommandHandler(ISourceOrderingService sourceOrderingService, ICrazyReport<MoveUpSourceCommandHandler> crazyReport)
    {
        _sourceOrderingService = sourceOrderingService;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");

    }

    public async Task Handle(MoveUpSourceCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            await _sourceOrderingService.MoveUpAsync(request.Source, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
