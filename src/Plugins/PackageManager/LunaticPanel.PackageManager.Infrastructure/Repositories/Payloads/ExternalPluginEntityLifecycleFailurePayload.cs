namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

public sealed record ExternalPluginEntityLifecycleFailurePayload
{
    public string Message { get; set; } = default!;
    public DateTimeOffset OccurredAt { get; set; } = default!;
}
