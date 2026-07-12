using LuncaticPanel.Package.Server.Web.Payloads.Enums;

namespace LuncaticPanel.Package.Server.Web.Payloads;

public sealed record CodedException

{
    public ExceptionProvenencePayload Provenence { get; } = ExceptionProvenencePayload.Unknown;
    public string Code { get; } = default!;
    public string Message { get; } = default!;
    public CodedException(string code, string message, ExceptionProvenencePayload provenence)
    {
        Code = code;
        Message = message;
        Provenence = provenence;
    }

}
