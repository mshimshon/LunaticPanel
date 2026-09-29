namespace LunaticPanel.PackageManager.Application.Payloads.Responses;

public sealed record PackageManagerConfigurationResponse
{
    public int UpdateRunnerInactiveFrequencySeconds { get; init; } = 900;
    public int UpdateRunnerActiveFrequencySeconds { get; init; } = 15;

    public bool AutoRestart { get; init; }
}
