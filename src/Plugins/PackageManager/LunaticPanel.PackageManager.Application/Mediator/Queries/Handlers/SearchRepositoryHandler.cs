using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class SearchRepositoryHandler : IRequestHandler<SearchRepositoryQuery, Dictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>>>
{
    private readonly IRepositorySourceService _repositorySource;
    private readonly ICrazyReport _crazyReport;

    public SearchRepositoryHandler(IRepositorySourceService repositorySource, ICrazyReport<SearchRepositoryHandler> crazyReport)
    {
        _repositorySource = repositorySource;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }
    public async Task<Dictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>>> Handle(SearchRepositoryQuery data, CancellationToken ct = default)
    {
        try
        {
            //TODO: ADD FLUENT VALIDATION FOR KEYWORDS
            //TODO: ADD SUPPORT FOR SEPCIFIC SOURCES SEARCH.
            var responses = await _repositorySource.SearchAsync(data.Search, ct);
            return responses;
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (HostCodedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR DETECTED!");
            _crazyReport.ReportErrorException(ex.Message, ex);
            throw new HostUnkownException();
        }

    }
}
// '/etc/lunaticpanel/plugins/lunaticpanel_packagemanager/config/packagemanager/sources.json'
