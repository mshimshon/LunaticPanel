using LunaticPanel.PackageManager.Application.Payloads;
using MaksimShimshon.Mediator;
using NuGet.Versioning;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record DownloadPanelUpdatePackagesCommand(ICollection<string> MissingPackages, NuGetVersion PanelVersion)
    : IRequest
{
}
