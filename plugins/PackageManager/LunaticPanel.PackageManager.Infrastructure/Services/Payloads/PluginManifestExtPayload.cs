namespace LunaticPanel.PackageManager.Infrastructure.Services.Payloads;

public sealed record PluginManifestExtPayload
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Company { get; set; }
    public string Version { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Author { get; set; }
    public string? Copyright { get; set; }
    public string PanelVersion { get; set; } = default!;
    public string DotnetVersion { get; set; } = default!;
    public string PluginEntryFile { get; set; } = default!;
}
