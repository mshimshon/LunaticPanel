using LunaticPanel.PackageManager.Application.Payloads;
using MaksimShimshon.Mediator;
using NuGet.Versioning;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record PeriodicCheckPackageUpdatesQuery(IDictionary<string, NuGetVersion> InstalledPackages)
    : IRequest<ICollection<PackageManifestPayload>>
{
}
