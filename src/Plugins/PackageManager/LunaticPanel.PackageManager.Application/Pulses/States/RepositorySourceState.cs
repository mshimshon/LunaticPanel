using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record RepositorySourceState : IStateFeatureSingleton
{
    public IEnumerable<RepositorySourcePayload> Sources { get; init; } = Array.Empty<RepositorySourcePayload>();
    public bool SourcesLoading { get; init; }
    public bool SourceSaving { get; init; }

}
