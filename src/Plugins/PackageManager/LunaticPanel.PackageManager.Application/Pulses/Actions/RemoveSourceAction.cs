using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record RemoveSourceAction : IAction
{
    public RepositorySourcePayload Source { get; set; } = default!;
}
