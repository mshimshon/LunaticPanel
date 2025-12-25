namespace LunaticPanel.Core.Messaging.EngineBus;

public interface IEngineBusHandler
{
    Task<EngineBusResponse> HandleAsync(IEngineBusMessage engineBusMessage);

}
