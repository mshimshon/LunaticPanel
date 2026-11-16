using LunaticPanel.PackageManager.Application.Payloads;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record AutoUpdatePackagesCommand(ICollection<PackageManifestPayload> FoundUpdates, IDictionary<string, string> InstalledPackageVersions)
    : IRequest
{
}
