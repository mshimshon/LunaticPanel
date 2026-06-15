using LunaticPanel.PackageManager.Application.Mediator;
using LunaticPanel.PackageManager.Application.Pulses;
using Microsoft.Extensions.DependencyInjection;


namespace LunaticPanel.PackageManager.Application;

public static class RegisterServicesExt
{
    public static void AddApplicationServices(this IServiceCollection services)
    {

        services.AddApplicationPulses();
        services.AddApplicationMediator();
    }
}
