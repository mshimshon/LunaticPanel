using LunaticPanel.Engine.Bootstraper;
using NuGet.Versioning;
using System.Security.Cryptography.X509Certificates;


namespace LunaticPanel.Engine.Hooks.Payloads.Responses;

public record PluginInfoResponse(string Id, NuGetVersion Version, NuGetVersion PanelVersion, BootstrapStartupState Bootup)
{
    public string? Failure { get; init; }
    public bool Failed => Failure != default;
    public bool Loaded { get; init; }
}
