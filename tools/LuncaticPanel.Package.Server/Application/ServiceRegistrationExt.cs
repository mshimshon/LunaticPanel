using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace LuncaticPanel.Package.Server.Application;

public static class ServiceRegistrationExt
{
    internal static void AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddMediatorServices();
    }
}
