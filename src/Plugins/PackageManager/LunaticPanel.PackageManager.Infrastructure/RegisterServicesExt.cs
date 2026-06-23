using LunaticPanel.PackageManager.Application;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Infrastructure.Repositories;
using LunaticPanel.PackageManager.Infrastructure.Services;
using MedihatR;
using Microsoft.Extensions.DependencyInjection;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Infrastructure;

public static class RegisterServicesExt
{
    public static void AddInfrasctructureServices(this IServiceCollection services)
    {
        services.AddStatePulseServices(p =>
        {
            p.PulseTrackingPerformance = StatePulse.Net.Configuration.PulseTrackingModel.BlazorServerSafe;

        });
        services.AddMedihaterServices();
        services.AddApplicationServices();
        services.AddTransient<IRepositorySourceService, RepositorySourceService>();
        services.AddTransient<IExternalSourceService, ExternalSourceService>();
        services.AddTransient<IPackageRepository, PackageRepository>();
        services.AddTransient<ISourceRepository, SourceRepository>();
    }
}
