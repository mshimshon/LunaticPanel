using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record RepositorySourceState : IStateFeatureSingleton
{
    public IEnumerable<RepositorySourcePayload> AvailableSources { get; init; } = Array.Empty<RepositorySourcePayload>();
    public bool AvailableSourcesLoading { get; set; }

    public IEnumerable<RepositorySourcePayload> EnabledSources { get; init; } = Array.Empty<RepositorySourcePayload>();
    public bool EnabledSourcesLoading { get; set; }

    public IEnumerable<RepositorySourcePayload> DisabledSources { get; init; } = Array.Empty<RepositorySourcePayload>();
    public bool DisabledSourcesLoading { get; set; }

}
