using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using System;

namespace LunaticPanel.Engine.Bootstraper.Discovery;

public class UpdateDiscoveryLocation : IUpdateDiscoveryLocator
{
    public string BinaryPathRoot { get; private set; } = default!;
    public string VarLibRoot { get; private set; } = default!;
    public string UpgradeRoot { get; private set; } = default!;
    public string UpgradeReadyApply { get; private set; } = default!;
    public string UpgradePluginInstalled { get; private set; } = default!;
    public string UpgradePluginPreinstalled { get; private set; } = default!;
    public string RuntimeBackupRoot { get; private set; } = default!;
    public string UpgradePluginCache { get; private set; } = default!;




    public UpdateDiscoveryLocation()
    {
        Initialize();
    }
    public void Initialize()
    {
        BinaryPathRoot = Pathf.CombineEnsureCreated(Pathf.DirectorySeparatorChar, "srv", "lunaticpanel");
        VarLibRoot = Pathf.CombineEnsureCreated(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel");
        UpgradeRoot = Pathf.CombineEnsureCreated(VarLibRoot, ".upgrade");

        UpgradePluginCache = Pathf.CombineEnsureCreated(UpgradeRoot, "plugins");
        UpgradeReadyApply = Pathf.CombineEnsureCreated(UpgradeRoot, "apply");
        UpgradePluginInstalled = Pathf.CombineEnsureCreated(UpgradeRoot, "apply", "installed");
        UpgradePluginPreinstalled = Pathf.CombineEnsureCreated(UpgradeRoot, "apply", "preinstalled");
        RuntimeBackupRoot = Pathf.CombineEnsureCreated(VarLibRoot, "upgrade_backup");
    }


}
