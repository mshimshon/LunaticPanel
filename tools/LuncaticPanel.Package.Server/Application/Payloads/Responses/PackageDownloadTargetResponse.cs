namespace LuncaticPanel.Package.Server.Application.Payloads.Responses;

public sealed record PackageDownloadTargetResponse
{
    public string Target { get; init; } = default!;
}
