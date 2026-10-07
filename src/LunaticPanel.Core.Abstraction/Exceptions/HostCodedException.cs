namespace LunaticPanel.Core.Abstraction.Exceptions;

public class HostCodedException : Exception
{
    public HostCodedException(string code, string message) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
