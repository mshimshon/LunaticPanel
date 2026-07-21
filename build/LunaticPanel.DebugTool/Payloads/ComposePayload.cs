namespace LunaticPanel.DebugTool.Payloads;

internal sealed class ComposePayload
{
    public int Debian { get; set; } = 13;
    public Dictionary<string, ServiceComposePayload> Services { get; set; } = new();
    public List<PluginComposePayload> Plugins { get; set; } = new();
}
