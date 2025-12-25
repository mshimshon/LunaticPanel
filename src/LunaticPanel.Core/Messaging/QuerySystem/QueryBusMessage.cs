using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.QuerySystem;

public sealed class QueryBusMessage : IQueryBusMessage
{

    public BusMessageData? Data { get; }
    public string EventBaseId { get; }
    public string EventType { get; }
    public Guid Id { get; }
    public QueryBusMessage(string queryBaseId, string queryType, object? data = default)
    {
        Id = Guid.NewGuid();
        if (data != default)
            Data = new BusMessageData(data);
        EventBaseId = queryBaseId;
        EventType = queryType;
    }

    public string GetId() => $"{EventBaseId}.{EventType}";

    public BusMessageData? GetData() => Data;
    public Guid GetMessageId() => Id;
}
