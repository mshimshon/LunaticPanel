using LunaticPanel.DebugTool.Payloads;
using LunaticPanel.DebugTool.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.DebugTool.Engine;

internal class ConsoleApplication
{
    public IServiceProvider ServiceProvider { get; }
    public ConfigurationPayload Configuration { get; }

    public ConsoleApplication(IServiceCollection services, ConfigurationPayload configuration)
    {
        ServiceProvider = services.BuildServiceProvider();
        Configuration = configuration;
    }



    public async Task RunAsync(CancellationToken ct = default)
    {
        var deployment = new DeploymentService(Configuration);
        await deployment.DeployAsync(ct);
    }
}
