using LunaticPanel.Package.Server.Application.Mediator.Engine;
using LunaticPanel.Package.Server.Application.Mediator.Queries;
using LunaticPanel.Package.Server.Application.Payloads.Mapping;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using LunaticPanel.Package.Server.Domain.Repositories;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

internal sealed class SearchManifestHandler : IRequestHandler<SearchManifestQuery, ManifestSearchResponse>
{
    private readonly IManifestReadRepository _readRepository;

    public SearchManifestHandler(IManifestReadRepository readRepository)
    {
        _readRepository = readRepository;
    }
    public async Task<ManifestSearchResponse> HandleAsync(SearchManifestQuery data, CancellationToken ct = default)
    {
        var searchModel = data.Query.ToDomain();
        var searchResult = await _readRepository.SearchAsync(searchModel, ct);
        return searchResult.ToApplication();
    }
}
