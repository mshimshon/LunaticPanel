using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record PackageRollbackDoneAction : IAction
{
    public ICollection<PackagePayload>? Packages { get; set; }
}
