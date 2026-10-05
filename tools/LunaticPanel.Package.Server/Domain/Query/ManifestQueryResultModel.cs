using LunaticPanel.Package.Server.Domain.Entites;

namespace LunaticPanel.Package.Server.Domain.Query;

public sealed record ManifestQueryResultModel : IManifestQueryResultModel
{
    public ICollection<ManifestEntity> Result { get; init; } = Array.Empty<ManifestEntity>();

    public int Position { get; init; }

    public int Total { get; init; }
}
