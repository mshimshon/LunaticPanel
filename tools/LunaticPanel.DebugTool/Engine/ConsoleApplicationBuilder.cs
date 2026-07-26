using LunaticPanel.DebugTool.Extensions;
using LunaticPanel.DebugTool.Payloads;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.DebugTool.Engine;

internal class ConsoleApplicationBuilder
{
    public IServiceCollection Services { get; }
    public ConfigurationPayload Configuration { get; }
    public ConsoleApplicationBuilder(string[] args)
    {
        Services = new ServiceCollection();
        Configuration = ConfigurationExt.GenerateConfiguration(args);

    }
    public ConsoleApplication Build()
        => new ConsoleApplication(Services, Configuration);
}
