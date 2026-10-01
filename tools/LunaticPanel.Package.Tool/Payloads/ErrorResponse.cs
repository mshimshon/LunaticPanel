namespace LunaticPanel.Package.Tool.Payloads;

public sealed record ErrorResponse
{
    public ErrorResponse(string code, string message)
    {
        Code = code;
        Message = message;
    }
    public ErrorResponse(Exception exception)
    {
        Code = exception.GetType().Name;
        Message = exception.Message;
    }

    public string Code { get; }
    public string Message { get; }
}
