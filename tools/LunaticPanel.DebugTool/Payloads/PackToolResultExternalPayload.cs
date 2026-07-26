namespace LunaticPanel.DebugTool.Payloads;

internal sealed record PackToolResultExternalPayload
{
    public PackToolPluginManifestExternalPayload? Data { get; set; }

}
