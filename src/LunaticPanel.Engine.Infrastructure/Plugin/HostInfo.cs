using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys;

namespace LunaticPanel.Engine.Infrastructure.Plugin;

internal class HostInfo : IPluginInfo
{
    public string PluginId => "LunaticPanel";

    public IReadOnlyList<string> Keys { get; }
    public HostInfo()
    {
        var keys = typeof(LPEngineKeys).Assembly.ScanKeyPackageForKeys().Select(p => p.ToLower()).ToList();
        Console.WriteLine("HOSTINFO -> " + string.Join(',', keys));
        Keys = keys;
    }
}
