using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Services;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class SearchRepositoryHandler : IRequestHandler<SearchRepositoryQuery, SearchResponse<PackageInfoPayload>>
{
    private readonly IRepositorySourceService _repositorySource;

    public SearchRepositoryHandler(IRepositorySourceService repositorySource)
    {
        _repositorySource = repositorySource;
    }
    public async Task<SearchResponse<PackageInfoPayload>> Handle(SearchRepositoryQuery data, CancellationToken ct = default)
    {
        try
        {
            //TODO: ADD FLUENT VALIDATION FOR KEYWORDS
            var response =
                await _repositorySource.SearchAsync(data.Search, ct);
            return response;
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }

    }
}
