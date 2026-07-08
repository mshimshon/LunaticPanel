namespace LuncaticPanel.Package.Server.Application.Payloads.Responses;

public sealed record ManifestSearchResponse
{
    public ICollection<ManifestPayload> Result { get; init; } = new List<ManifestPayload>();
    public int Position { get; init; }
    public int TotalResults { get; init; }
}
