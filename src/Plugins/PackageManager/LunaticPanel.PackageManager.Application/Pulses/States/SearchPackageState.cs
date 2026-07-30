using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record SearchPackageState : IStateFeature
{
    public bool IsLoading { get; init; }
    public SearchResponse<PackageInfoPayload>? Search { get; init; }
}
