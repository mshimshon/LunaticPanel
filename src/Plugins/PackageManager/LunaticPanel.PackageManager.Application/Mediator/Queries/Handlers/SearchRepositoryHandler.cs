using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
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
            string keywords = data.Keywords;
            var response =
                await _repositorySource.SearchAsync(keywords, data.Sources.AsReadOnly(), ct);
            return response;
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }

    }
}
