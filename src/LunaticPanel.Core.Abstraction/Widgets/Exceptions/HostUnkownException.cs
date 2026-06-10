namespace LunaticPanel.Core.Abstraction.Widgets.Exceptions;

public class HostUnkownException : HostCodedException
{
    public HostUnkownException() : base(nameof(HostUnkownException), "Unknown exception has occured.")
    {
    }
}
