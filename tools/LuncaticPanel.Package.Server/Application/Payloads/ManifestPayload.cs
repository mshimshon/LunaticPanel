using LuncaticPanel.Package.Server.Application.Payloads.Enums;

namespace LuncaticPanel.Package.Server.Application.Payloads;

public sealed record ManifestPayload
{
    public string Id { get; init; } = default!;
    public string Version { get; init; } = default!;
    public string PanelVersion { get; init; } = default!;
    public string DotnetVersion { get; init; } = default!;
    public string PluginEntryFile { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string Author { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Copyright { get; init; }
    public ManifestStatusPayload Status { get; init; } = ManifestStatusPayload.Visible;
    public string? EndOfLifeMessage { get; init; }
}
