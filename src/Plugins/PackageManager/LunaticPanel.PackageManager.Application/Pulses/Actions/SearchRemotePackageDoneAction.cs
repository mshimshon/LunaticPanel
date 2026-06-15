using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Actions;

public sealed record SearchRemotePackageDoneAction : IAction
{
    public SearchResponse<PackageInfoPayload> Result { get; set; } = new();

}
