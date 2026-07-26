using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record GetPackageDownloadTargetQuery(string Id, string Version) : IRequest<PackageDownloadTargetResponse>
{
}
