namespace LunaticPanel.Engine.Infrastructure.Plugin.DependencyController;

public record PluginScopedInstance(Type ServiceType, object Instance)
{
}
