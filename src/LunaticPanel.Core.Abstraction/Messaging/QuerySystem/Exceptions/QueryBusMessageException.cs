using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem.Exceptions;

public class QueryBusMessageException : Exception
{
    public string? Code { get; }
    public BusMessageData? ErrorData { get; }
    public QueryBusMessageException(string code, string message) : base(message)
    {
        Code = code;
    }
    public QueryBusMessageException(string code, BusMessageData errorData, string message) : this(code, message)
    {
        ErrorData = errorData;
    }

}
