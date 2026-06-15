using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record SearchRemotePackageAction : IAction
{
    public string Keywords { get; set; } = default!;
    public List<RepositorySourcePayload> Sources { get; set; } = new List<RepositorySourcePayload>();
}
