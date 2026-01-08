using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Messaging.EngineBus;

public class EngineBusReceiver : IEngineBusReceiver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEngineBusRegistry _engineBusRegistry;

    public EngineBusReceiver(IServiceProvider serviceProvider, IEngineBusRegistry engineBusRegistry)
    {
        _serviceProvider = serviceProvider;
        _engineBusRegistry = engineBusRegistry;
    }

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
    public IReadOnlyCollection<string> WhatDoYouListenFor() => _engineBusRegistry.GetAllAvailableIds();
    public bool DoYouListenTo(string key) => _engineBusRegistry.HasKey(key);
}
