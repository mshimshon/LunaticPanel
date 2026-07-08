namespace LuncaticPanel.Package.Server.Application.Payloads.Requests;

public sealed record ManifestSearchRequest
{
    public string? Keywords { get; set; }
    public string? PackageId { get; set; }
    public int? PanelVersion { get; set; }
    public int Position { get; set; }
    public int MaxResult { get; set; } = 50;
}
