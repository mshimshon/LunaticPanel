using LunaticPanel.Package.Server.Application.Mediator.Engine;
using LunaticPanel.Package.Server.Application.Payloads.Requests;
using LunaticPanel.Package.Server.Application.Payloads.Responses;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record SearchManifestQuery(ManifestSearchRequest Query) : IRequest<ManifestSearchResponse>
{
}
