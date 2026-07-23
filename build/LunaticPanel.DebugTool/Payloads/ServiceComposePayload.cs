namespace LunaticPanel.DebugTool.Payloads;

internal sealed record ServiceComposePayload
{
    public string? ImportFrom { get; set; }
    public string? StartupParameters { get; set; }
    public List<string> Environment { get; set; } = new List<string>();
    public string? DotnetProject { get; set; }
    public string? DebUrl { get; set; }
    public string ExecStart { get; set; } = default!;
    public string WorkingDir { get; set; } = default!;
    public string Description { get; set; } = "My Custom Background Service";
    public List<string> DependsOn { get; set; } = new();
    public bool Show { get; set; }
}
