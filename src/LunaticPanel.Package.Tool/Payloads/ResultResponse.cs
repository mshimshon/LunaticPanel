namespace LunaticPanel.Package.Tool.Payloads;

public sealed record ResultResponse
{
    public object? Data { get; set; }
    public ErrorResponse? Error { get; set; }
}
