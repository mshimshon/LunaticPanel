using LuncaticPanel.Package.Server.Web;
using Microsoft.Extensions.DependencyInjection;

namespace LuncaticPanel.Package.Server;

public static class ServiceRegistrationExt
{
    public static void AddPackageServeServices(this IServiceCollection services)
    {
        services.AddWebServices();
    }
}
