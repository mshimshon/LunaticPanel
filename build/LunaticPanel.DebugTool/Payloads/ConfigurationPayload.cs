namespace LunaticPanel.DebugTool.Payloads;

internal sealed record ConfigurationPayload
{
    public string WorkingDir { get; init; } = default!;
    public bool DebugMode { get; init; }
    public bool SkipSubSystemRebuild { get; init; }
    public bool SkipServiceRebuild { get; init; }
    public bool PerformSoftCleanup { get; init; }
    public bool PerformCleanup { get; init; }
    public bool PerformDeploy { get; init; }
    public bool NoInteraction { get; init; }
    public ComposePayload Compose { get; init; } = default!;


    public void PrintDebug(string line)
    {
        if (DebugMode) Console.WriteLine(line);
    }
}
