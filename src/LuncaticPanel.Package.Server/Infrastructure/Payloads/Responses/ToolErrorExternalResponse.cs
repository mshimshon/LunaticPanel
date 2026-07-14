namespace LuncaticPanel.Package.Server.Infrastructure.Payloads.Responses;

internal sealed record ToolErrorExternalResponse
{
    public string Code { get; init; } = default!;
    public string Message { get; init; } = default!;
}

