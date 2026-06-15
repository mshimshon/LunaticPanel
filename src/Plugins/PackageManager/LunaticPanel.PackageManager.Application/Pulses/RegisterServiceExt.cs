using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.Effects;
using LunaticPanel.PackageManager.Application.Pulses.Reducers;
using LunaticPanel.PackageManager.Application.Pulses.States;
using Microsoft.Extensions.DependencyInjection;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses;

internal static class RegisterServiceExt
{
    public static void AddApplicationPulses(this IServiceCollection services)
    {

        services.AddStatePulseService<SearchPackageState>();
        services.AddStatePulseService<RepositorySourceState>();
        services.AddStatePulseService<PackageManagerState>();

        services.AddStatePulseService<LoadLocalPackagesDoneReducer>();
        services.AddStatePulseService<LoadLocalPackagesReducer>();
        services.AddStatePulseService<LoadLocalPackagesEffect>();
        services.AddStatePulseService<LoadLocalPackagesDoneAction>();
        services.AddStatePulseService<LoadLocalPackagesAction>();

        services.AddStatePulseService<SearchRemotePackageAction>();
        services.AddStatePulseService<SearchRemotePackageDoneAction>();
        services.AddStatePulseService<SearchRemotePackageEffect>();
        services.AddStatePulseService<SearchRemotePackageDoneReducer>();
        services.AddStatePulseService<SearchRemotePackageReducer>();

    }
}
