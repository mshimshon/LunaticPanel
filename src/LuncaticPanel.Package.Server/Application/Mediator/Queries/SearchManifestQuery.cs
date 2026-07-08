using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record SearchManifestQuery : IRequest<SearchManifestHandler>
{
    public string? Keywords { get; set; }
    public string? PackageId { get; set; }
    public int? PanelVersion { get; set; }
    public int Position { get; set; }
    public int MaxResult { get; set; } = 50;
}
