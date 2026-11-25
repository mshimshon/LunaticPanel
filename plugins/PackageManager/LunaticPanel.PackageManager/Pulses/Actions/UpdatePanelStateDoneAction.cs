using NuGet.Versioning;
using StatePulse.Net;
using System;

namespace LunaticPanel.PackageManager.Pulses.Actions;

public sealed record UpdatePanelStateDoneAction : ISafeAction
{
    public NuGetVersion? Found { get; set; }
    public NuGetVersion? Downloaded { get; set; }
    public NuGetVersion? Staged { get; set; }


}
