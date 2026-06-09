using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace LunaticPanel.PackageManager.Application;

public static class RegisterServicesExt
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.ConfigureWolverine(options =>
        {
            // Explicitly force Wolverine to scan this Application assembly for handlers
            options.Discovery.IncludeAssembly(typeof(RegisterServicesExt).Assembly);

            // Optional: Configure outbox, error handling policies, or brokers here
        });
    }
}
