namespace LunaticPanel.DebugTool.Payloads;

internal sealed record PluginComposePayload
{
    public string? Local { get; set; }      // local build
    public bool Enabled { get; set; }
    public string? Id { get; set; }         // remote plugin id
    public string? Version { get; set; }    // optional
    public string? Source { get; set; }     // remote repo URL
}
