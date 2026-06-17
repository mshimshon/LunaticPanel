using LunaticPanel.Core.Utils;
using LunaticPanel.PackageManager.Application;
using LunaticPanel.PackageManager.Keys;
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
        services.AddLunaticPanelUtilityServices(PackageManagerKeys.AssemblyName);
        services.AddApplicationServices();
    }
}
