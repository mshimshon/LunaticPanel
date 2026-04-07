using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.LinuxCommand;
using LunaticPanel.Core.Utils.Logging;
using LunaticPanel.Core.Utils.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Utils;

public static class RegisterServicesExt
{
    public static void AddLinuxCommandUtilityService(this IServiceCollection serviceDescriptors)
    {
        serviceDescriptors.AddScoped<ILinuxCommand, CommandRunner>();
    }
    public static void AddPluginLocationUtilityService(this IServiceCollection serviceDescriptors, string assemblyName)
    {
        serviceDescriptors.AddTransient<IPluginLocation>((sp) => new PluginLocation(assemblyName));
    }

    public static void AddCrazyReportUtilityService(this IServiceCollection serviceDescriptors)
    {
        serviceDescriptors.AddScoped(typeof(ICrazyReport<>), typeof(CrazyReport<>));
        serviceDescriptors.AddScoped<ICrazyReport, CrazyReport>();
    }
}
