using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.PackageManager.Application;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Infrastructure.Repositories;
using LunaticPanel.PackageManager.Infrastructure.Services;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Infrastructure;

public static class RegisterServicesExt
{
    public static void AddInfrasctructureServices(this IPluginServiceCollection services)
    {
        services.AddApplicationServices();
        services.Services.AddStatePulseServices(p =>
        {
            p.PulseTrackingPerformance = StatePulse.Net.Configuration.PulseTrackingModel.BlazorServerSafe;

        });
        services.Services.AddMedihaterServices();
        services.AddTransient<IRepositorySourceService, RepositorySourceService>();
        services.AddTransient<IExternalSourceService, ExternalSourceService>();
        services.AddTransient<IPackageRepository, PackageRepository>();
        services.AddTransient<ISourceRepository, SourceRepository>();
    }
}
