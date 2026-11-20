using LunaticPanel.Engine.Plugin;
using NuGet.Versioning;
using System.Text.Json.Serialization;

namespace LunaticPanel.Engine.Bootstraper;

public sealed record BootstrapStatusEntity
{
    public BootstrapStatusEntity(PluginManifestEntity manifest, BootstrapStartupState startupState, string? failure)
    {
        Manifest = manifest;
        StartupState = startupState;
        Failure = failure;
    }
    public string Id => Manifest.Id;

    private NuGetVersion? _version;
    [JsonIgnore]
    public NuGetVersion Version
    {
        get
        {
            if (_version == default)
                _version = NuGetVersion.Parse(Manifest.Version);
            return _version;
        }
    }

    private NuGetVersion? _panelVersion;
    [JsonIgnore]
    public NuGetVersion PanelVersion
    {
        get
        {
            if (_panelVersion == default)
                _panelVersion = NuGetVersion.Parse(Manifest.PanelVersion);
            return _panelVersion;
        }
    }
    public PluginManifestEntity Manifest { get; init; }
    public BootstrapStartupState StartupState { get; init; }
    public string? Failure { get; init; }

}
