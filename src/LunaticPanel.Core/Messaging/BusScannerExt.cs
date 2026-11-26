using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using System.Dynamic;
using System.Reflection;

namespace LunaticPanel.Core.Messaging;

public static class BusScannerExt
{



    public static List<BusHandlerDescriptor> ScanBusHandlers(Action<BusHandlerDescriptor> onDescriptor, params Type[] typesToScan)
    {
        var engineBusType = typeof(IEngineBusHandler);
        var eventBusType = typeof(IEventBusHandler);
        var eventScheduledBusType = typeof(IEventScheduledBusHandler);
        var queryBusType = typeof(IQueryBusHandler);
        var busesClass = typesToScan
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t =>
                engineBusType.IsAssignableFrom(t) ||
                eventBusType.IsAssignableFrom(t) ||
                queryBusType.IsAssignableFrom(t) ||
                eventScheduledBusType.IsAssignableFrom(t));
        List<BusHandlerDescriptor> toRegister = new();
        foreach (var t in busesClass)
        {
            var attr = t.GetCustomAttributes<BusKeyAttribute>(inherit: false).ToArray();
            if (attr == default || attr.Count() <= 0)
                throw new InvalidOperationException($"Type {t.FullName} MUST implements {nameof(BusKeyAttribute)}.");
            bool isValidEngineBus = attr.Any(p => p.GetType() == typeof(EngineBusKeyAttribute)) && engineBusType.IsAssignableFrom(t);
            bool isValidEventBus = attr.Any(p => p.GetType() == typeof(EventBusKeyAttribute)) && eventBusType.IsAssignableFrom(t);
            bool isValidQueryBus = attr.Any(p => p.GetType() == typeof(QueryBusKeyAttribute)) && queryBusType.IsAssignableFrom(t);
            bool isValidEventScheduleBus = attr.Any(p => p.GetType() == typeof(EventScheduledBusKeyAttribute)) && eventScheduledBusType.IsAssignableFrom(t);
            if (!isValidEngineBus && !isValidEventBus && !isValidQueryBus && !isValidEventScheduleBus)
                throw new InvalidOperationException($"Type {t.FullName} MUST implements {nameof(BusKeyAttribute)}.");
            foreach (var busAttribute in attr)
            {
                var eventType = EBusType.EngineBus;
                if (isValidEventBus) eventType = EBusType.EventBus;
                else if (isValidQueryBus) eventType = EBusType.QueryBus;
                else if (isValidEventScheduleBus) eventType = EBusType.EventScheduledBus;
                var result = new BusHandlerDescriptor(busAttribute.Key.ToString(), t, eventType, busAttribute.ServiceLifetime, busAttribute);
                onDescriptor.Invoke(result);
                toRegister.Add(result);
            }
        }
        return toRegister;
    }
    public static List<BusHandlerDescriptor> ScanBusHandlers(Action<BusHandlerDescriptor> onDescriptor, params Assembly[] toScan)
    {
        var assembly = toScan.SelectMany(p => p.GetTypes());
        return ScanBusHandlers(onDescriptor, assembly.ToArray());
    }

}
