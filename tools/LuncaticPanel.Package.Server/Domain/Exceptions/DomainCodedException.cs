namespace LuncaticPanel.Package.Server.Domain.Exceptions;

public abstract class DomainCodedException : Exception
{
    protected DomainCodedException(string code, string message) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
