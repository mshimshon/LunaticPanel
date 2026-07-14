namespace LuncaticPanel.Package.Server.Infrastructure.Payloads;

internal sealed record ManifestExternalPayload
{
    public string PanelVersion { get; init; } = default!;

}
