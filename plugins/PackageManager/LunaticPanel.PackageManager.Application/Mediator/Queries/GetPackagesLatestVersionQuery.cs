using LunaticPanel.PackageManager.Application.Payloads;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record GetPackagesLatestVersionQuery(IEnumerable<string> Packages)
    : IRequest<ICollection<PackageManifestPayload>>
{
}
