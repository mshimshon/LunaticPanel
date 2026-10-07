namespace LunaticPanel.DebugTool.Payloads;

public sealed record PostProcessingComposePayload
{
    public string? ImportFrom { get; set; }
    public string? Command { get; set; }
    public string? DotnetProject { get; set; }
    public string? BuildTo { get; set; }
    public string? PublishTo { get; set; }
    public string? PluginPackTo { get; set; }

    public string? Folder { get; set; }
    public string? FolderTo { get; set; }

    public string? File { get; set; }
    public string? FileTo { get; set; }

    public string? Archive { get; set; }
    public string? Snap { get; set; }
}
