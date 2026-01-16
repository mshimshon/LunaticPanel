namespace LunaticPanel.Core.Abstraction.Tools.LinuxCommand;

public sealed record LinuxCommandResult
{
    public string StandardOutput { get; init; } = default!;
    public string StandardError { get; init; } = default!;
    public bool Failed => string.IsNullOrWhiteSpace(StandardOutput) && !string.IsNullOrWhiteSpace(StandardError);

}
