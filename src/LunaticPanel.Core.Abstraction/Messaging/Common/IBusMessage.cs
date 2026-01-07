namespace LunaticPanel.Core.Abstraction.Messaging.Common;

public interface IBusMessage
{
    string GetId();
    BusMessageData? GetData();
    Guid GetMessageId();
    Guid? GetOriginCircuitId();
    void SetOriginCircuitId(Guid id);
}
