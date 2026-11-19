using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys;

namespace LunaticPanel.Engine.Infrastructure.Plugin;

internal class HostInfo : IPluginInfo
{
    public string PluginId => "LunaticPanel";

    public IReadOnlyList<MessageKey> Keys { get; }
    public HostInfo()
    {
        var keys = typeof(LPEngineKeys).Assembly.ScanKeyPackageForKeys().ToList();
        Console.WriteLine("HOSTINFO -> " + string.Join(',', keys.Select(p => p.ToString())));
        Keys = keys;
    }
}
