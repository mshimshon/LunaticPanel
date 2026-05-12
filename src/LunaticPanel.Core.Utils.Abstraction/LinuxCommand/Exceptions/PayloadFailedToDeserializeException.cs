namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Exceptions;

public class PayloadFailedToDeserializeException : Exception
{
    public PayloadFailedToDeserializeException(string? payload) : this(payload, "Couldn't deserialize the payload.")
    {
    }

    public PayloadFailedToDeserializeException(string? payload, string message) : base(message)
    {
        Payload = payload;
    }

    public string? Payload { get; }
}
