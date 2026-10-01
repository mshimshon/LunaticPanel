using LuncaticPanel.Package.Server.Application.Payloads.Enums;

namespace LuncaticPanel.Package.Server.Application.Payloads.Responses;

public sealed record PackageValidationResponse
{
    public string Target { get; init; } = default!;
    public PackageValidationLocation Location { get; init; } = default!;
    public string ValidatorVersion { get; init; } = default!;
    public string ValidatorSource { get; init; } = default!;
    public DateTime ValidatedAt { get; init; } = DateTime.UtcNow;
    public ManifestPayload Manifest { get; init; } = default!;
}
