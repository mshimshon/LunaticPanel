using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record SearchPackageState : IStateFeature
{
    public bool IsLoading { get; init; }
    public IReadOnlyDictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>> Search { get; init; } = new Dictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>>();

}
