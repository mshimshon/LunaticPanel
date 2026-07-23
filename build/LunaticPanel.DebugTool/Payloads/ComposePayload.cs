namespace LunaticPanel.DebugTool.Payloads;

internal sealed record ComposePayload
{
    private HashSet<string> _apt = new(StringComparer.OrdinalIgnoreCase);

    public HashSet<string> Apt
    {
        get => _apt;
        // If YamlDotNet tries to overwrite the reference entirely, 
        // we can intercept it or safely clear and copy.
        set => _apt = value != null ? new HashSet<string>(value, StringComparer.OrdinalIgnoreCase) : new(StringComparer.OrdinalIgnoreCase);
    }
    public Dictionary<string, ServiceComposePayload> Services { get; set; } = new();
    public List<PluginComposePayload> Plugins { get; set; } = new();
}
