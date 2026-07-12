using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Mediator.Queries.Handlers;
using LuncaticPanel.Package.Server.Application.Payloads.Requests;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record SearchManifestQuery(ManifestSearchRequest Query) : IRequest<SearchManifestHandler>
{
}
