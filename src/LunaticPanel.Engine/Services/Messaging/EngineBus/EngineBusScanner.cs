using LunaticPanel.Core.Messaging.EngineBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LunaticPanel.Engine.Services.Messaging.EngineBus;

public static class EngineBusScanner
{
    private static Queue<(string, Type)> ToRuntimeRegister { get; set; } = new();
    public static void ScanEngineBusHandlers(this IServiceCollection services, Assembly[] assembly)
    {
        var handlerType = typeof(IEngineBusHandler);

        var toRegister = assembly.SelectMany(p => p.GetTypes())
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
                return (attr.Id, t);
            }).ToList();
        foreach (var item in toRegister)
        {
            services.AddScoped(item.t);
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
            registry.Register(item.Item1, item.Item2);
        } while (ToRuntimeRegister.Count > 0);
    }
}
