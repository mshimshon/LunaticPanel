using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Application.Messaging.EngineBus;
using LunaticPanel.Engine.Application.Messaging.Event;
using LunaticPanel.Engine.Application.Messaging.Query;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Application;

public static class RegisterServicesExt
{
    public static IServiceCollection AddEngineApplication(this IServiceCollection services)
    {
        services.AddScoped<IEventBus, EventBus>();
        services.AddScoped<IQueryBus, QueryBus>();
        services.AddScoped<IEngineBus, EngineBus>();
        return services;
    }
}
