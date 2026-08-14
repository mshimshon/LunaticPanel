using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Services;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class SearchRepositoryHandler : IRequestHandler<SearchRepositoryQuery, Dictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>>>
{
    private readonly IRepositorySourceService _repositorySource;

    public SearchRepositoryHandler(IRepositorySourceService repositorySource)
    {
        _repositorySource = repositorySource;
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
        catch (Exception)
        {

            throw new HostUnkownException();
        }

    }
}
