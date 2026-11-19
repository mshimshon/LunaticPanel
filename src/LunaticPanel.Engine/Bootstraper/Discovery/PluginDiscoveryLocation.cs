using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
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
    public PluginDiscoveryLocation()
    {

        Initialize();
    }
    public void Initialize()
    {
        string baseLoc = Path.Combine(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel", ".plugins");
        PluginLifecycleLocation = Pathf.CombineEnsureCreated(baseLoc);
        PluginLifecyclePreInstalledLocation = Pathf.CombineEnsureCreated(Pathf.DirectorySeparatorChar, "srv", "lunaticpanel", "plugins");
        PluginBootstrapStateLocation = Pathf.CombineEnsureCreated(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel", "config");
        PluginLifecycleRuntimeLocation = Pathf.CombineEnsureCreated(baseLoc, "runtime");
        PluginLifecycleApplyLocation = Pathf.CombineEnsureCreated(baseLoc, "apply");
        PluginLifecycleDeleteLocation = Pathf.CombineEnsureCreated(baseLoc, "delete");
        PluginLifecycleRollbackLocation = Pathf.CombineEnsureCreated(baseLoc, "rollbacks");
        PluginLifecycleInstalledLocation = Pathf.CombineEnsureCreated(baseLoc, "installed");
    }


}
