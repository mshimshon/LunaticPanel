using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record GetSpecificPackageVersionQuery(string Id, string Version) : IRequest<ManifestPayload>
{
}
