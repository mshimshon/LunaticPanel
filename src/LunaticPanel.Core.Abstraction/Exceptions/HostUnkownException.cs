namespace LunaticPanel.Core.Abstraction.Exceptions;

public class HostUnkownException : HostCodedException
{
    public HostUnkownException() : base(nameof(HostUnkownException), "Unknown exception has occured.")
    {
    }
}
