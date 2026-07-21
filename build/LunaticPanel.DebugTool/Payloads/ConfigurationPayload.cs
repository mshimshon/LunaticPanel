namespace LunaticPanel.DebugTool.Payloads;

internal sealed record ConfigurationPayload
{
    public bool SkipSubSystemRebuild { get; init; }
    public ComposePayload Compose { get; init; } = default!;
}
