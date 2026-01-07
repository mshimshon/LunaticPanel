using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Messaging.EngineBus;

public class EngineBus : IEngineBus, IEngineBusReceiver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEngineBusRegistry _engineBusRegistry;
    private readonly IEngineBusExchange _engineBusExchange;

    public EngineBus(IServiceProvider serviceProvider, IEngineBusRegistry engineBusRegistry, IEngineBusExchange engineBusExchange)
    {
        _serviceProvider = serviceProvider;
        _engineBusRegistry = engineBusRegistry;
        _engineBusExchange = engineBusExchange;
    }

    public Task<EngineBusResponse[]> ExecAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default)
        => _engineBusExchange.ExchangeAsync(engineBusRender, cancellationToken);

    public Task<EngineBusResponse[]> IncomingMessageAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default)
    {
        string id = engineBusRender.GetId();
        var registry = _engineBusRegistry;
        var handlers = registry.GetRegistryFor(id);
        List<Task<EngineBusResponse>> handlerTasks = new();

        foreach (var item in handlers)
            handlerTasks.Add(ExecuteHandler(engineBusRender, item.HandlerType));

        return Task.WhenAll(handlerTasks);
    }

    private Task<EngineBusResponse> ExecuteHandler(IEngineBusMessage engineBusRenderMessage, Type handlerType)
    {
        var handler = _serviceProvider.GetRequiredService(handlerType) as IEngineBusHandler;
        return handler!.HandleAsync(engineBusRenderMessage);
    }

    public IReadOnlyCollection<string> GetAvailableKeys() => _engineBusExchange.GetAvailableKeys();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _engineBusExchange.AnyListenerFor(key);


    public IReadOnlyCollection<string> WhatDoYouListenFor() => _engineBusRegistry.GetAllAvailableIds();
    public bool DoYouListenTo(string key) => _engineBusRegistry.HasKey(key);
}
