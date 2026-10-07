using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageRemoveHandler : IRequestHandler<PackageRemoveCommand>
{
    private readonly IPackageRepository _packageRepository;
    private readonly ICrazyReport<PackageRemoveHandler> _crazyReport;

    public PackageRemoveHandler(IPackageRepository packageRepository, ICrazyReport<PackageRemoveHandler> crazyReport)
    {
        _packageRepository = packageRepository;
        _crazyReport = crazyReport;
    }
    public async Task Handle(PackageRemoveCommand command, CancellationToken ct = default)
    {
        try
        {
            _crazyReport.Report("Parsing Id");
            PackageId id = new(command.Id);
            _crazyReport.Report("Disabling Plugin '{0}'", id.Value);
            await _packageRepository.DisableAsync(id, ct);
            _crazyReport.Report("Marking for Delete '{0}'", id.Value);
            await _packageRepository.DeleteAsync(id, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            throw new HostUnkownException();
        }
    }
}
