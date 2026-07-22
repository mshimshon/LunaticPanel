namespace LunaticPanel.DebugTool.Payloads;

internal sealed class ComposePayload
{
    public List<string> Apt { get; set; } = new();
    public Dictionary<string, ServiceComposePayload> Services { get; set; } = new();
    public List<PluginComposePayload> Plugins { get; set; } = new();
}
