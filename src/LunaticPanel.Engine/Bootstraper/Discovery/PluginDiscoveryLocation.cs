using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Bootstraper;
using LunaticPanel.Engine.Plugin;
using System;

namespace LunaticPanel.Engine.Bootstraper.Discovery;

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
    public PluginDiscoveryLocation()
    {

        Initialize();
    }
    public void Initialize()
    {
        string baseLoc = Path.Combine(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel", ".plugins");
        PluginLifecycleLocation = Pathf.CombineReady(baseLoc);
        PluginLifecyclePreInstalledLocation = Pathf.CombineReady(Pathf.DirectorySeparatorChar, "srv", "lunaticpanel", "plugins");
        PluginBootstrapStateLocation = Pathf.CombineReady(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel", "config");
        PluginLifecycleRuntimeLocation = Pathf.CombineReady(baseLoc, "runtime");
        PluginLifecycleApplyLocation = Pathf.CombineReady(baseLoc, "apply");
        PluginLifecycleDeleteLocation = Pathf.CombineReady(baseLoc, "delete");
        PluginLifecycleRollbackLocation = Pathf.CombineReady(baseLoc, "rollbacks");
        PluginLifecycleInstalledLocation = Pathf.CombineReady(baseLoc, "installed");
    }


}
