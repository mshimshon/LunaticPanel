using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record BootstrapState : IStateFeatureSingleton
{
    public IEnumerable<string> Enabled { get; init; } = Array.Empty<string>();
    public IEnumerable<string> Disabled { get; init; } = Array.Empty<string>();
    public bool IsLoading { get; init; }
    public bool IsRestartNeeded { get; init; }
}
