using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record SearchRepositoryQuery : IRequest<SearchResponse<PackageInfoPayload>>
{
    public List<RepositorySourcePayload> Sources { get; set; } = new();
    public SearchRequest Search { get; set; } = default!;

}
