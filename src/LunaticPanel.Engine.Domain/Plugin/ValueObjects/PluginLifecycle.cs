using GameServerManager.Engine.Domain.Plugin.Enums;

namespace GameServerManager.Engine.Domain.Plugin.ValueObjects;

public sealed record PluginLifecycle(PluginState State, PluginStartupState StartupState, PluginFailure? Failure,
        DateTimeOffset Since);