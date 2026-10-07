using LunaticPanel.Package.Server.Application.Mediator.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Package.Server.Application;

public static class ServiceRegistrationExt
{
    internal static void AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddMediatorServices();
    }
}
