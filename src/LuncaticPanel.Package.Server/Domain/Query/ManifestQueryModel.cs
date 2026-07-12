using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Query.ValueObjects;
using LuncaticPanel.Package.Server.Domain.QueryModels;

namespace LuncaticPanel.Package.Server.Domain.Query;

public sealed record ManifestQueryModel : IManifestQueryModel
{
    public PackageId? Id { get; init; }
    public PackagePanelVersion? PanelVersion { get; init; }

    public QueryKeywords? Keywords { get; init; }

    public bool ShowHidden { get; init; }

    public bool ShowEndOfLife { get; init; }

    public int Position { get; init; }

    public int MaxResult { get; init; } = 10;
    public ManifestQueryModel()
    {
        MaxResult = MaxResult > 100 ? 100 : MaxResult <= 10 ? 10 : MaxResult;
    }
}
