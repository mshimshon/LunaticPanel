using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record InstallNextUpdateAction : ISafeAction
{
    public PackagePayload Package { get; set; } = default!;
}
