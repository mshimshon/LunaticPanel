using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Services;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class SaveSourcesHandler : IRequestHandler<SaveSourcesCommand>
{
    private readonly ISourceFileService _sourceFileService;
    private readonly ICrazyReport<SaveSourcesHandler> _crazyReport;

    public SaveSourcesHandler(ISourceFileService sourceFileService, ICrazyReport<SaveSourcesHandler> crazyReport)
    {
        _sourceFileService = sourceFileService;
        _crazyReport = crazyReport;
    }
    public async Task Handle(SaveSourcesCommand request, CancellationToken ct)
    {
        try
        {
            await _sourceFileService.SaveSourcesAsync(request.Sources, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
