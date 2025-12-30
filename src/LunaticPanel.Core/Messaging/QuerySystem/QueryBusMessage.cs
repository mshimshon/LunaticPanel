using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.QuerySystem;

public sealed class QueryBusMessage : IQueryBusMessage
{
    private readonly MessageKey _messageKey;

    public BusMessageData? Data { get; }
    public Guid Id { get; }
    public QueryBusMessage(MessageKey messageKey, object? data = default)
    {
        Id = Guid.NewGuid();
        if (data != default)
            Data = new BusMessageData(data);
        _messageKey = messageKey;
    }

    public string GetId() => $"{_messageKey}";

    public BusMessageData? GetData() => Data;
    public Guid GetMessageId() => Id;
}
