using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record SearchRepositoryQuery : IRequest<SearchResponse<PackageInfoPayload>>
{
    public List<RepositorySourcePayload> Sources { get; set; } = new();
    public string Keywords { get; set; } = string.Empty;

}
