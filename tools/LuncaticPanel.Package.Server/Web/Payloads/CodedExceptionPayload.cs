using LuncaticPanel.Package.Server.Web.Payloads.Enums;

namespace LuncaticPanel.Package.Server.Web.Payloads;

public sealed record CodedExceptionPayload

{
    public ExceptionProvenencePayload Provenence { get; } = ExceptionProvenencePayload.Unknown;
    public string Code { get; } = default!;
    public object? Data { get; } = default!;
    public string Message { get; } = default!;
    public CodedExceptionPayload(string code, string message, ExceptionProvenencePayload provenence)
    {
        Code = code;
        Message = message;
        Provenence = provenence;
    }

    public CodedExceptionPayload(string code, string message, object? data, ExceptionProvenencePayload provenence)
    {
        Code = code;
        Message = message;
        Data = data;
        Provenence = provenence;
    }

}
