using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record MoveUpSourceAction : IAction
{
    public RepositorySourcePayload Source { get; set; } = default!;
}
