using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
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
    }
    public static void AddLinuxCommandUtilityService(this IServiceCollection services)
    {
        services.AddScoped<ILinuxCommand, LinuxCommandRunner>();
    }
    public static void AddPluginLocationUtilityService(this IServiceCollection services, string assemblyName)
    {
        services.AddTransient<IPluginLocation>((sp) => new PluginLocation(assemblyName));
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
}
