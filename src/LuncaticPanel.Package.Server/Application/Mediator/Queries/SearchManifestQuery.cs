using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Requests;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record SearchManifestQuery(ManifestSearchRequest Query) : IRequest<ManifestSearchResponse>
{
}
