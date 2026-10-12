using LunaticPanel.Package.Server.Application.Payloads;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record GetLatestPackageQuery(string Id) : IRequest<ManifestPayload>
{

}
