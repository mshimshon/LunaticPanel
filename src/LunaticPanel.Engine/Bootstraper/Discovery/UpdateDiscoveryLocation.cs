using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Web.Boostrap.Plugin;
using System;

namespace LunaticPanel.Engine.Web.Bootstraper.Discovery;

public class UpdateDiscoveryLocation
{
    public string BinaryPathRoot { get; private set; } = default!;
    public string VarLibRoot { get; private set; } = default!;
    public string UpgradeRoot { get; private set; } = default!;
    public string UpgradeReadyApply { get; private set; } = default!;
    public string UpgradePluginInstalled { get; private set; } = default!;
    public string UpgradePluginPreinstalled { get; private set; } = default!;
    public string RuntimeBackupRoot { get; private set; } = default!;
    public string UpgradePluginCache { get; private set; } = default!;

    public bool IsLinux => OperatingSystem.IsLinux();
    public bool IsWindows => OperatingSystem.IsWindows();
    internal string PathCombine(params string[] parts)
    {
        string path = Path.Combine([$"{Path.DirectorySeparatorChar}", .. parts]);
        if (IsWindows)
        {
            path = Path.Combine([Path.GetTempPath(), "lunaticpanel", "linux", .. parts]);
        }
        EnsureCreate(path);
        return path;
    }

    internal string EnsureCreate(params string[] parts)
    {
        string path = Path.Combine(parts);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    internal string EnsureCreate(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    public UpdateDiscoveryLocation()
    {
        Initialize();
    }
    public void Initialize()
    {
        BinaryPathRoot = PathCombine("srv", "lunaticpanel");
        VarLibRoot = PathCombine("var", "lib", "lunaticpanel");
        UpgradeRoot = EnsureCreate(VarLibRoot, ".upgrade");

        UpgradePluginCache = EnsureCreate(UpgradeRoot, "plugins");
        UpgradeReadyApply = EnsureCreate(UpgradeRoot, "apply");
        UpgradePluginInstalled = EnsureCreate(UpgradeRoot, "apply", "installed");
        UpgradePluginPreinstalled = EnsureCreate(UpgradeRoot, "apply", "preinstalled");
        RuntimeBackupRoot = PathCombine("lunaticpanel", "upgrade_backup");
    }


}
