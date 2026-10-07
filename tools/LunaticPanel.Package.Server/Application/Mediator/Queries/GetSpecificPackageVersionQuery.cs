using LunaticPanel.Package.Server.Application.Mediator.Engine;
using LunaticPanel.Package.Server.Application.Payloads;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record GetSpecificPackageVersionQuery(string Id, string Version) : IRequest<ManifestPayload>
{
}
