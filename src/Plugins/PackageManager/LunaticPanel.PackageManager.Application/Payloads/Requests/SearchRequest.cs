namespace LunaticPanel.PackageManager.Application.Payloads.Requests;

public sealed record SearchRequest
{
    public string Keywords { get; set; } = default!;
    public int Position { get; set; }
    public int MaxResult { get; set; }
}
