using CoreMap;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Abstraction.Tools.EventScheduler;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.EventScheduledBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Infrastructure.Messaging.EngineBus;
using LunaticPanel.Engine.Infrastructure.Messaging.Event;
using LunaticPanel.Engine.Infrastructure.Messaging.EventScheduled;
using LunaticPanel.Engine.Infrastructure.Messaging.Query;
using LunaticPanel.Engine.Infrastructure.Plugin;
using LunaticPanel.Engine.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Infrastructure;

public static class RegisterServicesExt
{

    public static IServiceCollection AddEngineInfrastructure(this IServiceCollection services)
    {
        services.AddEngineApplication();

        services.AddScoped<EngineBusExchange>();
        services.AddScoped<EventBusExchange>();
        services.AddScoped<QueryBusExchange>();
        services.AddScoped<EventScheduler>();
        services.AddSingleton<GlobalTickerService>();
        services.AddScoped<IEngineBus, EngineBus>();
        services.AddScoped<IEngineBusReceiver, EngineBusReceiver>();


        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IEventBusReceiver, EventBusReceiver>();

        services.AddScoped<IEventScheduledBus, EventScheduledBus>();
        services.AddScoped<IEventScheduledBusReceiver, EventScheduledBusReceiver>();

        services.AddScoped<IQueryBus, QueryBus>();
        services.AddScoped<IQueryBusReceiver, QueryBusReceiver>();

        services.AddSingleton<IPluginRegistry, PluginRegistry>();

        services.AddCoreMap(o => { o.Scope = CoreMap.Enums.ServiceScope.Transient; }, [
            typeof(RegisterServicesExt),
            typeof(Application.RegisterServicesExt)
            ]);

        return services;
    }

    public static IServiceCollection AddEngineInfrastructureRedirected(this IServiceCollection services)
    {
        services.AddScoped<IEngineBusExchange>(sp => sp.GetRequiredService<EngineBusExchange>());
        services.AddScoped<IEventScheduledBusExchange>(sp => sp.GetRequiredService<EventScheduledBusExchange>());
        services.AddScoped<IEventBusExchange>(sp => sp.GetRequiredService<EventBusExchange>());
        services.AddScoped<IQueryBusExchange>(sp => sp.GetRequiredService<QueryBusExchange>());
        services.AddSingleton<IGlobalTicker>(sp => sp.GetRequiredService<GlobalTickerService>());

        services.AddSingleton<IEventScheduler>(sp => sp.GetRequiredService<EventScheduler>());
        return services;
    }
    public static Task EngineInfrastructureRuntimeBeforePlugins(this IServiceProvider services)
    => Task.CompletedTask;
    public static Task EngineInfrastructureRuntimeAfterPlugins(this IServiceProvider services)
    {
        var scheduler = services.GetRequiredService<EventScheduler>();
        Task.Run(() => scheduler.Cycle());


        return Task.CompletedTask;
    }
}
