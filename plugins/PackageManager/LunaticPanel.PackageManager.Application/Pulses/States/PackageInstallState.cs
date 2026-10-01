using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record PackageInstallState : IStateFeatureSingleton
{
    public PackagePayload? Installing { get; init; }
    public bool RestartRequired { get; init; } // When any new package is installed.
}
