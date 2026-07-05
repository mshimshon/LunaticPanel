using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Package.Tool.Engine;

public sealed class ConsoleApplicationBuilder
{
    private readonly string[] _args;

    public IServiceCollection Services { get; }
    public ConsoleApplicationBuilder(string[] args)
    {
        Services = new ServiceCollection();
        _args = args;
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