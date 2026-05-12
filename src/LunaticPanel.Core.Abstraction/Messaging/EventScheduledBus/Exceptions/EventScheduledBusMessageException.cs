using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus.Exceptions;

public class EventScheduledBusMessageException : Exception
{
    public string? Code { get; }
    public BusMessageData? ErrorData { get; }
    public EventScheduledBusMessageException(string code, string message) : base(message)
    {
        Code = code;
    }
    public EventScheduledBusMessageException(string code, BusMessageData errorData, string message) : this(code, message)
    {
        ErrorData = errorData;
    }

}
