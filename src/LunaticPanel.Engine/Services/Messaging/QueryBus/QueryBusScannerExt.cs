using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application.Messaging.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LunaticPanel.Engine.Services.Messaging.QueryBus;

public static class QueryBusScannerExt
{
    private static Queue<(string, Type)> ToRuntimeRegister { get; set; } = new();
    public static void ScanQueryBusHandlers(this IServiceCollection services, Assembly[] assembly)
    {
        var handlerType = typeof(IQueryBusHandler);

        var toRegister = assembly.SelectMany(p => p.GetTypes())
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsGenericTypeDefinition &&
                handlerType.IsAssignableFrom(t))
            .Select(t =>
            {
                var attr = t.GetCustomAttribute<QueryBusIdAttribute>(inherit: false);
                if (attr == default)
                    throw new InvalidOperationException($"Type {t.FullName} MUST implements {nameof(QueryBusIdAttribute)}.");
                return (attr.QueryId, t);
            }).ToList();
        foreach (var item in toRegister)
        {
            services.AddScoped(item.t);
            ToRuntimeRegister.Enqueue(item);
        }
    }

    public static void RegisterScannedQueryBusHandlers(this WebApplication app)
    {
        if (ToRuntimeRegister.Count <= 0) return;
        var registry = app.Services.GetRequiredService<IQueryBusRegistry>();
        do
        {
            var item = ToRuntimeRegister.Dequeue();
            registry.Register(item.Item1, item.Item2);
        } while (ToRuntimeRegister.Count > 0);
    }
}
