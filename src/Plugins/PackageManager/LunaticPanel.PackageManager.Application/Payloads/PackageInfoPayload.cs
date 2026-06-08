namespace LunaticPanel.PackageManager.Application.Payloads;

public sealed record PackageInfoPayload
{
    public string PackageId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int Rating { get; set; }
    public int AutoUpdateScore { get; set; }
}
