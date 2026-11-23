using LunaticPanel.Engine.Bootstraper;

namespace LunaticPanel.Engine.Infrastructure.Interfaces;

public interface IPluginRegistry
{
    IPluginLifespanController GetById(string id);
    IPluginLifespanController? GetByIdOrDefault(string id);
    IReadOnlyList<IPluginLifespanController> ActivePlugins { get; }
    IReadOnlyList<BootstrapStatusEntity> KnownPlugins { get; }
    IReadOnlyDictionary<string, string> FailedPlugins { get; }
}
