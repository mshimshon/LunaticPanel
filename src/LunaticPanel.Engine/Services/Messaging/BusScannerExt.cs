using LunaticPanel.Core;
using LunaticPanel.Core.Messaging.Common;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application.Messaging.Event;
using LunaticPanel.Engine.Application.Messaging.Query;
using LunaticPanel.Engine.Domain.Messaging.Entities;
using LunaticPanel.Engine.Domain.Messaging.Enums;
using LunaticPanel.Engine.Services.Messaging.EngineBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LunaticPanel.Engine.Services.Messaging;

public static class BusScannerExt
{
    private static Queue<BusHandlerDescriptorEntity> ToRuntimeRegister { get; set; } = new();
    public static void ScanBusHandlers(this IServiceCollection services, IPlugin? plugin = default)
    {
        var engineBusType = typeof(IEngineBusHandler);
        var eventBusType = typeof(IEventBusHandler);
        var queryBusType = typeof(IQueryBus);
        var assembly = plugin?.GetType()?.Assembly ?? typeof(BusScannerExt).Assembly;
        var toRegister = assembly.GetTypes()
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
        {
            services.AddTransient(item.HandlerType);
            ToRuntimeRegister.Enqueue(item);
        }
    }


    public static void RegisterScannedBusHandlers(this WebApplication app)
    {
        if (ToRuntimeRegister.Count <= 0) return;
        var eventBusregistry = app.Services.GetRequiredService<IEventBusRegistry>();
        var queryBusregistry = app.Services.GetRequiredService<IQueryBusRegistry>();
        var engineBusregistry = app.Services.GetRequiredService<EngineBusRegistry>();
        do
        {
            var item = ToRuntimeRegister.Dequeue();
            if (item.BusType == EBusType.EventBus)
                eventBusregistry.Register(item.Id, item);
            else if (item.BusType == EBusType.QueryBus)
                queryBusregistry.Register(item.Id, item);
            else engineBusregistry.Register(item.Id, item);
        } while (ToRuntimeRegister.Count > 0);
    }
}
