using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;
namespace LunaticPanel.PackageManager.Pulses.States;

public sealed record PackageUpdateState : IStateFeatureSingleton
{
    public IEnumerable<PackageManifestPayload> FoundUpdates { get; init; } = Array.Empty<PackageManifestPayload>();
    public bool IsLoading { get; init; }
}
