using LunaticPanel.Package.Server.Application.Payloads.Requests;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record SearchManifestQuery(ManifestSearchRequest Query) : IRequest<ManifestSearchResponse>
{
}
