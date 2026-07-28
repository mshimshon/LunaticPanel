namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

internal sealed record ExternalPluginEntityIdentityPayload
{
    public string PackageId { get; set; } = default!;
    public string PakageVersion { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? Author { get; set; }
    public string? CompanyName { get; set; }
    public string? License { get; set; }
    public string? Copyright { get; set; }
    public string? Description { get; set; }
}
