using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public class SaveSourcesDoneAction : IAction
{
    public List<RepositorySourcePayload>? Sources { get; set; }

}
