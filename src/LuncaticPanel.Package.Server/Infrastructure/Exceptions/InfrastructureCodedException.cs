namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public abstract class InfrastructureCodedException : Exception
{
    protected InfrastructureCodedException(string code, string message) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
