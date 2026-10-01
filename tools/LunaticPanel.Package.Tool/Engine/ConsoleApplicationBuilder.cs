using LunaticPanel.Core.Extensions;
using LunaticPanel.Package.Tool.Payloads;
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
        var sdkVersion = typeof(PluginManifestPayload).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion!.Split('+')[0];
        Console.Out.WriteLine($"sdkVersion:{sdkVersion}".Magenta());
        var sdkVersionObj = new Version(sdkVersion!);
        PackSettings.LunaticPanelVersion = $"{sdkVersionObj.Major}.{sdkVersionObj.Minor}.{sdkVersionObj.Build}";
        var rootTmp = Path.GetTempPath();
        var cleanFolder = Path.Combine(rootTmp, $"lunaticpanel.lpkg.{PackSettings.LunaticPanelVersion}");
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