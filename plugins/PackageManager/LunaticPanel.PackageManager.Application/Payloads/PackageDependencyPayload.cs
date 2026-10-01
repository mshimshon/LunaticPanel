namespace LunaticPanel.PackageManager.Application.Payloads;

public sealed record PackageDependencyPayload
{
    public string Id { get; set; } = default!;
    public string Version { get; set; } = default!;
    public string Name { get; set; } = default!;
}
