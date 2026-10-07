namespace LunaticPanel.PackageManager.Application.Payloads;

public sealed record DiskPackagesPayload
{
    public List<PackagePayload> PendingUpdates { get; set; } = new();
    public List<PackagePayload> AvailableRollbacks { get; set; } = new();
    public List<PackagePayload> PendingDelete { get; set; } = new();
    public List<PackagePayload> Installed { get; set; } = new();
    public List<PackagePayload> PreInstalled { get; set; } = new();
    public List<PackagePayload> RuntimePackages { get; set; } = new();
}
