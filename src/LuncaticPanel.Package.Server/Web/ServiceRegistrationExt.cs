using LuncaticPanel.Package.Server.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LuncaticPanel.Package.Server.Web;

public static class ServiceRegistrationExt
{
    internal static void AddWebLayerServices(this IServiceCollection services)
    {
        services.AddInfrastructureLayerServices();
    }
}
