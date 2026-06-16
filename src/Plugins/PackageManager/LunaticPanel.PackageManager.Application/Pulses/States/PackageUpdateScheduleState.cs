using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record PackageUpdateScheduleState : IStateFeatureSingleton
{
    public PackageManagerConfigurationResponse Configuration { get; init; } = new();
    public PackagePayload? CurrentlyUpdating { get; init; }
    public IEnumerable<PackagePayload> ToUpdate { get; init; } = new List<PackagePayload>();
    public IEnumerable<PackagePayload> CancelledRequests { get; init; } = new List<PackagePayload>();

}
