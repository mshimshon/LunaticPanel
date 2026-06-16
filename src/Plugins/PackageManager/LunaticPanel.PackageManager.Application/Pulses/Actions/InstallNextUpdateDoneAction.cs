using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record InstallNextUpdateDoneAction : IAction
{
    public PackagePayload? ToRemove { get; set; }
}
