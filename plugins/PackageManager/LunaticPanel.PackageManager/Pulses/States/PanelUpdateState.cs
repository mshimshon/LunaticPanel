using LunaticPanel.PackageManager.Application.Payloads;
using NuGet.Versioning;
using StatePulse.Net;
namespace LunaticPanel.PackageManager.Pulses.States;

public sealed record PanelUpdateState : IStateFeatureSingleton
{
    public IEnumerable<string> MissingPlugins { get; init; } = Array.Empty<string>();
    public NuGetVersion? AvailableVersion { get; init; }
    public NuGetVersion? DownloadedVersion { get; init; }
    public NuGetVersion? StagedVersion { get; init; }
}
