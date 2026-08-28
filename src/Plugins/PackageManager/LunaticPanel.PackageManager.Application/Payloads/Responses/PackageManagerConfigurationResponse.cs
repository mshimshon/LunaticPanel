namespace LunaticPanel.PackageManager.Application.Payloads.Responses;

public sealed record PackageManagerConfigurationResponse
{
    public int UpdateRunnerInactiveFrequencySeconds { get; init; } = 30;
    public int UpdateRunnerActiveFrequencySeconds { get; init; } = 5;
    public bool AutoRestart { get; init; }
}
