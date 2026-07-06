using LunaticPanel.Core;

namespace LunaticPanel.PackageManager;

public class PluginA : PluginBase
{
    public override void CheckFeatureDegradation(Func<string, bool> isBusAvailable)
    {

    }
    public override string[] GetMyPackageKeys() => Array.Empty<string>();
}
