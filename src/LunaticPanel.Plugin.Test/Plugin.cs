using LunaticPanel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Plugin.Test;

public class Plugin : IPlugin
{
    public void Disable() { }
    public void Enable() { }
    public void Initialize()
    {
        Console.WriteLine("PLugin Initializes");
    }
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<MyService>();
    }
}
