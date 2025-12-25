using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application;
using LunaticPanel.Engine.Application.Messaging.Event;
using LunaticPanel.Engine.Application.Messaging.Query;
using LunaticPanel.Engine.Infrastructure.Messaging.Event;
using LunaticPanel.Engine.Infrastructure.Messaging.Query;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Infrastructure;

public static class RegisterServicesExt
{
    public static IServiceCollection AddLunaticPanelEngineInfrastructure(this IServiceCollection services)
    {
        services.AddLunaticPanelEngineApplication();
        services.AddScoped<IEventBus, EventBus>();
        services.AddSingleton<IEventBusRegistry, EventBusRegistry>();

        services.AddScoped<IQueryBus, QueryBus>();
        services.AddSingleton<IQueryBusRegistry, QueryBusRegistry>();

        return services;
    }
}
