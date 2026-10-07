
namespace LunaticPanel.Core.Abstraction.Plugin;

public interface IPluginInfo
{
    string PluginId { get; }
    IReadOnlyList<string> Keys { get; }
}
