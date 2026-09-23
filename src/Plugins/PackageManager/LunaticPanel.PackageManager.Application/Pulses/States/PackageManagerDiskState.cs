using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record PackageManagerDiskState : IStateFeatureSingleton
{
    public bool IsLoading { get; init; }
    public bool IsInitialized { get; init; }
    public IEnumerable<PackagePayload> PendingUpdates { get; init; } = Array.Empty<PackagePayload>();
    public IEnumerable<PackagePayload> AvailableRollbacks { get; init; } = Array.Empty<PackagePayload>();

    public IEnumerable<PackagePayload> PendingDelete { get; init; } = Array.Empty<PackagePayload>();

    /// <summary>
    /// Only LPKG contained into to installed folder not the actual decompiled package.
    /// </summary>
    public IEnumerable<PackagePayload> Installed { get; init; } = Array.Empty<PackagePayload>();
    public IEnumerable<PackagePayload> RuntimePackages { get; init; } = Array.Empty<PackagePayload>();
    public IEnumerable<PackagePayload> PreInstalled { get; init; } = Array.Empty<PackagePayload>();
}
