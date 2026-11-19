
using LunaticPanel.Core.Abstraction.Messaging.Common;

namespace LunaticPanel.Core.Abstraction.Plugin;

public interface IPluginInfo
{
    string PluginId { get; }
    IReadOnlyList<MessageKey> Keys { get; }
}
