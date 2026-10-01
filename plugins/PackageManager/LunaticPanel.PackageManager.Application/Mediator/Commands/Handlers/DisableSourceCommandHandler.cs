using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class DisableSourceCommandHandler : IRequestHandler<DisableSourceCommand>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly ICrazyReport<DisableSourceCommandHandler> _crazyReport;

    public DisableSourceCommandHandler(ISourceRepository sourceRepository, ICrazyReport<DisableSourceCommandHandler> crazyReport)
    {
        _sourceRepository = sourceRepository;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");

    }

    public async Task Handle(DisableSourceCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            var entity = request.Source.ToDomainEntity();
            await _sourceRepository.DisableAsync(entity, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}

