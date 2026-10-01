using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class MoveDownSourceCommandHandler : IRequestHandler<MoveDownSourceCommand>
{
    private readonly ISourceOrderingService _sourceOrderingService;
    private readonly ICrazyReport<MoveDownSourceCommandHandler> _crazyReport;

    public MoveDownSourceCommandHandler(ISourceOrderingService sourceOrderingService, ICrazyReport<MoveDownSourceCommandHandler> crazyReport)
    {
        _sourceOrderingService = sourceOrderingService;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");

    }

    public async Task Handle(MoveDownSourceCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            await _sourceOrderingService.MoveDownAsync(request.Source, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
