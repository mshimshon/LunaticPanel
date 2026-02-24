using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Messaging.EventScheduledBus;

public class EventScheduledBusReceiver : IEventScheduledBusReceiver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventScheduledBusRegistry _eventScheduledBusRegistry;
    private readonly ICircuitRegistry _circuitRegistry;

    public EventScheduledBusReceiver(IServiceProvider serviceProvider, IEventScheduledBusRegistry eventScheduledBusRegistry, ICircuitRegistry circuitRegistry)
    {
        _serviceProvider = serviceProvider;
        _eventScheduledBusRegistry = eventScheduledBusRegistry;
        _circuitRegistry = circuitRegistry;
    }
    public Task<EventScheduledBusMessageResponse?> IncomingMessageAsync(IEventScheduledBusMessage msg, CancellationToken cancellationToken = default)
    {
        string id = msg.GetId();
        var registry = _eventScheduledBusRegistry;
        Func<CancellationToken, Task> actionTask = (cancellationToken) => Task.CompletedTask;
        try
        {
            var handler = registry.GetRegistryFor(id);
            if (handler == default) return Task.FromResult<EventScheduledBusMessageResponse?>(default);

            try
            {
                var handlerService = (_serviceProvider.GetRequiredService(handler.HandlerType) as IEventScheduledBusHandler)!;
                var data = handlerService.DueToExecute(msg, cancellationToken);
                actionTask = data.Action;
                _ = Task.Run(() => actionTask(cancellationToken));
                var result = new EventScheduledBusMessageResponse(data)
                {
                    Origin = msg.GetId()
                }; // TODO: Find wtf is the Origin
                return Task.FromResult<EventScheduledBusMessageResponse?>(result);
            }
            catch (EventScheduledBusMessageException ex)
            {
                var result = new EventScheduledBusMessageResponse(new(actionTask))
                {
                    Error = ex,
                    Origin = handler.HandlerType.FullName!
                };
                // TODO: OPEN TELEMETRY? OR CAP EVENT
                return Task.FromResult<EventScheduledBusMessageResponse?>(result);
            }
            catch (Exception ex)
            {
                // TODO: RESCHEDULE ACCORDING TO INITAL SCHEDULING VALUES
                var result = new EventScheduledBusMessageResponse(new(actionTask))
                {
                    Error = new("INTERNAL", ex.Message),
                    Origin = handler.HandlerType.FullName!
                };
                return Task.FromResult<EventScheduledBusMessageResponse?>(result);
            }
        }
        catch (Exception ex)
        {
            var result = new EventScheduledBusMessageResponse(new(actionTask))
            {
                Error = new("INTERNAL", ex.Message),
                Origin = id
            };
            return Task.FromResult<EventScheduledBusMessageResponse?>(result);
        }
    }
    public IReadOnlyCollection<string> WhatDoYouListenFor() => _eventScheduledBusRegistry.GetAllAvailableIds();
    public bool DoYouListenTo(string key) => _eventScheduledBusRegistry.HasKey(key);

}
