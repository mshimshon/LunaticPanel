namespace LunaticPanel.Package.Tool.Payloads;

public sealed record ToolInformationPayload
{
    public string BoundToPanelVersion { get; set; } = default!;
    public string ToolVersion { get; set; } = default!;

}
