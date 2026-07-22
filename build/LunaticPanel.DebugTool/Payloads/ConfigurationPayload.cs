namespace LunaticPanel.DebugTool.Payloads;

internal sealed record ConfigurationPayload
{
    public string WorkingDir { get; init; } = default!;
    public bool SkipSubSystemRebuild { get; init; }
    public bool SkipServiceRebuild { get; init; }
    public bool NoInteraction { get; init; }
    public ComposePayload Compose { get; init; } = default!;
}
