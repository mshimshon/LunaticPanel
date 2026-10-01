using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Keys;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetSourcesHandler : IRequestHandler<GetSourcesQuery, ICollection<RepositorySourcePayload>>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly ICrazyReport<SearchRepositoryHandler> _crazyReport;

    public GetSourcesHandler(ISourceRepository sourceRepository, ICrazyReport<SearchRepositoryHandler> crazyReport)
    {
        _sourceRepository = sourceRepository;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }
    public async Task<ICollection<RepositorySourcePayload>> Handle(GetSourcesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var sources = await _sourceRepository.GetAllAsync(cancellationToken);
            return sources.Select(p => p.ToApplicationPayload()).ToList();
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
