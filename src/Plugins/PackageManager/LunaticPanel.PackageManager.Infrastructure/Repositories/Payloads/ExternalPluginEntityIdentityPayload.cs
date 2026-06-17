namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

internal class ExternalPluginEntityIdentityPayload
{
    public string PackageId { get; set; } = default!;
    public string PakageVersion { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? Author { get; set; }
    public string? CompanyName { get; set; }
    public string? License { get; set; }
    public string? Copyright { get; set; }
}
