using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record MoveDownSourceAction : IAction
{
    public RepositorySourcePayload Source { get; set; } = default!;
}
