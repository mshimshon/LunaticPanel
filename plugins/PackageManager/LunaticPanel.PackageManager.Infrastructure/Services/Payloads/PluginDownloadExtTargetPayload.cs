namespace LunaticPanel.PackageManager.Infrastructure.Services.Payloads;

public sealed record PluginDownloadExtTargetPayload
{
    public string Target { get; init; } = default!;
}
