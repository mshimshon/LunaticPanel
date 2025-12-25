namespace GameServerManager.Engine.Domain.Plugin.ValueObjects;

public sealed record PluginFailure(
    string Message,
    DateTimeOffset OccurredAt
);
