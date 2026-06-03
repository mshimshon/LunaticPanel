
namespace LunaticPanel.Core.Abstraction.Plugin;

public interface IPluginInfo
{
    string PluginId { get; }
    string[] Keys { get; }
}
