using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.QuerySystem;

public sealed class QueryBusMessage : IQueryBusMessage
{
    private readonly MessageKey _messageKey;

    public Guid TargetCircuit { get; init; }
    public BusMessageData? Data { get; }
    public Guid Id { get; }
    private Guid? _circuitId;
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
    public Guid? GetOriginCircuitId() => _circuitId;
    public void SetOriginCircuitId(Guid id)
    {
        if (_circuitId == default) _circuitId = id;
    }
}
