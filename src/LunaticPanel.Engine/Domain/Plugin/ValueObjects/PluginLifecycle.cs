using LunaticPanel.Engine.Domain.Plugin.Enums;

namespace LunaticPanel.Engine.Domain.Plugin.ValueObjects;

public sealed record PluginLifecycle(PluginState State, PluginStartupState StartupState, PluginFailure? Failure,
        DateTimeOffset Since);