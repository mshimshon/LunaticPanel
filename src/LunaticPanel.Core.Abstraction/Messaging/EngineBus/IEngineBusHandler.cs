namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public interface IEngineBusHandler
{
    Task<EngineBusResponse> HandleAsync(IEngineBusMessage engineBusMessage);

}
