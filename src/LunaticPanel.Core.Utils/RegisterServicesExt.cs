using LunaticPanel.Core.Utils.Abstraction.FileWatcher;
using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.Core.Utils.FileWatcher;
using LunaticPanel.Core.Utils.LinuxCommand;
using LunaticPanel.Core.Utils.Logging;
using LunaticPanel.Core.Utils.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Utils;

public static class RegisterServicesExt
{
    public static void AddLunaticPanelUtilityServices(this IServiceCollection services, string assemblyName)
    {
        services.AddLinuxCommandUtilityService();
        services.AddPluginLocationUtilityService(assemblyName);
        services.AddCrazyReportUtilityService();
        services.AddSafeFileWriterUtilityService();
        services.AddSafeFileWriterUtilityService();
        services.AddFileWatcherFactoryUtilityService();
    }

    public static void AddLinuxCommandUtilityService(this IServiceCollection services)
    {
        services.AddScoped<ILinuxCommand, LinuxCommandRunner>();
        services.AddCrazyReportUtilityService();
    }
    public static void AddPluginLocationUtilityService(this IServiceCollection services, string assemblyName)
    {
        //TODO: REGISTER ALL INDIVIDUALLY INCLUDE USER
        services.AddTransient<IPluginLocation>((sp) => new PluginLocation(assemblyName));
        services.AddScoped<IPluginSystemLocation>((sp) => new PluginLocation(assemblyName));
        services.AddScoped<IPluginWebLocation>((sp) => new PluginLocation(assemblyName));
        // IPluginSystemLocation
    }

    public static void AddCrazyReportUtilityService(this IServiceCollection services)
    {
        services.AddTransient(typeof(ICrazyReport<>), typeof(CrazyReport<>));
        services.AddTransient<ICrazyReport, CrazyReport>();
    }

    public static void AddSafeFileWriterUtilityService(this IServiceCollection services)
    {
        services.AddScoped<ISafeFileWriter, SafeFileWriter.SafeFileWriter>();
    }

    public static void AddFileWatcherFactoryUtilityService(this IServiceCollection services)
    {
        services.AddScoped<IFileWatcherSystemFactory, FileWatcherSystemFactory>();
    }

    public static void AddFileWatcherFor<TAction>(this IServiceCollection services, string path, string filePattern, FileWatchEvent[] whatToWatch, Func<TAction, IServiceProvider, Task> onNotify)
        where TAction : IFileWatcherAction
    {
        services.AddScoped<IFileWatcherSystem<TAction>>((sp) => new FileWatcherSystem<TAction>(path, filePattern, whatToWatch, onNotify, sp));
    }

    public static void AddSingletonFileWatcherFor<TAction>(this IServiceCollection services, string path, string filePattern, FileWatchEvent[] whatToWatch, Func<TAction, IServiceProvider, Task> onNotify)
    where TAction : IFileWatcherAction
    {
        services.AddSingleton<IFileWatcherSystem<TAction>>((sp) => new FileWatcherSystem<TAction>(path, filePattern, whatToWatch, onNotify, sp));
    }

    public static IFileWatcherSystem<TAction> StartFileWatcherFor<TAction>(this IServiceProvider sp)
        where TAction : IFileWatcherAction => sp.GetRequiredService<IFileWatcherSystem<TAction>>();
}
