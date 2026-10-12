using LunaticPanel.Package.Server.Application.Payloads.Responses;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record GetPackageDownloadTargetQuery(string Id, string Version) : IRequest<PackageDownloadTargetResponse>
{
}
