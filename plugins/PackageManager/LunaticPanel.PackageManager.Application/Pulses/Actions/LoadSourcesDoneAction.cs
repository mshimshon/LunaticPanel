using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed class LoadSourcesDoneAction : IAction
{
    public List<RepositorySourcePayload>? Sources { get; set; }
}
