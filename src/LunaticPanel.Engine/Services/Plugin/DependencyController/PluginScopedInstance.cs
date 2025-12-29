namespace LunaticPanel.Engine.Services.Plugin.DependencyController;

public record PluginScopedInstance(Type ServiceType, object Instance)
{
}
