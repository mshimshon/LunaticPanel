using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record PackageManagerUpdateState : IStateFeatureSingleton
{
    public IEnumerable<PackagePayload> AvailableUpdates { get; init; } = Array.Empty<PackagePayload>();
    public bool AvailableUpdatesLoading { get; init; }

    public IEnumerable<PackagePayload> AvailableRollbacks { get; init; } = Array.Empty<PackagePayload>();
    public bool AvailableRollbacksLoading { get; init; }

    public bool CurrentlyUpdating { get; init; }

}
