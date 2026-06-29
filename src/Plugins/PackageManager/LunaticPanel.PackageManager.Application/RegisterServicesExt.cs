using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.PackageManager.Application.Mediator;
using LunaticPanel.PackageManager.Application.Pulses;


namespace LunaticPanel.PackageManager.Application;

public static class RegisterServicesExt
{
    public static void AddApplicationServices(this IPluginServiceCollection services)
    {

        services.Services.AddApplicationPulses();
        services.Services.AddApplicationMediator();
    }
}
