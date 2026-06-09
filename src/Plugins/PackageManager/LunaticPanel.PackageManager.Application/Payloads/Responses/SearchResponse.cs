namespace LunaticPanel.PackageManager.Application.Payloads.Responses;

public sealed record SearchResponse<T>
{
    public List<T> Result { get; set; } = new();
    public int Total { get; set; }
    public int Position { get; set; }
}
