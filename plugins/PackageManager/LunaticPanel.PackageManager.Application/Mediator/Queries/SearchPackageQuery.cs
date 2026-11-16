using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record SearchPackageQuery : IRequest<SearchResponse<PackageInfoPayload>>
{
    public string Keywords { get; set; } = string.Empty;
}
