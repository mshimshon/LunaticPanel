namespace LunaticPanel.PackageManager.Application.Payloads.Responses;

public sealed record SearchResponse<T>
{
    public IEnumerable<T> Result { get; set; } = new List<T>();
    public int TotalResults { get; set; }
    public int Position { get; set; }
}
