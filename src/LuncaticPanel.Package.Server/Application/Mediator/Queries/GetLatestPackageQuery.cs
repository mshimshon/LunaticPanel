using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record GetLatestPackageQuery(string Id) : IRequest<ManifestPayload>
{

}
