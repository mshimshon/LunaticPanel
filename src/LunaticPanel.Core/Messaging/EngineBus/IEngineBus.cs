namespace LunaticPanel.Core.Messaging.EngineBus;

public interface IEngineBus
{
    Task<EngineBusResponse[]> ExecAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default);

}
