namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

internal sealed record ExternalBootstrapPayload
{
    public List<ExternalPluginPayload> KnownPlugins { get; set; } = new();
}
