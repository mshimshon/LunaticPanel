using CoreMap;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Infrastructure.Messaging.EngineBus;
using LunaticPanel.Engine.Infrastructure.Messaging.Event;
using LunaticPanel.Engine.Infrastructure.Messaging.Query;
using LunaticPanel.Engine.Infrastructure.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Infrastructure;

public static class RegisterServicesExt
{
    public static IServiceCollection AddEngineInfrastructure(this IServiceCollection services)
    {
        services.AddEngineApplication();
        services.AddSingleton<IEngineBusRegistry, EngineBusRegistry>();
        services.AddScoped<IEngineBusExchange, EngineBusExchange>();

        services.AddSingleton<IEventBusRegistry, EventBusRegistry>();
        services.AddSingleton<IEventBusExchange, EventBusExchange>();

        services.AddSingleton<IQueryBusRegistry, QueryBusRegistry>();
        services.AddSingleton<IQueryBusExchange, QueryBusExchange>();


        services.AddSingleton<IPluginRegistry, PluginRegistry>();

        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IQueryBus, QueryBus>();
        services.AddScoped<IEngineBus, EngineBus>();

        services.AddCoreMap(o => { o.Scope = CoreMap.Enums.ServiceScope.Transient; }, [
            typeof(RegisterServicesExt),
            typeof(Application.RegisterServicesExt)
            ]);
        return services;
    }
}
