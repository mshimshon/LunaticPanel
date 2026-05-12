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
                bool isValidEngineBus = attr?.GetType() == typeof(EngineBusIdAttribute) && engineBusType.IsAssignableFrom(t);
                bool isValidEventBus = attr?.GetType() == typeof(EventBusIdAttribute) && eventBusType.IsAssignableFrom(t);
                bool isValidQueryBus = attr?.GetType() == typeof(QueryBusIdAttribute) && queryBusType.IsAssignableFrom(t);
                bool isValidEventScheduleBus = attr?.GetType() == typeof(EventScheduledBusIdAttribute) && eventScheduledBusType.IsAssignableFrom(t);

                if (attr == default || (!isValidEngineBus && !isValidEventBus && !isValidQueryBus && !isValidEventScheduleBus))
                    throw new InvalidOperationException($"Type {t.FullName} MUST implements {nameof(BusIdAttribute)}.");

                var eventType = EBusType.EngineBus;
                if (isValidEventBus) eventType = EBusType.EventBus;
                else if (isValidQueryBus) eventType = EBusType.QueryBus;
                else if (isValidEventScheduleBus) eventType = EBusType.EventScheduledBus;

                var result = new BusHandlerDescriptor(attr.Key.ToString(), t, eventType, attr.ServiceLifetime);
                onDescriptor.Invoke(result);
                return result;
            }).ToList();



        return toRegister;
    }

}
