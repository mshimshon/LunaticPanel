using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application;
using LunaticPanel.Engine.Application.Messaging.EngineBus;
using LunaticPanel.Engine.Application.Messaging.Event;
using LunaticPanel.Engine.Application.Messaging.Query;
using LunaticPanel.Engine.Application.Plugin;
using LunaticPanel.Engine.Infrastructure.Messaging.EngineBus;
using LunaticPanel.Engine.Infrastructure.Messaging.Event;
using LunaticPanel.Engine.Infrastructure.Messaging.Query;
using LunaticPanel.Engine.Infrastructure.Plugin;
using LunaticPanel.Engine.Infrastructure.Plugin.DependencyController;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Infrastructure;

public static class RegisterServicesExt
{
    public static IServiceCollection AddEngineInfrastructure(this IServiceCollection services)
    {
        services.AddEngineApplication();
        services.AddSingleton<IEventBusRegistry, EventBusRegistry>();
        services.AddSingleton<IQueryBusRegistry, QueryBusRegistry>();
        services.AddSingleton<IEngineBusRegistry, EngineBusRegistry>();
        services.AddSingleton<IPluginRegistry, PluginRegistry>();
        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IQueryBus, QueryBus>();
        services.AddScoped<IEngineBus, EngineBus>();
        services.AddScoped<PluginDependencyInjectionController>();
        return services;
    }
}
