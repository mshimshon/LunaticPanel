namespace LunaticPanel.Package.Server.Application.Payloads.Requests;

public record PackageUploadRequest
{
    public string Provider { get; set; } = default!;
    public string ConfirmationToken { get; set; } = default!;
    public string AssetName { get; set; } = default!;
    public string AssetLocation { get; set; } = default!;
    public ManifestPayload Manifest { get; set; } = default!;
}
