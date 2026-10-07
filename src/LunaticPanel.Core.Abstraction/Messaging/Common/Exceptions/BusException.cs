namespace LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;

public class BusException : Exception
{
    public BusException(string code, string message) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
