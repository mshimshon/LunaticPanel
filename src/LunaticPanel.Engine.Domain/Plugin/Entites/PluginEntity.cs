using LunaticPanel.Engine.Domain.Plugin.ValueObjects;

namespace LunaticPanel.Engine.Domain.Plugin.Entites;

public sealed record PluginEntity
{
    public PluginIdentity Identity { get; init; }
    public PluginLifecycle Lifecycle { get; init; }
    public PluginEntity(PluginIdentity identity, PluginLifecycle lifecycle)
    {
        Identity = identity;
        Lifecycle = lifecycle;
    }
}
