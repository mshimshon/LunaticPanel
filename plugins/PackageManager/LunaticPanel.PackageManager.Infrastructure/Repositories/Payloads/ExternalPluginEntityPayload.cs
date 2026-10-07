namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

internal sealed record ExternalPluginEntityPayload
{
    public ExternalPluginEntityIdentityPayload Identity { get; set; } = default!;
    public ExternalPluginEntityLifecyclePayload Lifecycle { get; set; } = default!;
}
