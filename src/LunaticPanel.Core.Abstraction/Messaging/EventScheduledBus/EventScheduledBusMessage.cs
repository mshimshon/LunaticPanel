using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;

public sealed class EventScheduledBusMessage : IEventScheduledBusMessage
{
    private readonly MessageKey _messageKey;

    public BusMessageData? Data { get; }
    public bool EnableTicker { get; init; }
    private long _currentTick = long.MinValue;
    public Guid Id { get; }
    private Guid? _circuitId;
    public List<Guid>? TargetCircuits { get; init; }
    public EventScheduledBusMessage(MessageKey messageKey, object? data = default)
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

    public long GetTick() => _currentTick;
    public bool HasTickerEnabled() => EnableTicker;
    public long SetTick(long current) => _currentTick = current;
    public IReadOnlyList<Guid>? GetTargets() => TargetCircuits?.AsReadOnly() ?? default;
}
