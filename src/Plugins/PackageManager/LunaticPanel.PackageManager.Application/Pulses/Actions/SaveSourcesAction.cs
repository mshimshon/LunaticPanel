using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public class SaveSourcesAction : IAction
{
    public List<RepositorySourcePayload> Sources { get; set; } = default!;
}
