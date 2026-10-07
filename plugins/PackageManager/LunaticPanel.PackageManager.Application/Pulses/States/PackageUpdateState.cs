using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record PackageUpdateState : IStateFeatureSingleton
{
    public IEnumerable<PackagePayload> FoundUpdates { get; init; } = Array.Empty<PackagePayload>();
    public bool IsLoading { get; init; }
}
