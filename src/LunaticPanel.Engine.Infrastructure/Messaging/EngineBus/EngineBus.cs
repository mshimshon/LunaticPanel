using LunaticPanel.Core;
using LunaticPanel.Core.Messaging.Common;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Application.Messaging.EngineBus;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Infrastructure.Messaging.EngineBus;

internal class EngineBus : IEngineBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEngineBusRegistry _engineBusRegistry;

    public EngineBus(IServiceProvider serviceProvider, IEngineBusRegistry engineBusRegistry)
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
            if (item.Plugin != default)
            {
                var pluginType = item.Plugin.GetType();
                var serviceType = typeof(IPluginService<>).MakeGenericType(pluginType);
                var pluginService = _serviceProvider.GetRequiredService(serviceType) as IPluginService;
                handlerTasks.Add(ExecuteHandler(engineBusRender, pluginService!, item.HandlerType));
            }
            else
                handlerTasks.Add(ExecuteHandler(engineBusRender, _serviceProvider, item.HandlerType));

        return Task.WhenAll(handlerTasks);
    }

    private static Task<EngineBusResponse> ExecuteHandler(IEngineBusMessage engineBusRenderMessage, IServiceProvider serviceProvider, Type handlerType)
    {
        var handler = serviceProvider.GetRequiredService(handlerType) as IEngineBusHandler;
        return handler!.HandleAsync(engineBusRenderMessage);
    }

    public IReadOnlyCollection<Type> GetAllHandlersByEventId(string eventId)
        => _engineBusRegistry.GetRegistryFor(eventId).Select(p => p.HandlerType).ToList().AsReadOnly();
    public IReadOnlyCollection<string> GetAvailableKeys() => _engineBusRegistry.GetAllAvailableIds();
    public bool HasKeyFor(MessageKey messageKey) => HasKeyFor(messageKey.ToString());
    public bool HasKeyFor(string key) => _engineBusRegistry.HasKey(key);
}
