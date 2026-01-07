using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;

namespace LunaticPanel.Engine.Infrastructure.Messaging.EngineBus;

internal class EngineBusExchange : IEngineBusExchange
{
    private readonly ICircuitRegistry _circuitRegistry;
    private readonly IEngineBusReceiver _engineBusReceiver;
    private Guid CircuitId => _circuitRegistry.CurrentCircuit.CircuitId;
    public EngineBusExchange(ICircuitRegistry circuitRegistry, IEngineBusReceiver engineBusReceiver)
    {
        _circuitRegistry = circuitRegistry;
        _engineBusReceiver = engineBusReceiver;
    }
    public bool AnyListenerFor(string key)
    {
        key = key.ToLower();
        var result = _engineBusReceiver.DoYouListenTo(key);
        if (result) return result;
        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            if (item.Key.CircuitId != CircuitId) continue;

            var pluginBusReceiver = item.Value.GetRequired<IEngineBusReceiver>();
            result = pluginBusReceiver.DoYouListenTo(key);
            if (result) return result;
        }
        return false;
    }
    public async Task<EngineBusResponse[]> ExchangeAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default)
    {
        engineBusRender.SetOriginCircuitId(CircuitId);
        var hostHandlers = await _engineBusReceiver.IncomingMessageAsync(engineBusRender, cancellationToken);
        List<EngineBusResponse> result = [.. hostHandlers];
        var contexts = _circuitRegistry.GetPluginContexts();
        foreach (var item in contexts)
        {
            if (item.Key.CircuitId != CircuitId) continue;
            var pluginBusReceiver = item.Value.GetRequired<IEngineBusReceiver>();
            var handlerResults = await pluginBusReceiver.IncomingMessageAsync(engineBusRender, cancellationToken);
            result.AddRange(handlerResults);

        }
        return result.ToArray();
    }
    public IReadOnlyCollection<string> GetAvailableKeys()
    {
        var hostListeningList = _engineBusReceiver.WhatDoYouListenFor();
        List<string> result = new();
        result.AddRange(hostListeningList);
        foreach (var item in _circuitRegistry.GetPluginContexts())
        {
            if (item.Key.CircuitId != CircuitId) continue;
            var pluginBusReceiver = item.Value.GetRequired<IEngineBusReceiver>();
            var handlerResults = pluginBusReceiver.WhatDoYouListenFor();
            result.AddRange(handlerResults.Where(p => !result.Contains(p)));

        }
        return result.AsReadOnly();
    }
}
