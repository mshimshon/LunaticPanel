using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;
namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

internal sealed record ExternalPluginEntityLifecyclePayload
{
    public ExternalPluginEntityLifecycleState State { get; set; } = default!;
    public ExternalPluginEntityLifecycleStartupState StartupState { get; set; } = default!;
    public ExternalPluginEntityLifecycleFailurePayload? Failure { get; set; }
    public DateTime Since { get; set; } = default!;
}
