using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Web.Boostrap.Plugin;
using System;

namespace LunaticPanel.Engine.Web.Bootstraper.Discovery;

public class PluginDiscoveryLocation : IPluginDiscoveryLocator
{


    public string PluginLifecycleLocation { get; private set; } = default!;
    public string PluginLifecyclePreInstalledLocation { get; private set; } = default!;
    public string PluginBootstrapStateLocation { get; private set; } = default!;
    public string PluginLifecycleRuntimeLocation { get; private set; } = default!;
    public string PluginLifecycleApplyLocation { get; private set; } = default!;
    public string PluginLifecycleDeleteLocation { get; private set; } = default!;
    public string PluginLifecycleRollbackLocation { get; private set; } = default!;
    public string PluginLifecycleInstalledLocation { get; private set; } = default!;
    internal string PathCombine(params string[] parts)
    {
        string path = Path.Combine([$"{Path.DirectorySeparatorChar}", .. parts]);
        // Note: This is only for testing esp unit tests.. fallback normally as targeted linux.
        if (OperatingSystem.IsWindows())
        {
            path = Path.Combine([Path.GetTempPath(), "lunaticpanel", "linux", .. parts]);
        }
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    public PluginDiscoveryLocation()
    {

        Initialize();
    }
    public void Initialize()
    {
        string baseLoc = Path.Combine("var", "lib", "lunaticpanel", ".plugins");
        PluginLifecycleLocation = PathCombine(baseLoc);
        PluginLifecyclePreInstalledLocation = PathCombine("srv", "lunaticpanel", "plugins");
        PluginBootstrapStateLocation = PathCombine("var", "lib", "lunaticpanel", "config");
        PluginLifecycleRuntimeLocation = PathCombine(baseLoc, "runtime");
        PluginLifecycleApplyLocation = PathCombine(baseLoc, "apply");
        PluginLifecycleDeleteLocation = PathCombine(baseLoc, "delete");
        PluginLifecycleRollbackLocation = PathCombine(baseLoc, "rollbacks");
        PluginLifecycleInstalledLocation = PathCombine(baseLoc, "installed");
    }


}
