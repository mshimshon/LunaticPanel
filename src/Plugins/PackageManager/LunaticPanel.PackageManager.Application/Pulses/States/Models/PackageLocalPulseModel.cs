using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Application.Pulses.States.Models;

public sealed record PackageLocalPulseModel
{
    public PackagePayload Package { get; init; } = default!;
    public PackagePayload? Update { get; init; }
    public bool IsUpdateLoading { get; init; }
    public PackagePayload? Rollback { get; init; }
    public bool IsRollingBackLoading { get; init; }
}
