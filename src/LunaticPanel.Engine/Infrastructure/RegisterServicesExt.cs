using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.EventScheduledBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Abstraction.Tools.EventScheduler;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.EventScheduledBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Core.Utils.Logging;
using LunaticPanel.Engine.Infrastructure.Implementations;
using LunaticPanel.Engine.Infrastructure.Implementations.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LunaticPanel.Engine.Infrastructure;

internal static class RegisterServicesExt
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<EngineBusExchange>();
        services.AddScoped<EventBusExchange>();
        services.AddScoped<QueryBusExchange>();
        services.AddScoped<EventScheduledBusExchange>();
        services.AddScoped<EventScheduler>();
        services.AddSingleton<IPluginInfo, HostInfo>();
        services.AddSingleton<GlobalTickerService>();
        services.AddScoped<IEngineBus, EngineBus>();
        services.AddScoped<IEngineBusReceiver, EngineBusReceiver>();


        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IEventBusReceiver, EventBusReceiver>();

        services.AddScoped<IEventScheduledBus, EventScheduledBus>();
        services.AddScoped<IEventScheduledBusReceiver, EventScheduledBusReceiver>();

        services.AddScoped<CrazyReportCircuit>();
        services.AddScoped<IQueryBus, QueryBus>();
        services.AddScoped<IQueryBusReceiver, QueryBusReceiver>();

        // services.AddSingleton<IPluginRegistry, PluginRegistry>();


        return services;
    }

    public static IServiceCollection AddInfrastructureRedirected(this IServiceCollection services)
    {
        services.AddScoped<ICrazyReportCircuit>((sp) => sp.GetRequiredService<CrazyReportCircuit>());
        services.AddScoped<IEngineBusExchange>(sp => sp.GetRequiredService<EngineBusExchange>());
        services.AddScoped<IEventScheduledBusExchange>(sp => sp.GetRequiredService<EventScheduledBusExchange>());
        services.AddScoped<IEventBusExchange>(sp => sp.GetRequiredService<EventBusExchange>());
        services.AddScoped<IQueryBusExchange>(sp => sp.GetRequiredService<QueryBusExchange>());
        services.AddSingleton<IGlobalTicker>(sp => sp.GetRequiredService<GlobalTickerService>());
        services.AddScoped<IEventScheduler>(sp => sp.GetRequiredService<EventScheduler>());
        return services;
    }
    public static Task InfrastructureRuntimeBeforePlugins(this IServiceProvider services)
    => Task.CompletedTask;
    public static Task InfrastructureRuntimeAfterPlugins(this IServiceProvider services)
    {

        var scheduler = services.GetRequiredService<EventScheduler>();
        Task.Run(() => scheduler.Cycle());
        return Task.CompletedTask;
    }
}
