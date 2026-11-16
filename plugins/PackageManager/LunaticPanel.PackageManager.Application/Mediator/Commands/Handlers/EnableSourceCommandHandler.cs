using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class EnableSourceCommandHandler : IRequestHandler<EnableSourceCommand>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly ICrazyReport<EnableSourceCommandHandler> _crazyReport;

    public EnableSourceCommandHandler(ISourceRepository sourceRepository, ICrazyReport<EnableSourceCommandHandler> crazyReport)
    {
        _sourceRepository = sourceRepository;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }

    public async Task HandleAsync(EnableSourceCommand request, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Handler Called");
            var entity = request.Source.ToDomainEntity();
            await _sourceRepository.EnableAsync(entity, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            throw new HostUnkownException();
        }
    }
}
