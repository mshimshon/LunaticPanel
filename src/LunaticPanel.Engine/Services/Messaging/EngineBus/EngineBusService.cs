using LunaticPanel.Core.Messaging.EngineBus;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Services.Messaging.EngineBus;

internal class EngineBusService : IEngineBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EngineBusRegistry _engineBusRegistry;

    public EngineBusService(IServiceProvider serviceProvider, EngineBusRegistry engineBusRegistry)
    {
        _serviceProvider = serviceProvider;
        _engineBusRegistry = engineBusRegistry;
    }

    public Task<EngineBusResponse[]> ExecAsync(IEngineBusMessage engineBusRender, CancellationToken cancellationToken = default)
    {
        string id = engineBusRender.GetId();
        var registry = _engineBusRegistry;
        var handlers = registry.GetRegistryFor(id);
        List<Task<EngineBusResponse>> handlerTasks = new();
        foreach (var item in handlers)
            handlerTasks.Add(ExecuteHandler(engineBusRender, _serviceProvider, item));

        return Task.WhenAll(handlerTasks);
    }

    private static Task<EngineBusResponse> ExecuteHandler(IEngineBusMessage engineBusRenderMessage, IServiceProvider serviceProvider, Type handlerType)
    {
        var handler = serviceProvider.GetRequiredService(handlerType) as IEngineBusHandler;
        return handler!.HandleAsync(engineBusRenderMessage);
    }


    public IReadOnlyCollection<string> GetAllEventIds()
        => _engineBusRegistry.GetAllAvailableIds();

    public IReadOnlyCollection<Type> GetAllHandlersByEventId(string eventId)
        => _engineBusRegistry.GetRegistryFor(eventId).ToList().AsReadOnly();
}
