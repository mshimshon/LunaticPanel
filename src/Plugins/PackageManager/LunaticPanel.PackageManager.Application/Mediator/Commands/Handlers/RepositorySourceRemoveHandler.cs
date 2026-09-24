using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Keys;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class RepositorySourceRemoveHandler : IRequestHandler<RepositorySourceRemoveCommand>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly ICrazyReport<RepositorySourceRemoveHandler> _crazyReport;

    public RepositorySourceRemoveHandler(ISourceRepository sourceRepository, ICrazyReport<RepositorySourceRemoveHandler> crazyReport)
    {
        _sourceRepository = sourceRepository;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }
    public async Task Handle(RepositorySourceRemoveCommand command, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Converting to Entity");
            var source = command.Source.ToDomainEntity();
            _crazyReport.Report("Converting to Entity");
            await _sourceRepository.RemoveAsync(source, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            throw new HostUnkownException();
        }
    }
}
