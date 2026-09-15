namespace LunaticPanel.PackageManager.Application.Payloads;

public sealed record DiskPackagesPayload
{
    public List<PackagePayload> PendingUpdates { get; set; } = new();
    public List<PackagePayload> AvailableRollbacks { get; init; } = new();
    public List<PackagePayload> PendingDelete { get; init; } = new();
    public List<PackagePayload> Installed { get; init; } = new();
}
