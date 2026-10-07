namespace LunaticPanel.Core.Abstraction.Messaging.EngineBus;

public interface IEngineBusExchange
{
    Task<EngineBusResponse[]> ExchangeAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default);
    IReadOnlyCollection<string> GetAvailableKeys();
    bool AnyListenerFor(string key);
}
