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
        services.AddStatePulseService<PackageUpdateScheduleState>();
        services.AddStatePulseService<LoadPackageRollbackAction>();
        services.AddStatePulseService<LoadPackageRollbackDoneAction>();
        services.AddStatePulseService<LoadPackageRollbackEffect>();
        services.AddStatePulseService<LoadPackageRollbackReducer>();
        services.AddStatePulseService<LoadPackageRollbackDoneReducer>();

        services.AddStatePulseService<InstallNextUpdateAction>();
        services.AddStatePulseService<InstallNextUpdateDoneAction>();
        services.AddStatePulseService<InstallNextUpdateEffect>();
        services.AddStatePulseService<InstallNextUpdateReducer>();
        services.AddStatePulseService<InstallNextUpdateDoneReducer>();

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
