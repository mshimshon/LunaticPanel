using LunaticPanel.Engine.Domain.Plugin.Enums;
using LunaticPanel.Engine.Domain.Plugin.ValueObjects;

namespace LunaticPanel.Engine.Web.Boostrap;

internal static class BootstrapPluginsMapping
{

    public static BootstrapPluginDescriptor FailedToLoadMapping(this BootstrapPluginDescriptor info, string message, DateTimeOffset? occuredAt = default)
    => info with
    {
        Entity = info.Entity with
        {
            Lifecycle = info.Entity.Lifecycle with
            {
                State = PluginState.Failed,
                Failure = new PluginFailure(message, occuredAt ?? DateTimeOffset.UtcNow)
            }
        }
    };


    public static BootstrapPluginDescriptor MissingPluginMapping(this BootstrapPluginDescriptor info)
        => info with
        {
            Entity = info.Entity with
            {
                Lifecycle = info.Entity.Lifecycle with
                {
                    State = PluginState.Missing
                }
            }
        };

    public static BootstrapPluginDescriptor DisablePluginMapping(this BootstrapPluginDescriptor info)
        => info with
        {
            Entity = info.Entity with
            {
                Lifecycle = info.Entity.Lifecycle with
                {
                    State = PluginState.Unloaded
                }
            }
        };

    public static BootstrapPluginDescriptor ActivatedPluginMapping(this BootstrapPluginDescriptor info)
    => info with
    {
        Entity = info.Entity with
        {
            Lifecycle = info.Entity.Lifecycle with
            {
                State = PluginState.Active
            }
        }
    };

    public static BootstrapPluginDescriptor EnablePluginMapping(this BootstrapPluginDescriptor info)
=> info with
{
    Entity = info.Entity with
    {
        Lifecycle = info.Entity.Lifecycle with
        {
            StartupState = PluginStartupState.Enabled
        }
    }
};
}
