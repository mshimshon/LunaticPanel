namespace LunaticPanel.Package.LocalServer.Infrastructure.Exceptions;

public class InfrastructureException : Exception
{
    public string Code { get; }
    public InfrastructureException(string code, string message) : base(message)
    {
        Code = code;
    }

}
