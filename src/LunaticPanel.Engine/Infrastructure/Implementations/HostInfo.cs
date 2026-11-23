using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Keys;

namespace LunaticPanel.Engine.Infrastructure.Implementations;

internal class HostInfo : IPluginInfo
{
    public string PluginId => "LunaticPanel";

    public IReadOnlyList<MessageKey> Keys { get; }
    public HostInfo()
    {
        var keys = typeof(LPEngineKeys).Assembly.ScanKeyPackageForKeys().ToList();
        CrazyReportConsole.ReportInfo("HOSTINFO -> ");
        foreach (var key in keys.Select(p => p.ToString()))
        {
            CrazyReportConsole.ReportInfo($"{key}");
        }
        CrazyReportConsole.ReportInfo("<- HOSTINFO ");
        Keys = keys;
    }
}
