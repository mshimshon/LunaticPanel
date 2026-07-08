using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Query;

namespace LuncaticPanel.Package.Server.Domain.QueryModels;

public interface IManifestQueryModel : IQueryModel
{
    PackageId? Id { get; }
    PackageVersion? Version { get; }
    PackagePanelVersion? PanelVersion { get; }
    PackageTitle? Title { get; }
    string? Keywords { get; }
    bool ShowHidden { get; }
    bool ShowEndOfLife { get; }
}
