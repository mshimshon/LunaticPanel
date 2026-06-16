using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record LoadPackageRollbackDoneAction : IAction
{
    public IEnumerable<PackagePayload>? UpdateRollback { get; set; }
}
