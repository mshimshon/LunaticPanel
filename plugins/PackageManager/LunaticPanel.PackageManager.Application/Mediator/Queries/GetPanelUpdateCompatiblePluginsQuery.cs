using LunaticPanel.PackageManager.Application.Payloads;
using MaksimShimshon.Mediator;
using NuGet.Versioning;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record GetPanelUpdateCompatiblePluginsQuery(ICollection<string> Packages, NuGetVersion PanelVersion) : IRequest<ICollection<PackageManifestPayload>>
{
}
