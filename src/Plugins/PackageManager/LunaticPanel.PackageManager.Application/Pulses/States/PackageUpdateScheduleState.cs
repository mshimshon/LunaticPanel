using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record PackageUpdateScheduleState : IStateFeatureSingleton
{
    public PackageManagerConfigurationResponse Configuration { get; init; } = new();
    public IEnumerable<PackagePayload> ToUpdate { get; init; } = new List<PackagePayload>();
    public IEnumerable<PackagePayload> IgnoreUpdatesFrom { get; init; } = new List<PackagePayload>();
}
