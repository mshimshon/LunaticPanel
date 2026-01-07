using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;

namespace LunaticPanel.Core.Messaging.EngineBus;

public class EngineBus : IEngineBus
{
    private readonly IEngineBusExchange _engineBusExchange;

    public EngineBus(IEngineBusExchange engineBusExchange)
    {
        _engineBusExchange = engineBusExchange;
    }

    public Task<EngineBusResponse[]> ExecAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default)
        => _engineBusExchange.ExchangeAsync(engineBusRender, cancellationToken);


    public IReadOnlyCollection<string> GetAvailableKeys() => _engineBusExchange.GetAvailableKeys();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _engineBusExchange.AnyListenerFor(key);



}
