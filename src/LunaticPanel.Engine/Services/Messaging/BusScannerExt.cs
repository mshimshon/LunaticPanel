using LunaticPanel.Core;
using LunaticPanel.Core.Messaging.Common;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application.Messaging.EngineBus;
using LunaticPanel.Engine.Application.Messaging.Event;
using LunaticPanel.Engine.Application.Messaging.Query;
using LunaticPanel.Engine.Domain.Messaging.Entities;
using LunaticPanel.Engine.Domain.Messaging.Enums;
using LunaticPanel.Engine.Services.Plugin;
using System.Reflection;

namespace LunaticPanel.Engine.Services.Messaging;

public static class BusScannerExt
{
    public static Queue<BusHandlerDescriptorEntity> ToRuntimeRegister { get; set; } = new();
    public static List<BusHandlerDescriptorEntity> ScanBusHandlers(this IServiceCollection hostServices, IPlugin? plugin = default)
    {
        var engineBusType = typeof(IEngineBusHandler);
        var eventBusType = typeof(IEventBusHandler);
        var queryBusType = typeof(IQueryBusHandler);
        var assembly = plugin?.GetType()?.Assembly ?? typeof(BusScannerExt).Assembly;
        List<BusHandlerDescriptorEntity> toRegister = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => engineBusType.IsAssignableFrom(t) || eventBusType.IsAssignableFrom(t) || queryBusType.IsAssignableFrom(t))
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

                return new BusHandlerDescriptorEntity(attr.Id, t, eventType, plugin);
            }).ToList();

        foreach (var item in toRegister)
            ToRuntimeRegister.Enqueue(item);
        return toRegister;
    }


    public static void RegisterScannedBusHandlers(this WebApplication app)
    {
        if (ToRuntimeRegister.Count <= 0) return;
        var eventBusregistry = app.Services.GetRequiredService<IEventBusRegistry>();
        var queryBusregistry = app.Services.GetRequiredService<IQueryBusRegistry>();
        var pluginRegistry = app.Services.GetRequiredService<PluginRegistry>();
        var engineBusregistry = app.Services.GetRequiredService<IEngineBusRegistry>();
        do
        {
            var item = ToRuntimeRegister.Dequeue();
            if (item.BusType == EBusType.EventBus)
                eventBusregistry.Register(item.Id, item);
            else if (item.BusType == EBusType.QueryBus)
                queryBusregistry.Register(item.Id, item);
            else engineBusregistry.Register(item.Id, item);
            if (item.Plugin != default)
                pluginRegistry.GetByEntryType(item.Plugin.GetType()).Services.AddTransient(item.HandlerType);
        } while (ToRuntimeRegister.Count > 0);
    }
}
