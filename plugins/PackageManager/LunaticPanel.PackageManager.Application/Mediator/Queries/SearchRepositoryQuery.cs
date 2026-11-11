using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record SearchRepositoryQuery : IRequest<Dictionary<RepositorySourcePayload, SearchResponse<PackageManifestPayload>>>
{
    public List<RepositorySourcePayload> Sources { get; set; } = new();
    public SearchRequest Search { get; set; } = default!;

}
