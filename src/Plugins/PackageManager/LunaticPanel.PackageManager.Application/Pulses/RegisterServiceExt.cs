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
        services.AddStatePulseService<PackageInstallState>();
        services.AddStatePulseService<PackageManagerDiskState>();

        services.AddStatePulseService<PackageUpdateScheduleState>();

        services.AddStatePulseService<InstallPackageAction>();
        services.AddStatePulseService<InstallPackageDoneAction>();
        services.AddStatePulseService<InstallPackageEffect>();
        services.AddStatePulseService<InstallPackageReducer>();
        services.AddStatePulseService<InstallPackageDoneReducer>();

        services.AddStatePulseService<AddSourceAction>();
        services.AddStatePulseService<AddSourceDoneAction>();
        services.AddStatePulseService<AddSourceEffect>();
        services.AddStatePulseService<AddSourceReducer>();
        services.AddStatePulseService<AddSourceDoneReducer>();

        services.AddStatePulseService<RemoveSourceAction>();
        services.AddStatePulseService<RemoveSourceDoneAction>();
        services.AddStatePulseService<RemoveSourceEffect>();
        services.AddStatePulseService<RemoveSourceReducer>();
        services.AddStatePulseService<RemoveSourceDoneReducer>();

        services.AddStatePulseService<LoadPackageDiskFoldersAction>();
        services.AddStatePulseService<LoadPackageDiskFoldersDoneAction>();
        services.AddStatePulseService<LoadPackageDiskFoldersEffect>();
        services.AddStatePulseService<LoadPackageDiskFoldersReducer>();
        services.AddStatePulseService<LoadPackageDiskFoldersDoneReducer>();

        services.AddStatePulseService<SearchRemotePackageAction>();
        services.AddStatePulseService<SearchRemotePackageDoneAction>();
        services.AddStatePulseService<SearchRemotePackageEffect>();
        services.AddStatePulseService<SearchRemotePackageDoneReducer>();
        services.AddStatePulseService<SearchRemotePackageReducer>();

        services.AddStatePulseService<LoadSourcesAction>();
        services.AddStatePulseService<LoadSourcesDoneAction>();
        services.AddStatePulseService<LoadSourcesEffect>();
        services.AddStatePulseService<LoadSourcesReducer>();
        services.AddStatePulseService<LoadSourcesDoneReducer>();

        services.AddStatePulseService<SaveSourcesAction>();
        services.AddStatePulseService<SaveSourcesDoneAction>();
        services.AddStatePulseService<SaveSourcesEffect>();
        services.AddStatePulseService<SaveSourcesReducer>();
        services.AddStatePulseService<SaveSourcesDoneReducer>();



    }
}
