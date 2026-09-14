using LuncaticPanel.Package.Server.Domain.Entites;

namespace LuncaticPanel.Package.Server.Domain.Query;

public sealed record ManifestQueryResultModel : IManifestQueryResultModel
{
    public ICollection<ManifestEntity> Result { get; init; } = Array.Empty<ManifestEntity>();

    public int Position { get; init; }

    public int Total { get; init; }
}
