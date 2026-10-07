namespace LunaticPanel.DebugTool.Payloads;

internal sealed record BootstrapExternalPayload
{
    public List<BootstrapPluginExternalPayload> KnownPlugins { get; set; } = new();
}
