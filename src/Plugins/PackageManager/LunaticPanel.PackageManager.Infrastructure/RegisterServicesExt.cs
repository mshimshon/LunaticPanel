using LunaticPanel.PackageManager.Application;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.PackageManager.Infrastructure;

public static class RegisterServicesExt
{
    public static void AddInfrasctructureServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
    }
}
