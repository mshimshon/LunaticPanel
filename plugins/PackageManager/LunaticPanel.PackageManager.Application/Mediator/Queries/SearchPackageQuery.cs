using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record SearchPackageQuery : IRequest<SearchResponse<PackageInfoPayload>>
{
    public string Keywords { get; set; } = string.Empty;
}
