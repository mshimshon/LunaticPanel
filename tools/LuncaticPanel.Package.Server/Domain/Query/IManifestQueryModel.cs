using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Query;
using LuncaticPanel.Package.Server.Domain.Query.ValueObjects;

namespace LuncaticPanel.Package.Server.Domain.QueryModels;

public interface IManifestQueryModel : IQueryModel
{
    PackageId? Id { get; }
    PackagePanelVersion? PanelVersion { get; }
    QueryKeywords? Keywords { get; }
    bool ShowHidden { get; }
    bool ShowEndOfLife { get; }
}
