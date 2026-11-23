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
        BinaryPathRoot = Pathf.CombineReady(Pathf.DirectorySeparatorChar, "srv", "lunaticpanel");
        VarLibRoot = Pathf.CombineReady(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel");
        UpgradeRoot = Pathf.CombineReady(VarLibRoot, ".upgrade");

        UpgradePluginCache = Pathf.CombineReady(UpgradeRoot, "plugins");
        UpgradeReadyApply = Pathf.CombineReady(UpgradeRoot, "apply");
        UpgradePluginInstalled = Pathf.CombineReady(UpgradeRoot, "apply", "installed");
        UpgradePluginPreinstalled = Pathf.CombineReady(UpgradeRoot, "apply", "preinstalled");
        RuntimeBackupRoot = Pathf.CombineReady(VarLibRoot, "upgrade_backup");
    }


}
