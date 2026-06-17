namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

internal sealed record ExternalPluginPayload
{
    public ExternalPluginEntityPayload Entity { get; set; } = default!;
    public string PluginDir { get; set; } = default!;
}
