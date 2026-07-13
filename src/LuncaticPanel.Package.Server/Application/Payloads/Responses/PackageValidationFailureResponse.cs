namespace LuncaticPanel.Package.Server.Application.Payloads.Responses;

public sealed record PackageValidationFailureResponse
{
    public string Code { get; init; } = default!;
    public string Message { get; init; } = default!;
}
