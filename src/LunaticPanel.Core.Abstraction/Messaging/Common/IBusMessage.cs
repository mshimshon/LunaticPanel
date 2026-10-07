namespace LunaticPanel.Core.Abstraction.Messaging.Common;

public interface IBusMessage
{
    string GetKey();
    BusMessageData? GetData();
    Guid GetMessageId();
    Guid? GetOriginCircuitId();
    void SetOriginCircuitId(Guid id);

}
