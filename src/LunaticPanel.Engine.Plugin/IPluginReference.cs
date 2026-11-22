using LunaticPanel.Core.Abstraction.Plugin;
using NuGet.Versioning;
using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

public interface IPluginReference
{
    bool IsLoaded { get; }
    IPlugin Load();
    void Unload();
    IPlugin Entry { get; }
    NuGetVersion PanelVersion { get; }
    PluginManifestEntity Manifest { get; }
    NuGetVersion Version { get; }
}
