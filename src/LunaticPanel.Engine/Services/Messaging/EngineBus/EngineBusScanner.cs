using LunaticPanel.Core;
using LunaticPanel.Core.Messaging.EngineBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LunaticPanel.Engine.Services.Messaging.EngineBus;

public static class EngineBusScanner
{
    private static Queue<EngineBusHandlerDescriptor> ToRuntimeRegister { get; set; } = new();
    public static void ScanEngineBusHandlers(this IServiceCollection services)
    {
        var handlerType = typeof(IEngineBusHandler);

        var toRegister = typeof(EngineBusScanner).Assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericTypeDefinition &&
                handlerType.IsAssignableFrom(t))
            .Select(t =>
            {
                var attr = t.GetCustomAttribute<EngineBusIdAttribute>(inherit: false);
                if (attr == default)
                    throw new InvalidOperationException($"Type {t.FullName} MUST implements {nameof(EngineBusIdAttribute)}.");
                return new EngineBusHandlerDescriptor(attr.Id, t, default);
            }).ToList();
        foreach (var item in toRegister)
        {
            services.AddScoped(item.HandlerType);
            ToRuntimeRegister.Enqueue(item);
        }
    }

    public static void ScanEngineBusHandlers(this IServiceCollection services, IPlugin plugin)
    {
        var handlerType = typeof(IEngineBusHandler);

        var toRegister = plugin.GetType().Assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericTypeDefinition &&
                handlerType.IsAssignableFrom(t))
            .Select(t =>
            {
                var attr = t.GetCustomAttribute<EngineBusIdAttribute>(inherit: false);
                if (attr == default)
                    throw new InvalidOperationException($"Type {t.FullName} MUST implements {nameof(EngineBusIdAttribute)}.");
                return new EngineBusHandlerDescriptor(attr.Id, t, plugin);
            }).ToList();
        foreach (var item in toRegister)
        {
            services.AddScoped(item.HandlerType);
            ToRuntimeRegister.Enqueue(item);
        }
    }

    public static void RegisterScannedEngineBusHandlers(this WebApplication app)
    {
        if (ToRuntimeRegister.Count <= 0) return;
        var registry = app.Services.GetRequiredService<EngineBusRegistry>();
        do
        {
            var item = ToRuntimeRegister.Dequeue();
            registry.Register(item.Id, item);
        } while (ToRuntimeRegister.Count > 0);
    }
}
