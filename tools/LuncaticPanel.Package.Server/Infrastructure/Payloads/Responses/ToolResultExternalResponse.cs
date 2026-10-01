namespace LuncaticPanel.Package.Server.Infrastructure.Payloads.Responses;

internal sealed record ToolResultExternalResponse<T>
{
    public T? Data { get; set; }
    public ToolErrorExternalResponse? Error { get; set; }
}
