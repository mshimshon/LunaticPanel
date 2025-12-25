using LunaticPanel.Core.Messaging.Common;

namespace LunaticPanel.Core.Messaging.EngineBus;

public sealed class EngineBusMessage : IEngineBusMessage
{
    private readonly BusMessageData? _data;
    private readonly Guid _messageId;
    private readonly string _baseId;
    private readonly string _type;
    //private readonly RenderFragment _renderFragment;

    public EngineBusMessage(string baseId, string type)
    {
        _messageId = Guid.NewGuid();
        _baseId = baseId;
        _type = type;
    }

    public EngineBusMessage(string baseId, string type, object data) : this(baseId, type)
    {
        _data = new(data);
    }

    public string GetId() => $"{_baseId}.{_type}";

    public BusMessageData? GetData() => _data;
    public Guid GetMessageId() => _messageId;

    //public Task<RenderFragment> GetRender() => Task.FromResult(_renderFragment);
}
