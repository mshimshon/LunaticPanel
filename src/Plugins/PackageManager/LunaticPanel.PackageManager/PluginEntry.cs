using LunaticPanel.Core;
using LunaticPanel.Core.Extensions;
using LunaticPanel.PackageManager.Keys;

namespace LunaticPanel.PackageManager;

public class PluginEntry : PluginBase
{
    public override void CheckFeatureDegradation(Func<string, bool> isBusAvailable)
    { }

    public override string[] GetMyPackageKeys()
        => typeof(PackageManagerKeys).Assembly.ScanKeyPackageForKeys();
}
