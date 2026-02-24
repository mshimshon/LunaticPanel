using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using System.Reflection;

namespace LunaticPanel.Core.Messaging;

public static class BusScannerExt
{
    public static List<BusHandlerDescriptor> ScanBusHandlers(Action<BusHandlerDescriptor> onDescriptor, params Assembly[] toScan)
    {
        var engineBusType = typeof(IEngineBusHandler);
        var eventBusType = typeof(IEventBusHandler);
        var eventScheduledBusType = typeof(IEventScheduledBusHandler);
        var queryBusType = typeof(IQueryBusHandler);

        var assembly = toScan.SelectMany(p => p.GetTypes());
        List<BusHandlerDescriptor> toRegister = assembly
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t =>
                engineBusType.IsAssignableFrom(t) ||
                eventBusType.IsAssignableFrom(t) ||
                queryBusType.IsAssignableFrom(t) ||
                eventScheduledBusType.IsAssignableFrom(t))
            .Select(t =>
            {
                var attr = t.GetCustomAttribute<BusIdAttribute>(inherit: false);
                if (attr == default)
                    throw new InvalidOperationException($"Type {t.FullName} MUST implements {nameof(BusIdAttribute)}.");

                var eventType = EBusType.EngineBus;
                if (eventBusType.IsAssignableFrom(t))
                    eventType = EBusType.EventBus;
                else if (queryBusType.IsAssignableFrom(t))
                    eventType = EBusType.QueryBus;
                else if (eventScheduledBusType.IsAssignableFrom(t))
                    eventType = EBusType.EventScheduledBus;
                var result = new BusHandlerDescriptor(attr.Key.ToString(), t, eventType, attr.ServiceLifetime);
                onDescriptor.Invoke(result);
                return result;
            }).ToList();



        return toRegister;
    }

}
