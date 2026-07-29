using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Keys;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetAllPackagesHandler : IRequestHandler<GetAllPackagesQuery, ICollection<PackagePayload>>
{
    private readonly IPackageRepository _packageRepository;
    private readonly ICrazyReport<GetAllPackagesHandler> _crazyReport;

    public GetAllPackagesHandler(IPackageRepository packageRepository, ICrazyReport<GetAllPackagesHandler> crazyReport)
    {
        _packageRepository = packageRepository;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }
    public async Task<ICollection<PackagePayload>> Handle(
        GetAllPackagesQuery query,
        CancellationToken ct = default)
    {
        try
        {
            ICollection<Domain.Entities.PackageEntity> result = await _packageRepository.GetAll(ct);

            return result.Select(p => p.ToApplicationPayload()).ToList();
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            throw new HostUnkownException();
        }

    }
}
