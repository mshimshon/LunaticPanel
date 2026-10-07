namespace LunaticPanel.DebugTool.Payloads;

internal sealed record PackToolPluginManifestExternalPayload
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Version { get; set; } = default!;
}
