using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

internal sealed class SearchManifestHandler : IRequestHandler<SearchManifestQuery, ManifestSearchResponse>
{
    public Task<ManifestSearchResponse> HandleAsync(SearchManifestQuery data, CancellationToken ct = default)
        => throw new NotImplementedException();
}
