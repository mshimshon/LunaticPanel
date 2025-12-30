using LunaticPanel.Engine.Application;
using LunaticPanel.Engine.Application.Messaging.EngineBus;
using LunaticPanel.Engine.Application.Messaging.Event;
using LunaticPanel.Engine.Application.Messaging.Query;
using LunaticPanel.Engine.Infrastructure.Messaging.EngineBus;
using LunaticPanel.Engine.Infrastructure.Messaging.Event;
using LunaticPanel.Engine.Infrastructure.Messaging.Query;
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

        return services;
    }
}
