using LunaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LunaticPanel.Package.Server.Domain.Query.ValueObjects;

namespace LunaticPanel.Package.Server.Domain.Query;

public interface IManifestQueryModel : IQueryModel
{
    PackageId? Id { get; }
    PackagePanelVersion? PanelVersion { get; }
    QueryKeywords? Keywords { get; }
    bool ShowHidden { get; }
    bool ShowEndOfLife { get; }
}
