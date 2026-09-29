using LunaticPanel.PackageManager.Application.Payloads;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.States;

public sealed record PackageManagerDiskEditorState : IStateFeatureSingleton
{
    public IEnumerable<PackagePayload> PendingRollbacks { get; init; } = Array.Empty<PackagePayload>();
    public bool IsLoading { get; init; }
}
