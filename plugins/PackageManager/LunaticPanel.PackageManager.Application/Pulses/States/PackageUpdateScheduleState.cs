using LunaticPanel.PackageManager.Application.Payloads.Responses;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record PackageUpdateScheduleState : IStateFeatureSingleton
{
    public PackageManagerConfigurationResponse Configuration { get; init; } = new();
}
