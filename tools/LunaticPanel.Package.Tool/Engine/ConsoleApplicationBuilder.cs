using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Package.Tool.Payloads;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Versioning;
using System.Reflection;

namespace LunaticPanel.Package.Tool.Engine;

public sealed class ConsoleApplicationBuilder
{
    private readonly string[] _args;

    public IServiceCollection Services { get; }
    public ConsoleApplicationBuilder(string[] args)
    {
        Services = new ServiceCollection();
        _args = args;
        Console.WriteLine($"ARGS = {string.Join(' ', args)}");
        var sdkVersion = typeof(PluginManifestEntity).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        Console.Out.WriteLine($"sdkVersion:{sdkVersion}".Magenta());
        var sdkVersionObj = NuGetVersion.Parse(sdkVersion!);
        PackSettings.lunaticPanelVersion = $"{sdkVersionObj}";
        var rootTmp = Path.GetTempPath();
        var cleanFolder = Path.Combine(rootTmp, $"lunaticpanel.lpkg.{PackSettings.lunaticPanelVersion}");
        if (Directory.Exists(cleanFolder))
            foreach (var item in Directory.GetDirectories(cleanFolder))
            {
                try
                {
                    Directory.Delete(item, true);
                }
                catch { }
            }
        else
        {
            Directory.CreateDirectory(cleanFolder);
        }
    }

    public ConsoleApplicationRuntime Build()
    {
        var sp = Services.BuildServiceProvider();
        return new ConsoleApplicationRuntime(_args)
        {
            ServiceProvider = sp.CreateScope().ServiceProvider,
        };
    }
}
