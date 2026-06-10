using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Services;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class SearchRepositoryHandler
{
    public async Task<SearchResponse<PackageInfoPayload>> Handle(SearchRepositoryQuery data, IRepositorySourceService repositorySourceService, CancellationToken ct = default)
    {
        try
        {
            //TODO: ADD FLUENT VALIDATION FOR KEYWORDS
            string keywords = data.Keywords;
            var response =
                await repositorySourceService.SearchAsync(keywords, data.Sources.AsReadOnly(), ct);
            return response;
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }

    }
}
