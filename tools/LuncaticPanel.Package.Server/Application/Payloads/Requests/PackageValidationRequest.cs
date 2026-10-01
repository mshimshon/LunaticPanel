using LuncaticPanel.Package.Server.Application.Payloads.Enums;

namespace LuncaticPanel.Package.Server.Application.Payloads.Requests;

public sealed record PackageValidationRequest
{
    public string Target { get; init; } = default!;
    public PackageValidationLocation LocationType { get; init; }

}
