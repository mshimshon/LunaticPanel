using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.PackageManager.Application.Mediator;


namespace LunaticPanel.PackageManager.Application;

public static class RegisterServicesExt
{
    public static void AddApplicationServices(this IPluginServiceCollection services)
    {

        services.Services.AddApplicationMediator();
    }
}
