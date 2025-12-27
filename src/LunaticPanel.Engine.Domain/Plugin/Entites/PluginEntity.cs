using LunaticPanel.Engine.Domain.Plugin.ValueObjects;

namespace LunaticPanel.Engine.Domain.Plugin.Entites;

public sealed record PluginEntity
{
    public PluginIdentity Identity { get; }
    public PluginLifecycle Lifecycle { get; }
    public PluginEntity(PluginIdentity identity, PluginLifecycle lifecycle)
    {
        Identity = identity;
        Lifecycle = lifecycle;
    }
}
