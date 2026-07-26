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
        if (Configuration.PerformCleanup || Configuration.PerformSoftCleanup)
        {
            if (Configuration.PerformCleanup && Configuration.PerformSoftCleanup)
                Console.WriteLine("--hard-reset and --soft-clean detected, ignoring hard-reset just in case... use either one! hard-reset clears WSL disks and images along side soft-cleaning temp directories.");
            var deployment = new DeploymentService(Configuration);

            await deployment.CleanUp(Configuration.PerformSoftCleanup, ct);
        }
        else
            Console.WriteLine("Skip deployment no --clean.");

        if (Configuration.PerformDeploy)
        {
            var deployment = new DeploymentService(Configuration);
            await deployment.DeployAsync(ct);
        }
        else
            Console.WriteLine("Skip deployment no --deploy.");
        Console.WriteLine("Lunatic Panel Development CLI completed.");

    }
}
