using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class DisableSourceCommandHandler : IRequestHandler<DisableSourceCommand> 
{
    private readonly ICrazyReport<DisableSourceCommandHandler> _crazyReport;

    public DisableSourceCommandHandler(ICrazyReport<DisableSourceCommandHandler> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");

    }

    public async Task Handle(DisableSourceCommand request, CancellationToken ct = default)
    {
        try
        {
             _crazyReport.Report("Handler Called");
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}

